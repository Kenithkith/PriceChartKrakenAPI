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

namespace src
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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

        }
    }
}
