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

        bool ConveyorState = false;
        bool SeperatorState = false;
        bool BottlingState = false;

        string SeperatorTriggerAddress = "M0.0";

        public MainWindow()
        {
            InitializeComponent();

            // Get ip address
            ConnectButton.Click += ConnectButtonClicked;
            ConveyorButton.Click += ConveyorButtonClicked;
            SeperatorButton.Click += SeperatorButtonClicked;
            BottlingButton.Click += BottlingButtonClicked;

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
                handler = new PlcHandler(address, false);
                isConnected = handler.isConnected;
            }
            else
            {
                handler.Close();
                isConnected = false;
            }
        }

        public void ConveyorButtonClicked(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                handler.WriteState("Q0.2", !ConveyorState);
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


            }
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

            ConveyorButton.Background = handler.S_4M3 ? Brushes.Green : Brushes.Red;
            SeperatorButton.Background = handler.S_4M4 ? Brushes.Green : Brushes.Red;
            BottlingButton.Background = handler.S_4M2 ? Brushes.Green : Brushes.Red;
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
            }
        }

        public void WriteState(string variable, object value)
        {
            plcInstance.Write(variable, value);
        }
    }

}
