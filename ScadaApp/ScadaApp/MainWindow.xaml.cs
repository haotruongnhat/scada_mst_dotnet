using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

using S7.Net;
using System.Timers;
using System.Windows.Threading;

namespace ScadaApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PlcHandler handler;
        bool isConnected = false;

        bool PumpState = false;
        double PumpValue = 0;
        bool ConveyorState = false;
        bool SeperatorState = false;
        bool BottlingState = false;

        string PumpTriggerAddress = "Q0.0";
        string BottlingTriggerAddress = "Q0.1";
        string ConveyorTriggerAddress = "Q0.2";
        string SeperatorTriggerAddress = "Q0.3";

        string StatusMessage = "Standby";

        public MainWindow()
        {
            InitializeComponent();

            // Get ip address
            ConnectButton.Click += ConnectButtonClicked;
            PumpButton.Click += PumpButtonClicked;
            SeperatorButton.Click += SeperatorButtonClicked;
            BottlingButton.Click += BottlingButtonClicked;
            ConveyorButton.Click += ConveyorButtonClicked;
            PumpSlider.ValueChanged += PumpValueChanged;
            EmergencyStopButton.Click += EmergencyStopButtonClicked;

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(OnTimedEvent);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherTimer.Start();
        }

        public void ConnectButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
            {
                string address = PlcAddress.Text;
                try
                {
                    handler = new PlcHandler(address, false);
                    isConnected = handler.isConnected;
                    StatusMessage = "Connected";
                }
                catch
                {
                    StatusMessage = "Cannot Connect";
                }

            }
            else
            {
                handler.Close();
                isConnected = false;
            }
        }

        public void EmergencyStopButtonClicked(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                handler.WriteState(PumpTriggerAddress, false);
                PumpState = false;
                handler.WriteState(ConveyorTriggerAddress, false);
                PumpState = false;
                handler.WriteState(SeperatorTriggerAddress, false);
                PumpState = false;
                handler.WriteState(BottlingTriggerAddress, false);
                PumpState = false;

                PumpSlider.Value = 0;
                handler.WriteDB(100, 4, 0);
            }
        }

        public void PumpButtonClicked(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                handler.WriteState(PumpTriggerAddress, !PumpState);
                PumpState = !PumpState;
            }
        }

        public void ConveyorButtonClicked(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                handler.WriteState(ConveyorTriggerAddress, !ConveyorState);
                ConveyorState = !ConveyorState;
            }
        }

        public void SeperatorButtonClicked(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                handler.WriteState(SeperatorTriggerAddress, !SeperatorState);
                SeperatorState = !SeperatorState;
            }
        }

        public void BottlingButtonClicked(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                if(!BottlingState) { 
                    string sMessageBoxText = "Will activate bottling - WATER LEAKED WARNING. Continue?";
                    string sCaption = "Warning";

                    MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
                    MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                    MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

                    switch (rsltMessageBox)
                    {
                        case MessageBoxResult.Yes:
                            BottlingState = !BottlingState;
                            break;

                        case MessageBoxResult.No:
                            /* ... */
                            break;
                    }
                }
                else
                {
                    BottlingState = !BottlingState;
                }
                handler.WriteState(BottlingTriggerAddress, !BottlingState);
            }
        }

        public void PumpValueChanged(object sender, RoutedEventArgs e)
        {
            PumpValue = PumpSlider.Value;
            handler.WriteDB(100, 4, (float)PumpValue);
        }

        private void OnTimedEvent(object sender, EventArgs e)
        {

            if (isConnected)
            {
                handler.ReadState();
                SetUIState();
            }
            else
            {
                ResetState();
            }
        }

        public void SetUIState()
        {
            ConnectButton.Background = handler.isConnected ? Brushes.Green : Brushes.Red;

            I_4B1.Fill = handler.S_4B1 ? Brushes.Green : Brushes.Red;
            I_4B2.Fill = handler.S_4B2 ? Brushes.Green : Brushes.Red;
            I_4B3.Fill = handler.S_4B3 ? Brushes.Green : Brushes.Red;
            I_4B5.Fill = handler.S_4B5 ? Brushes.Green : Brushes.Red;
            I_4B6.Fill = handler.S_4B6 ? Brushes.Green : Brushes.Red;
            I_4B4.Fill = handler.S_4B4 ? Brushes.Green : Brushes.Red;

            PumpButton.Background = handler.S_4M1 ? Brushes.Green : Brushes.Red;
            ConveyorButton.Background = handler.S_4M3 ? Brushes.Green : Brushes.Red;
            SeperatorButton.Background = handler.S_4M4 ? Brushes.Green : Brushes.Red;
            BottlingButton.Background = handler.S_4M2 ? Brushes.Green : Brushes.Red;


            double tank2_UpperBound = 100;
            double tank2_LowerBound = 0;

            double sensor_UpperBound = 27648;
            double sensor_LowerBound = 190;

            Tank2.Value = (tank2_UpperBound - tank2_LowerBound)*handler.D_4CO1 / (sensor_UpperBound - sensor_LowerBound);

            StatusText.Text = StatusMessage;
        }

        public void ResetState()
        {
            ConnectButton.Background = Brushes.Red;

            I_4B1.Fill = Brushes.Yellow;
            I_4B2.Fill = Brushes.Yellow;
            I_4B3.Fill = Brushes.Yellow;
            I_4B5.Fill = Brushes.Yellow;
            I_4B6.Fill = Brushes.Yellow;
            I_4B4.Fill = Brushes.Yellow;

            ConveyorButton.Background = Brushes.Yellow;
            SeperatorButton.Background = Brushes.Yellow;
            BottlingButton.Background = Brushes.Yellow;
            PumpButton.Background = Brushes.Yellow;
        }
    }

    public class PlcHandler
    {
        Plc plcInstance;
        string plcIpAddress;
        bool testMode;
        public bool isConnected = false;

        public bool S_4B1;
        public bool S_4B2;
        public bool S_4B3;
        public bool S_4B4;
        public bool S_4B5;
        public bool S_4B6;

        public bool S_4M1;
        public bool S_4M2;
        public bool S_4M3;
        public bool S_4M4;

        public float D_4CO1 = 0;
        public float D_4CI1 = 0;

        public PlcHandler(string ipAdress, bool test)
        {
            plcIpAddress = ipAdress;
            testMode = test;
            if (testMode == false)
            {
                PlcInitialization();
            }
        }

        ~PlcHandler()
        {
            if (testMode == false)
            {
                plcInstance.Close();
            }
        }

        public void PlcInitialization()
        {
            plcInstance = new Plc(CpuType.S7300, plcIpAddress, 0, 2);

            plcInstance.ReadTimeout = 2000;
            plcInstance.WriteTimeout = 2000;

            //Check exception
            plcInstance.Open();
            isConnected = plcInstance.IsConnected;
        }

        public void Close()
        {
            plcInstance.Close();
            isConnected = false;
        }

        public void ReadState()
        {
            if (isConnected)
            {
                S_4B1 = (bool)plcInstance.Read("I0.0");
                S_4B2 = (bool)plcInstance.Read("I0.1");
                S_4B3 = (bool)plcInstance.Read("I0.2");
                S_4B4 = (bool)plcInstance.Read("I0.3");
                S_4B5 = (bool)plcInstance.Read("I0.4");
                S_4B6 = (bool)plcInstance.Read("I0.5");

                S_4M1 = (bool)plcInstance.Read("Q0.0");
                S_4M2 = (bool)plcInstance.Read("Q0.1");
                S_4M3 = (bool)plcInstance.Read("Q0.2");
                S_4M4 = (bool)plcInstance.Read("Q0.3");

                D_4CO1 = (float)plcInstance.Read(DataType.DataBlock, 100, 0, VarType.Real, 1);
            }
        }

        public void WriteState(string variable, object value)
        {
            plcInstance.Write(variable, value);
        }

        public void WriteDB(int db, int startByteAdr, object value)
        {
            plcInstance.Write(DataType.DataBlock, db, startByteAdr, value);
        }
    }

}
