using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Model
{
    public class C_Ohlc
    {
        public int intInterval { get; set; }
        public long longSince { get; set; }
        public string strPair { get; set; }
        public List<List<string>> listOfListResult { get; set; }
        public List<string> listDate { get; set; }
        public List<double> listClosePrice { get; set; }
        public List<double> listClosePriceInLog { get; set; }
        public List<string> listError { get; set; }

        public void M_convertClosePriceToLog(double dbBase)
        {
            if (this.listClosePrice == null || this.listClosePrice.Count == 0)
                return;

            List<double> listOfPrice = new List<double>(listClosePrice);
            this.listClosePriceInLog = new List<double>();
            foreach(double dbNum in listOfPrice)
            {
                this.listClosePriceInLog.Add(Math.Log(dbNum, dbBase));
            }
        }

    }
}
