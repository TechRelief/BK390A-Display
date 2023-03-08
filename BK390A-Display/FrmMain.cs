using System;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;

/// <summary>
/// The BK390A_Display namespace encorporates the code to display the output from the BK PRECISION 390A multimeter so it can be used in YouTube videos etc.
/// </summary>
namespace BK390A_Display
{
    public partial class FrmMain : Form
    {
        /// <summary>The COM port
        /// object used to access the serial data from the BK-390A</summary>
        private SerialPort ComPort = null;

        private readonly BK390Collector collector = new();
        private readonly BK390Decoder decoder = new();


        /// <summary>Delegate BK390DataDelegate
        /// is used to pass data from the non-ui thread to the ui for processing and display.</summary>
        /// <param name="data">The data received from the 390A Multimeter.</param>
        private delegate void BK390DataDelegate(string data);

        public FrmMain()
        {
            InitializeComponent();
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            MnuPopup.Show(BtnStart, 0, 0);
        }

        /// <summary>Events to receive data from the serial COM port</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SerialDataReceivedEventArgs" /> instance containing the event data.</param>
        void EventComDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //Thread.Sleep(500);
            string data = ComPort.ReadLine();
            if (data != string.Empty)
                this.BeginInvoke(new BK390DataDelegate(DelegateDataReceived), new object[] { data });
        }

        /// <summary>Delegate to receive the data from the BK390A so it can be send for processing and display in th UI.</summary>
        /// <param name="data">The data to process.</param>
        private void DelegateDataReceived(string data)
        {
            _ = collector.Collect(data);
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            foreach (string packet in collector)
            {
                if (decoder.Decode(packet))
                {
                    TxtValue.Text = /*decoder.Data.Sign + */ decoder.Data.Value;
                    TxtACDC.Text = decoder.Data.ACDC;
                    TxtMode.Text = decoder.Data.Mode;
                    TxtStatus.Text = decoder.Data.Status;
                    TxtUnit.Text = decoder.Data.Prefix + decoder.Data.Unit + decoder.Data.Postfix;
                    TxtOptions.Text = decoder.Data.VAHz + " " + decoder.Data.MinMax;
                }
            }
        }

        private void ItmStartStop_Click(object sender, EventArgs e)
        {
            BtnStart_Click(sender, e);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            TxtValue.Text = "-----";
            TxtACDC.Text = string.Empty;
            TxtMode.Text = string.Empty;
            TxtStatus.Text = string.Empty;
            TxtUnit.Text = string.Empty;
            TxtOptions.Text = string.Empty;
        }

        /// <summary>Stops the COM port and Update Timer.</summary>
        private void StopComPort()
        {
            try
            {
                UpdateTimer.Stop();
                ComPort?.Close();
            }
            catch { }
        }

        /// <summary>Starts the COM port and Update Timer.</summary>
        /// <returns>
        ///   <c>true</c> if successfully started, <c>false</c> otherwise.</returns>
        private string StartComPort()
        {
            try
            {
                string comPortName = Properties.Settings.Default.ComPort;
                ComPort ??= new SerialPort(comPortName, 2400, Parity.Odd, 7, StopBits.One);  //TODO make COM port configurable
                ComPort.Handshake = Handshake.None;
                ComPort.DataReceived += new SerialDataReceivedEventHandler(EventComDataReceived);
                ComPort.ReadTimeout = -1;
                ComPort.WriteTimeout = -1;
                ComPort.Open();
                UpdateTimer.Start();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private void ItmExit_Click(object sender, EventArgs e)
        {
            StopComPort();
            this.Close();
        }

        //Stuff to make a borderless window move
        private bool mouseDown;
        private Point lastLocation;

        private void PnlMain_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void PnlMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }

        private void PnlMain_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void ItmStartStop_Click_1(object sender, EventArgs e)
        {
            if (ComPort != null && ComPort.IsOpen)
            {
                StopComPort();
            }
            else
            {
                string errMsg = StartComPort();
                if (errMsg != string.Empty)
                    MessageBox.Show("Error opening serial port: " + errMsg, "Error!");
            }
        }

        private void LblHandle_MouseDown(object sender, MouseEventArgs e)
        {
            PnlMain_MouseDown(sender, e);
        }

        private void LblHandle_MouseMove(object sender, MouseEventArgs e)
        {
            PnlMain_MouseMove(sender, e);
        }

        private void LblHandle_MouseUp(object sender, MouseEventArgs e)
        {
            PnlMain_MouseUp(sender, e);
        }

        private void ItmComPort_Click(object sender, EventArgs e)
        {
            using (AboutBox box = new())
            {
                box.ShowDialog(this);
            }
        }
    }
}
