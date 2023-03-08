using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace BK390A_Display
{
    /*
    BK390A Communication Protocol
    The BK PRECISION 390A Multimeter (aka BK390) has a communication capability via an optical port which is converted to a USB output to a computer.
    This is handled in Windows by a FTDI driver as a virtual COM port.  The communications is hard coded to 2400 Baud, 7 data bits, 1 stop bit and Odd parity.
    The BK390 outputs a continuous stream of 11 byte packets in the following format:
    Byte # (base 0)
    0	Range:	The measurement range
    1	Most significant digit	Value displayed by BK390
    2	Next significant digit	
    3	Next significant digit	
    4	Least significant digit	4 digits in total without decimal point
    5	Function:	The selection of the rotary dial
    6	Status:	This byte signifies indicators and whether the value is negative
    7	Option 1:	Measurement mode byte defining: PMax, PMin VAHZ
    8	Option 2:	Operation mode byte defining: DC, AC, Auto, APO modes
    9, 10	CR,LF	Carriage return / line feed pair 0D, 0A delimits the stream
    
    Note: The bytes in the stream all start with 011 binary, or when converted to an 8 bit byte: 0011 or 30H, 
    which means the first nibble of each byte is only significant for the Value bytes (1 to 4) and can be ignored for all the other bytes.
    
    Range (Byte 0)
    The following Range codes are defined:
    Note:	The RPM mode is not a known feature of the BK390A model so is not shown here.
            Blank table entries are not applicable.
    Code 	    V	        mA	        uA	        Ohm	        Frequency	Capacitance
    00110000	400.0 mV	40.00 mA	400.0 μA	400.0 Ω	    4.000 kHz	4.000 nF
    00110001	4.000 V	    400.0 mA	4000 μA	    4.000 kΩ	40.00 kHz	40.00 nF
    00110010	40.00 V			                    40.00 kΩ	400.0 kHz	400.0 nF
    00110011	400.0 V			                    400. kΩ	    4.00 MHz	4.000 μF
    00110100	4000 V			                    4.000 MΩ	40.00 MHz	40.00 μF
    00110101				                        40.00 MΩ	400.0 MHz	400.0 μF
    00110110						                                        4.000 mF
    00110111						                                        40.00 mF
    
    Value (Bytes 1-4)
    The measurement value is presented as four bytes in ASCII format without the decimal point, which has to be inferred by the Range code shown above.
    If the display should show O.L. (see the Status byte below) the value should be ignored.  
    I.e.: a Value of “1234” and a Function byte of 00111011 (Voltage) and a Range byte of 00110010 should be interpreted as 12.34V.
 
    Function (Byte 5)
    The Function byte is related to the selection of the rotary dial on the BK390.  
    Note that there are functions defined which are not available on the BK390 such as 
    ADP0 (00111110), ADP1 (00111100), ADP2 (00111000), ADP3 (00111010).  
    Also the RPM function is the same as Frequency but can be ignored for the BK390A.
    Code	    Mode	    Comment
    00111011	Voltage	
    00111101	μA Current	
    00111001	mA Current	
    00111111	A Current	
    00110011	Ohms	
    00110101	Continuity	
    00110001	Diode	
    00110010	Frequency	Also used for RPM mode but ignored for the BK390A
    00110110	Capacitance	
    00110100	Temperature	The “Judge” bit in the Status byte determines °C or °F
    
    Status (Byte 6)
    The Status byte is used to define the unit C or F in temperature mode and whether the value is positive or negative or the value should be displayed as O.L.
    The most significant four bits are always 0011 and can be ignored.
    Bit #	Usage
    3	    So-called “Judge” bit. In Temperature mode if 1 it is °C if 0 it is °F (Also used for RPM but ignored for BK390A)
    2	    “Sign” bit, if 1 the value is negative, if 0 it is positive
    1	    “Batt” bit is 1 if the battery is low
    0	    “OL” bit if 1 the value should be ignored and displayed as O.L. or overload
	
    Option1 (Byte 7)
    This byte defines special measurement modes, the most significant four bits are always 0011 and can be ignored.
    Bit #	Usage
    3	    Pmax mode, the value is the Max value
    2	    Pmin mode, the value is the Min value
    1	    Always 0 and n/a
    0	    VAHz, the frequency of the V or A measurement, i.e. the Hz button was pressed while in V or A modes.
    
    Option2 (Byte 8)
    This byte defines operation modes, the most significant four bits are always 0011 and can be ignored.
    Bit #	Usage
    3	    DC mode for Voltage or Current
    2	    AC Mode for Voltage or Current
    1	    Auto is set to 1 if the BK390 is in Auto range mode
    0	    APO is set to 1 if the APO mode is active (Auto Power Off)
    */
    /// <summary>Class BK390Collector collects the multimeter data which consists of 11 character strings ending in cr/lf.</summary>
    public class BK390Collector : IEnumerable<string>
    {
        private const int packetLenght = 10;  //Complete packet lenght including cr

        private readonly string cr = Encoding.Default.GetString("\r"u8.ToArray()); //LF is stripped off by coms readline

        /// <summary>The raw data build from the data collected from the BK390 serial output.</summary>
        private string rawData = string.Empty;

        /// <summary>The last packet
        /// that was stored in the queue for comparison</summary>
        private string lastPacket = string.Empty;

        /// <summary>The queue (a FIFO stack) where we are storing completed BK390 output lines.</summary>
        private readonly ConcurrentQueue<string> queue = new();

        /// <summary>Gets the count of the line items available for processing.</summary>
        /// <value>The count.</value>
        public int Count { get => queue.Count; }

        /// <summary>Clears the collector of any data.</summary>
        public void Clear()
        { 
            queue.Clear(); 
            rawData = string.Empty;
        }

        /// <summary>
        /// Collects the specified data string and adds it to the queue if complete.
        /// </summary>
        /// <param name="data">The data to add.</param>
        /// <returns>Returns the number of packets ready to be processed.</returns>
        public int Collect(string data)
        {
            rawData += data; //Add data to the line to be collected.
            string dataPacket;

            /// (Local Method for the Collect Method)
            /// <summary>Removes the CRLF delimiter from the packet and returns the undelimited data if it ends with CRLF.</summary>
            /// <param name="packet">The packet to process.</param>
            /// <returns>System.String.</returns>
            string RemoveDelimiter(string packet)
            {
                if (packet.EndsWith(cr))
                    return packet.Substring(0, packet.Length - cr.Length);
                else
                    return packet; //Should not happen ...
            }

            /// (Local Method for the Collect Method)
            /// <summary>Gets the first packet from multi packet raw dat and remoces it from the raw data.</summary>
            /// <returns>The first packet.</returns>
            string GetFirstPacket()
            {
                string pkt = string.Empty;
                int crlfLocation = rawData.IndexOf(cr);
                pkt = rawData.Substring(0, crlfLocation); //Won't include the crlf
                rawData = rawData.Substring(crlfLocation + cr.Length);
                return pkt;
            }

            /// <summary>
            /// (Local Method for the Collect Method)
            /// Extracts the packet from the rawData received. 
            /// Returns an undelimited data packet if the rawData has a complete data packet.  
            /// If a delimited data packet is found will update the raw data by removing the packet.
            /// </summary>
            /// <returns>Return an undelimited data packet if found or an empty string if none.</returns>
            string ExtractPacket()
            {
                string packet = string.Empty;
                //Clean up the data incase a few extranious characters were received or some where missing which is possible at the start
                //If the bytes before the crlf are < 9 ignore the input if more than include onlly the 9 bytes before the crlf.
                if (rawData.Contains(cr))  //Packet end received
                {
                    if (rawData.EndsWith(cr))  //Packet End
                    {
                        if (rawData.Length < packetLenght)
                        {
                            rawData = string.Empty; // Must be junk, too short
                        }
                        else if (rawData.Length > packetLenght) //Too long, remove extraneous chars
                        {
                            packet = GetFirstPacket();
                        }
                        else // rawData == packet lenght
                        {
                            packet = RemoveDelimiter(rawData);
                            rawData = string.Empty;
                        }
                        return packet;
                    }
                    else // Only process one packet at the time so extract it from the raw data leaving any incomplete packets
                    {
                        packet = GetFirstPacket();
                    }
                } // If no crlf packet end nothing to do right now
                return string.Empty;
            } //Extract Packet

            //the start of the Collect() method.
            while (rawData.Contains(cr))
            {
                dataPacket = ExtractPacket();
                if (!string.IsNullOrEmpty(dataPacket))
                {
                    if (lastPacket != dataPacket) //If no change just ignore it
                    {
                        queue.Enqueue(dataPacket);
                        lastPacket = dataPacket;
                    }
                }
            }
            return queue.Count;
        }

        /// <summary>Gets the top packet from the queue if one is available.</summary>
        /// <returns>True if there was a data packet available, false otherwise</returns>
        public bool GetPacket(out string packet)
        {
            packet = string.Empty;
            if (queue.IsEmpty)
                return false;
            return queue.TryDequeue(out packet);
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<string> GetEnumerator()
        {
            return ((IEnumerable<string>)queue).GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator">IEnumerator</see> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)queue).GetEnumerator();
        }

        public BK390Collector() { }

    }
}
