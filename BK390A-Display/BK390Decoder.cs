using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BK390A_Display
{
    /// <summary>Class BK390Data is used to house the BK390A data string and the decoded values during decoding.</summary>
    public class BK390Data
    {
        // Raw data retrieved from the serial data received from the BK390A multimeter. (~ means invalid data)
        /// <summary>
        /// The range code is a byte send as the first byte in the data packet by the BK390A.
        /// It defines where the decimal point should be inserted
        /// </summary>
        public byte RangeCode;
        /// <summary>
        /// The value string consists of four digits with leading zeros i.e. 1.0 would be represented as "0010"
        /// </summary>
        public string ValueString;
        /// <summary>
        /// The function code is a bit map representing the function selected on the BK390A dial: Voltage, Current, Resistance, Temperature, etc.
        /// </summary>
        public byte FuncCode;
        /// <summary>
        /// The status code is a bitmap defining things like the sign (positive/negative) O.L. C or F for temperature, etc.
        /// </summary>
        public byte StatusCode;
        /// <summary>
        /// The option1 code is a bitmap defining MAX, MIN, VAHz modes
        /// </summary>
        public byte Opt1Code;
        /// <summary>
        /// The option2 code is a bitmap defining AC, DC, Auot, Range and APO modes
        /// </summary>
        public byte Opt2Code;

        // Decoded data
        /// <summary>
        /// The number of decimal places for the value
        /// </summary>
        public int Decimals;
        /// <summary>
        /// The mode will be Voltage, Current, Resistance, Temperature etc., depending on the setting of the dial on the BK390A 
        /// </summary>
        public string Mode;
        /// <summary>
        /// The status will be Auto or Range
        /// </summary>
        public string Status;
        /// <summary>
        /// The sign negative sign or blank for positive
        /// </summary>
        public string Sign;
        /// <summary>
        /// The value consists of four characters with leading zeroes and no decimal point
        /// </summary>
        public string Value;
        /// <summary>
        /// The prefix if either blank or like k, M etc. It goes before the unit like kHz
        /// </summary>
        public string Prefix;
        /// <summary>
        /// The unit like V, A, Hz, C, F etc.
        /// </summary>
        public string Unit;
        /// <summary>
        /// The postfix is any thing that might come after the unit or is blank
        /// </summary>
        public string Postfix;
        /// <summary>
        /// The MIN, MAX or blank, shows if the BK390A is recording minimum and / or maximum values
        /// </summary>
        public string MinMax;
        /// <summary>
        /// The VA Hz mode is active if the Hz button is pressed on the BK390A or blank otherwise
        /// </summary>
        public string VAHz;
        /// <summary>
        /// The will contain AC, DC for voltage or current or blank otherwise 
        /// </summary>
        public string ACDC;
        /// <summary>
        /// The APO is Auto power off but is not displayed in this version
        /// </summary>
        public string APO;

        /// <summary>Initializes a new instance of the <see cref="T:BK390A_Display.BK390Data" /> class.
        /// It is where the raw fields and decoded properties are stored.</summary>
        public BK390Data()
        {
            Clear();
        }

        /// <summary>Clears the data in this instace of BK390Data ready for new data..</summary>
        public void Clear()
        {
            RangeCode = 0x00;
            ValueString = "0000";
            FuncCode = 0x00;
            StatusCode = 0x00;
            Opt1Code = 0x00;
            Opt2Code = 0x00;
            Decimals = 0;
            Mode = string.Empty; 
            Status = string.Empty;
            Sign = " ";
            Value = string.Empty;
            Prefix = string.Empty;
            Unit = string.Empty;
            Postfix = string.Empty;
            MinMax = string.Empty; // If the Option 1 Min or Max bits are set it will be Min or Max.
            VAHz = string.Empty; //If option 1 VAHz bit is set it will contain "VA Hz"
            ACDC = string.Empty; //If appropriate will have AC or DC
            APO =   string.Empty; //If APO bit in Option 2 is set will contain "APO"

        }

    }

    /// <summary>Class BK390Decoder takes the packet data and decodes it ready for display.</summary>
    public class BK390Decoder
    {
        //Bit masks used for decoding
        const byte FunctionVoltage = 0b00111011; // 3BH 59D
        const byte FunctionCurrent_UA  = 0b00111101;
        const byte FunctionCurrent_MA  = 0b00111001;
        const byte FunctionCurrent_A  = 0b00111111;
        const byte FunctionOhms  = 0b00110011;
        const byte FunctionContinuity  = 0b00110101;
        const byte FunctionDiode  = 0b00110001;
        const byte FunctionFqRPM  = 0b00110010;
        const byte FunctionCapacitance  = 0b00110110;
        const byte FunctionTemperature  = 0b00110100;
        //const byte FunctionADP0  = 0b00111110; //Not sure what these are for
        //const byte FunctionADP1  = 0b00111100;
        //const byte FunctionADP2  = 0b00111000;
        //const byte FunctionADP3  = 0b00111010;

        //Masks for Status byte
        const byte StatusOLMask = 0b00000001;
        //const byte StatusBattMask = 0b00000010; //Not concerned with the BATT signal
        const byte StatusSignMask = 0b00000100;
        const byte StatusJudgeMask = 0b00001000;

        //Masks for Option 1 byte
        const byte Option1VAHzMask = 0b00000001;
        const byte Option1MinMask = 0b00000100;
        const byte Option1MaxMask = 0b00001000;

        //Masks for Option 2 byte
        const byte Option2APOMask = 0b00000001;
        const byte Option2AutoMask = 0b00000010; // If bit is set it is Auto
        const byte Option2ACMask = 0b00000100;
        const byte Option2DCMask = 0b00001000;

        // Byte offset into data packet
        const int ByteRange = 0;
        const int ByteValue = 1;
        const int ByteFunction = 5;
        const int ByteStatus = 6;
        const int ByteOpt1 = 7;
        const int ByteOpt2 = 8;

        /// <summary>Extracts the string from the location specified.</summary>
        /// <param name="s">The string to extract from.</param>
        /// <param name="location">The location and length of the string to extract.</param>
        /// <returns>The Extracted String</returns>
        /// <exception cref="System.ArgumentException">Extract: string parameter is invalid</exception>
        private static string ExtractString(string s, int[] location)
        {
            if (string.IsNullOrEmpty(s) || s.Length < (location[0] + location[1]))
                throw new ArgumentException("Extract: string parameter is invalid");

            return s.Substring(location[0], location[1]);
        }

        /// <summary>
        /// Extracts the byte value at the specified location.
        /// </summary>
        /// <param name="s">The string to extract the byte value from.</param>
        /// <param name="location">The location to extract the byte from.</param>
        /// <param name="mask">The mask.</param>
        /// <returns>System.Byte the 8 bit value at the location specified.</returns>
        private static byte ExtractByte(string s, int[] location, byte mask = 255)
        {
            byte b = Encoding.UTF8.GetBytes(ExtractString(s, location))[0];
            return (byte)(b & mask);
        }

        // Some special characters to use for display
        public const string Micro = "μ";
        public const string Degree = "°";
        public const string Ohm = "Ω";

        // The position and lenght of the fields in the BK390 data string.
        readonly int[] posRange =  { ByteRange, 1 };
        readonly int[] posValue =  { ByteValue, 4 };
        readonly int[] posFunc =   { ByteFunction, 1 };
        readonly int[] posStatus = { ByteStatus, 1};
        readonly int[] posOpt1 =   { ByteOpt1, 1 };
        readonly int[] posOpt2 =   { ByteOpt2, 1 };

        private BK390Data data = new();

        /// <summary>Gets the data object where the data is stored and decoded from.</summary>
        /// <value>The data object.</value>
        public BK390Data Data { get => data;/* set => data = value; */}

        /// <summary>Initializes a new instance of the <see cref="T:BK390A_Display.BK390Decoder" /> class.</summary>
        public BK390Decoder()
        {
        }

        /// <summary>Formats the value by placing the decimal point in the right location and stripping of leading zeroes.</summary>
        /// <param name="value">The value to format.</param>
        /// <param name="decimals">The number of decimals.</param>
        /// <param name="sign">The sign blank or -.</param>
        /// <returns>The formatted value.</returns>
        private static string FormatValue(string value, int decimals, string sign)
        {
            if (!double.TryParse(value, out double d)) //This will remove leading zeroes
                return "O.L.";
            
            switch (decimals)
            { 
                case 0:
                    return sign + d.ToString("F0").PadLeft(5);
                case 1:
                    return sign + (d / 10).ToString("F1").PadLeft(5);
                case 2:
                    return sign + (d / 100).ToString("F2").PadLeft(5);
                case 3:
                    return sign + (d / 1000).ToString("F3").PadLeft(5);
                default:
                    return sign + d.ToString("F0").PadLeft(5); //Shouldn't happen
            }
         }

        /// <summary>Decodes the specified BK390 data string, returns true if succesful, false otherwise.</summary>
        /// <param name="packet">The string to decode</param>
        /// <returns><c>true</c> if the decode was successful, <c>false</c> otherwise.</returns>
        public bool Decode(string packet)
        {
            data.Clear();

            if (string.IsNullOrEmpty(packet))
                return false;

            try
            {
                //First extract the data elements
                data.RangeCode = ExtractByte(packet, posRange, 0x0F);
                data.ValueString = ExtractString(packet, posValue);
                data.FuncCode = ExtractByte(packet, posFunc);
                data.StatusCode = ExtractByte(packet, posStatus);
                data.StatusCode = ExtractByte(packet, posStatus);
                data.Opt1Code = ExtractByte(packet, posOpt1);
                data.Opt2Code = ExtractByte(packet, posOpt2);
            }
            catch { return false; }

            //Now decode them and format them for displaying
            switch (data.FuncCode)
            {
                case FunctionVoltage:
                    data.Unit = "V";
                    data.Mode = "Voltage";
                    switch(data.RangeCode)
                    {
                        case 0:
                            data.Decimals = 1;
                            data.Prefix = "m";
                            break;
                        case 1: data.Decimals = 3; break;
                        case 2: data.Decimals = 2; break;
                        case 3: data.Decimals = 1; break;
                        case 4: data.Decimals = 0; break;
                    }
                    break;

                case FunctionCurrent_UA:
                    data.Unit = "A";
                    data.Mode = "Current";
                    data.Prefix = Micro;
                    switch (data.RangeCode)
                    { 
                        case 0: data.Decimals = 1; break;
                        case 1: data.Decimals = 0; break;
                    }
                    break;

                case FunctionCurrent_MA:
                    data.Unit = "A";
                    data.Mode = "Current";
                    data.Prefix = "m";
                    switch (data.RangeCode)
                    {
                        case 0: data.Decimals = 2; break;
                        case 1: data.Decimals = 1; break;   
                    }
                    break;

                case FunctionCurrent_A:
                    data.Unit = "A";
                    data.Mode = "Current";
                    data.Decimals = 2;
                    break;

                case FunctionOhms:
                    data.Unit = Ohm;
                    data.Mode = "Resistance";
                    switch (data.RangeCode)
                    {
                        case 0: data.Decimals = 1; break;
                        case 1: data.Decimals = 3; data.Prefix = "k"; break;
                        case 2: data.Decimals = 2; data.Prefix = "k"; break;
                        case 3: data.Decimals = 1; data.Prefix = "k"; break;
                        case 4: data.Decimals = 3; data.Prefix = "M"; break;
                        case 5: data.Decimals = 2; data.Prefix = "M"; break;
                    }
                    break;

                case FunctionContinuity:
                    data.Unit = Ohm;
                    data.Mode = "Continuity";
                    data.Decimals = 1;
                    break;

                case FunctionDiode:
                    data.Unit = "V";
                    data.Mode = "Diode";
                    data.Decimals = 3;
                    break;

                case FunctionFqRPM:
                    data.Unit = "Hz";
                    data.Mode = "Frequency";
                    if ((data.StatusCode & StatusJudgeMask) == 0)
                    {
                        switch (data.RangeCode)
                        {
                            case 0: data.Decimals = 3; data.Prefix = "k"; break;
                            case 1: data.Decimals = 2; data.Prefix = "k"; break;
                            case 2: data.Decimals = 1; data.Prefix = "k"; break;
                            case 3: data.Decimals = 3; data.Prefix = "M"; break;
                            case 4: data.Decimals = 2; data.Prefix = "M"; break;
                            case 5: data.Decimals = 1; data.Prefix = "M"; break;
                        }
                    }
                    else
                    {
                        data.Unit = "RPM";
                        data.Mode = "RPM";
                        switch (data.RangeCode)
                        {
                            case 0: data.Decimals = 2; data.Prefix = "k"; break;
                            case 1: data.Decimals = 1; data.Prefix = "k"; break;
                            case 2: data.Decimals = 3; data.Prefix = "M"; break;
                            case 3: data.Decimals = 2; data.Prefix = "M"; break;
                            case 4: data.Decimals = 1; data.Prefix = "M"; break;
                            case 5: data.Decimals = 0; data.Prefix = "M"; break;
                        }
                    }
                    break;

                case FunctionCapacitance:
                    data.Unit = "F";
                    data.Mode = "Capacitance";
                    switch (data.RangeCode)
                    {
                        case 0: data.Decimals = 3; data.Prefix = "n"; break;
                        case 1: data.Decimals = 2; data.Prefix = "n"; break;
                        case 2: data.Decimals = 1; data.Prefix = "n"; break;
                        case 3: data.Decimals = 3; data.Prefix = Micro; break;
                        case 4: data.Decimals = 2; data.Prefix = Micro; break;
                        case 5: data.Decimals = 1; data.Prefix = Micro; break;
                        case 6: data.Decimals = 3; data.Prefix = "m"; break;
                        case 7: data.Decimals = 2; data.Prefix = "m"; break;
                    }
                    break;

                case FunctionTemperature:
                    data.Mode = "Temperature";
                    data.Decimals = 0;
                    data.Prefix = Degree;
                    if ((data.StatusCode & StatusJudgeMask) != 0)
                        data.Unit = "C";
                    else
                        data.Unit = "F";
                    break;
            }

            // Now get various options for display
            if ((data.StatusCode & StatusSignMask) != 0)
                data.Sign = "-";
            if ((data.Opt2Code & Option2AutoMask) == 0)
                data.Status = "Range";
            else
                data.Status = "Auto";
            if ((data.StatusCode & StatusOLMask) != 0)
                data.Value = "O.L.";
            else
                data.Value = FormatValue(data.ValueString, data.Decimals, data.Sign);
            if ((data.Opt1Code & Option1MinMask) != 0)
                data.MinMax = "MIN"; // If the Option 1 Min or Max bits are set it will be Min or Max.
            else if ((data.Opt1Code & Option1MaxMask) != 0)
                data.MinMax = "MAX";
            if ((data.Opt1Code & Option1VAHzMask) != 0)
            {
                data.VAHz = "VA Hz";
                data.Unit = "Hz";
                data.Prefix = "k";
            }
            if ((data.Opt2Code & Option2DCMask) != 0)
                data.ACDC = "DC";
            else if ((data.Opt2Code & Option2ACMask) != 0)
                data.ACDC = "AC";
            if ((data.Opt2Code & Option2APOMask) != 0)
                data.APO = "APO";
            return true;
        }
    }
}
