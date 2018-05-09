using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Helpers;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;

namespace src
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // the linear chart in wpf
        public SeriesCollection liveChartLinear { get; set; }
        // the log chart in wpf
        public SeriesCollection liveChartLog { get; set; }
        // the combo chart in wpf
        public SeriesCollection liveChartCombo { get; set; }
        // a KraKen client implemented by our
        private KraKenClient.Inf_client Inf_KkClient;
        // the x-aixs value of the linear chart
        public string[] XValueLinear { get; set; }
        // the x-aixs and y-axis format of the linear chart
        public Func<double, string> YFormatLinear { get; set; }
        public Func<double, string> XFormatLinear { get; set; }
        // the x-aixs value of the log chart
        public string[] XValueLog { get; set; }
        // the x-aixs and y-axis format of the log chartg
        public Func<double, string> XFormatLog { get; set; }
        // the x-aixs value of the combo chart
        public string[] XValueCombo { get; set; }
        // the x-aixs and y-axis format of the combo chart
        public Func<double, string> YFormatCombo { get; set; }
        public Func<double, string> XFormatCombo { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            // create KraKen client instance
            this.Inf_KkClient = new KraKenClient.C_client(this);

            // dummy charts
            this.liveChartLinear = new SeriesCollection { };
            this.liveChartLog = new SeriesCollection { };
            this.liveChartCombo = new SeriesCollection { };

            // get the list of tradable asset on KraKen 
            List<string> listTradeAsset = this.Inf_KkClient.M_giveListOfTradeAsset(true);
            if(listTradeAsset == null)
            {
                this.M_changeStatusBarMsG("network connection fail.", Brushes.Red);
                return;
            }
            if (listTradeAsset.Count > 0)
            {
                this.cbx01AssetPairs.ItemsSource = listTradeAsset;
                this.cbx01AssetPairs.SelectedValue = this.cbx01AssetPairs.Items[0];
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // get the asset user selected
            string strPair = (this.cbx01AssetPairs.SelectedValue ?? String.Empty).ToString().Trim();
            if (strPair == null || String.IsNullOrWhiteSpace(strPair))
                return;

            // get the log-base user entered
            double dbLogBase;
            if(!Double.TryParse(tbx01LogBase.Text, out dbLogBase))
            {
                // display warming msg if case of need
                this.M_changeStatusBarMsG("Log base shoud be a number.", Brushes.Red);
                return;
            }

            // call KraKne server 
            ThreadPool.QueueUserWorkItem(o => { this.M_actionAfterButtonClick(strPair, dbLogBase); });

        }

        // call KraKne server and display the price chart according to the KraKen's jason file
        private void M_actionAfterButtonClick(string strPair, double dbLogBase)
        {
            try
            {
                // switch on the ProgressBar bar under a thread
                if (!Dispatcher.CheckAccess())
                    Dispatcher.BeginInvoke(new Action(() => this.pbar01.IsIndeterminate = true));
                else
                    this.pbar01.IsIndeterminate = true;

                // call KraKen API and convert its jason file to an object
                AnsycMethod ansycMethod = new AnsycMethod(this.M_callKraKenServer);
                IAsyncResult ansycResult = ansycMethod.BeginInvoke(strPair, dbLogBase, null, null);
                if (!Dispatcher.CheckAccess())
                    Dispatcher.BeginInvoke(new Action(() => M_changeStatusBarMsG("calling KraKen server...", Brushes.Black)));
                else
                    this.M_changeStatusBarMsG("calling KraKen server...", Brushes.Black);
                dynamic c_ohlc = ansycMethod.EndInvoke(ansycResult);

                // display error msg if calling the KraKen's server fail
                if (c_ohlc == null || c_ohlc.listError != null)
                {
                    if (!Dispatcher.CheckAccess())
                        Dispatcher.BeginInvoke(new Action(() => M_changeStatusBarMsG("network connection fail.", Brushes.Red)));
                    else
                        this.M_changeStatusBarMsG("network connection fail.", Brushes.Red);

                    Dispatcher.BeginInvoke(new Action(() => this.pbar01.IsIndeterminate = false));

                    return;
                }

                // clear the pervious chart
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    if (this.liveChartLinear != null && this.liveChartLinear.Count > 0)
                    {
                        foreach (Series chart in this.liveChartLinear)
                        { this.liveChartLinear.Remove(chart); }
                    }
                    if (this.liveChartLog.Chart != null && this.liveChartLog.Count > 0)
                    {
                        foreach (Series chart in this.liveChartLog)
                        { this.liveChartLog.Remove(chart); }
                    }
                    if (this.liveChartCombo.Chart != null && this.liveChartCombo.Count > 0)
                    {
                        foreach (Series chart in this.liveChartCombo)
                        { this.liveChartCombo.Remove(chart); }

                    }

                // display the price chart
                this.liveChartLinear.Add(new LineSeries()
                    {
                        Title = c_ohlc.strPair,
                        Values = ((List<double>)c_ohlc.listClosePrice).AsChartValues(),
                    });
                    this.liveChartLog.Add(new LineSeries()
                    {
                        Title = c_ohlc.strPair,
                        Values = ((List<double>)c_ohlc.listClosePriceInLog).AsChartValues(),
                    });
                    this.liveChartCombo.Add(new LineSeries()
                    {
                        Title = c_ohlc.strPair,
                        Values = ((List<double>)c_ohlc.listClosePrice).AsChartValues(),
                        ScalesYAt = 0
                    });
                    this.liveChartCombo.Add(new LineSeries()
                    {
                        Title = "log(price)",
                        Values = ((List<double>)c_ohlc.listClosePriceInLog).AsChartValues(),
                        ScalesYAt = 1
                    });
                    this.XValueLinear = this.XValueLog = this.XValueCombo = c_ohlc.listDate.ToArray();
                    this.OnPropertyChanged("XValueLinear");
                    this.OnPropertyChanged("XValueLog");
                    this.OnPropertyChanged("XValueCombo");
                    this.XFormatLinear = value => value.ToString();
                    this.XFormatLog = value => value.ToString();
                    this.XFormatCombo = value => value.ToString();

                    // update the chart
                    DataContext = this;

                    this.M_changeStatusBarMsG("done.", Brushes.Black);

                });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    this.M_changeStatusBarMsG(ex.ToString(), Brushes.Black);
                });
            }
            finally
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    // end the ProgressBar bar
                    this.pbar01.IsIndeterminate = false;
                });
            }

        }

        // to use anonymous method
        public delegate dynamic AnsycMethod(string strPair, double dbLogBase);

        private dynamic M_callKraKenServer(string strPair, double dbLogBase)
        {
            return this.Inf_KkClient.M_giveOhlc(strPair, dbLogBase);

        }

        // update the message on status bar
        private void M_changeStatusBarMsG(string strMsg, Brush color)
        {
            tbk01StausMsg.Text = strMsg;
            tbk01StausMsg.Foreground = color;
        }

        // a event handler
        public event PropertyChangedEventHandler PropertyChanged;

        // event to update the wpf component's property
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
