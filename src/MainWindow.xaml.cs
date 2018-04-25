using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Helpers;
using System;

namespace src
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // the linear chart in wpf
        public SeriesCollection liveChartLinear { get; set; }
        // the log chart in wpf
        public SeriesCollection liveChartLog { get; set; }
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

            // two dummy chart
            liveChartLinear = new SeriesCollection
            { 
                new LineSeries
                {
                    Values = new ChartValues<double> { 0 }
                },
            };
            liveChartLog = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<double> { 0 }
                },
            };

            // update the chart
            DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // get the asset user selected
            string strPair = this.cbx01AssetPairs.SelectedValue.ToString().Trim();

            // call KraKen API and convert its jason file to an object
            dynamic c_ohlc;
            do
            {
                System.Threading.Thread.Sleep(5000);
                c_ohlc = this.Inf_KkClient.M_giveOhlc(strPair);

            } while (c_ohlc.error != null);

            // clear the pervious chart
            if (this.liveChartLinear.Chart != null && this.liveChartLinear.Count > 0)
            {
                foreach (Series chart in this.liveChartLinear)
                { this.liveChartLinear.Remove(chart); }
            }
            if (this.liveChartLog.Chart != null && this.liveChartLog.Count > 0)
            {
                foreach (Series chart in this.liveChartLog)
                { this.liveChartLog.Remove(chart); }
            }

            // display the price chart
            this.liveChartLinear.Add(new LineSeries()
            {
                Title = c_ohlc.strPair,
                Values = ((List<double>)c_ohlc.closePrice).AsChartValues(),
            });
            this.XValueLinear = c_ohlc.date.ToArray();
            this.OnPropertyChanged("XValueLinear");
            this.XFormatLinear = value => value.ToString();

            // update the chart
            DataContext = this;
        }

        // a event handler
        public event PropertyChangedEventHandler PropertyChanged;

        // event to update the wpf component's Property
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
