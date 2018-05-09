using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Media;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace src.KraKenClient
{
    public class C_client : Inf_client
    {
        private C_publicQuery C_publicQ = new C_publicQuery();
        private MainWindow C_appUI;

        public C_client(MainWindow C_appUI)
        {
            this.C_appUI = C_appUI;
        }

        public List<string> M_giveListOfTradeAsset(Boolean bool_skipDarkPool)
        {
            try
            {
                // get json from Kraken
                var jsonFromKK = this.C_publicQ.M_getActiveAsset();

                // json to object
                var desJsonFromKK = JsonConvert.DeserializeObject(jsonFromKK.ToString());
                JToken jTokenBuffer = JObject.Parse(desJsonFromKK.ToString());
                JObject jObjectBuffer = jTokenBuffer["result"].Value<JObject>();

                // get a particular item
                List<string> listOfPair = jObjectBuffer.Properties().Select(p => p.Name).ToList();
                List<string> listOfAltname = new List<string>();
                foreach (string strPair in listOfPair)
                {
                    if (bool_skipDarkPool && strPair.Contains(".d"))
                        continue;

                    listOfAltname.Add(jTokenBuffer["result"][strPair]["altname"].ToObject<string>());
                }

                return listOfAltname;

            }
            catch (WebException wex)
            {
                return null;
            }
        }

        public Model.C_Ohlc M_giveOhlc(string strPair, double dbBase)
        {
            try
            {
                // create a instance to represent Ohlc 
                Model.C_Ohlc C_ohlc = new Model.C_Ohlc();
                C_ohlc.strPair = strPair;
                C_ohlc.intInterval = 1440;
                C_ohlc.longSince = 0;

                // get json from Kraken
                var jsonFromKK = this.C_publicQ.M_getOhlc(strPair, 1440, 0);

                // json to object 1/2
                var desJsonFromKK = JsonConvert.DeserializeObject(jsonFromKK.ToString());
                JToken jTokenBuffer = JObject.Parse(desJsonFromKK.ToString());

                // any error
                if (jTokenBuffer["error"].ToObject<List<string>>().Count != 0)
                {
                    C_ohlc.listError = jTokenBuffer["error"].ToObject<List<string>>();
                    return C_ohlc;
                }

                //  json to object 2/2
                C_ohlc.strPair = jTokenBuffer["result"].First.Path.ToString().Replace("result.", String.Empty);
                C_ohlc.listOfListResult = jTokenBuffer["result"][C_ohlc.strPair].ToObject<List<List<string>>>();

                // let date and price to be a List
                List<string> dateTime = new List<string>();
                List<double> closePrice = new List<double>();
                for (int index = 0; index < C_ohlc.listOfListResult.Count; index++)
                {
                    dateTime.Add(C_struct.UnixTimeStampToDateTime(Double.Parse(C_ohlc.listOfListResult[index][0])).ToString("ddMMMyyyy"));
                    closePrice.Add(Double.Parse(C_ohlc.listOfListResult[index][4]));
                }

                C_ohlc.listDate = dateTime;
                C_ohlc.listClosePrice = closePrice;
                C_ohlc.M_convertClosePriceToLog(dbBase);

                return C_ohlc;
            }
            catch (WebException wex)
            {
                return null;
            }
        }

    }

}
