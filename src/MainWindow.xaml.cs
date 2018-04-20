using System.Collections.Generic;
using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;

namespace src
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SeriesCollection liveChartLinear { get; set; }
        public SeriesCollection liveChartLog { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            // get the list of tradable asset on KraKen 
            KraKenClient.Inf_client Inf_KkClient = new KraKenClient.C_client();
            List<string> listTradeAsset = Inf_KkClient.M_giveListOfTradeAsset(true);
            if (listTradeAsset.Count > 0)
            {
                this.cbx01AssetPairs.ItemsSource = listTradeAsset;
                this.cbx01AssetPairs.SelectedValue = this.cbx01AssetPairs.Items[0];
            }

            liveChartLinear = new SeriesCollection
            { 
                new LineSeries
                {
                    Values = new ChartValues<double> { 0 }
                },
                new ColumnSeries
                {
                    Values = new ChartValues<decimal> { 0 }
                }
            };

            liveChartLog = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<double> { 0 }
                },
                new ColumnSeries
                {
                    Values = new ChartValues<decimal> { 0 }
                }
            };
        }
    }
}
