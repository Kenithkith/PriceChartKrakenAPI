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
        public List<List<string>> result { get; set; }
        public List<string> date { get; set; }
        public List<double> closePrice { get; set; }
        public List<string> error { get; set; }

    }
}
