using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Helpers;


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
        private KraKenClient.Inf_client Inf_KkClient = new KraKenClient.C_client();
        // the x-aixs value of the linear chart
        public string[] XValueLinear { get; set; }
        // the x-aixs and y-axis format of the linear chart
        public Func<double, string> YFormatLinear { get; set; }
        public Func<double, string> XFormatLinear { get; set; }
        // the x-aixs value of the log chart
        public string[] XValueLog { get; set; }
        // the x-aixs and y-axis format of the log chart
        public Func<double, string> YFormatLog { get; set; }
        public Func<double, string> XFormatLog { get; set; }
        // the x-aixs value of the combo chart
        public string[] XValueCombo { get; set; }
        // the x-aixs and y-axis format of the combo chart
        public Func<double, string> YFormatCombo { get; set; }
        public Func<double, string> XFormatCombo { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            // get the list of tradable asset on KraKen 
            List<string> listTradeAsset = this.Inf_KkClient.M_giveListOfTradeAsset(true);
            if (listTradeAsset.Count > 0)
            {
                this.cbx01AssetPairs.ItemsSource = listTradeAsset;
                this.cbx01AssetPairs.SelectedValue = this.cbx01AssetPairs.Items[0];
            }
    
            // dummy charts
            this.liveChartLinear = new SeriesCollection { };
            this.liveChartLog = new SeriesCollection { };
            this.liveChartCombo= new SeriesCollection { };

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // get the asset user selected
            string strPair = this.cbx01AssetPairs.SelectedValue.ToString().Trim();

            // get log base user entered
            if (!this.M_isLogBaseANum())
            {
                tbk01StausMsg.Text = "Log base shoud be a number.";
                tbk01StausMsg.Foreground = Brushes.Red;
                return;
            }
            else
            {
                tbk01StausMsg.Text = "Loading...";
                tbk01StausMsg.Foreground = Brushes.Black;
            }
            double dbLogBase = Double.Parse(tbx01LogBase.Text);

            // call KraKen API and convert its jason file to an object
            dynamic c_ohlc;
            do
            {
                System.Threading.Thread.Sleep(5000);
                c_ohlc = this.Inf_KkClient.M_giveOhlc(strPair, dbLogBase);

            } while (c_ohlc.listError != null);

            // clear the pervious chart
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
            this.OnPropertyChanged( "XValueLinear");
            this.OnPropertyChanged("XValueLog");
            this.OnPropertyChanged("XValueCombo");
            this.XFormatLinear = value => value.ToString();
            this.XFormatLog = value => value.ToString();
            this.XFormatCombo = value => value.ToString();

            // update the chart
            DataContext = this;
        }

        // make sure the log base is real number
        private Boolean M_isLogBaseANum()
        {
            var varUserInput = tbx01LogBase.Text;
            double dbTemp;
            if(!Double.TryParse(varUserInput, out dbTemp))
                return false;

            return true;
        }

        // a event handler
        public event PropertyChangedEventHandler PropertyChanged;

        // event to update the wpf component's pgroperty
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
