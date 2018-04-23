using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace src.KraKenClient
{
    public class C_client : Inf_client
    {
        private C_publicQuery C_publicQ = new C_publicQuery();

        public List<string> M_giveListOfTradeAsset(Boolean bool_skipDarkPool)
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

        public Model.C_Ohlc M_giveOhlc(string strPair)
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
                C_ohlc.error = jTokenBuffer["error"].ToObject<List<string>>();
                return C_ohlc;
            }

            //  json to object 2/2
            C_ohlc.strPair = jTokenBuffer["result"].First.Path.ToString().Replace("result.", String.Empty);
            C_ohlc.result = jTokenBuffer["result"][C_ohlc.strPair].ToObject<List<List<string>>>();

            // let date and price to be a List
            List<string> dateTime = new List<string>();
            List<double> closePrice = new List<double>();
            for (int index = 0; index < C_ohlc.result.Count; index++)
            {
                dateTime.Add(C_struct.UnixTimeStampToDateTime(Double.Parse(C_ohlc.result[index][0])).ToString("ddMMMyyyy"));
                closePrice.Add(Double.Parse(C_ohlc.result[index][4]));
            }
            C_ohlc.date = dateTime;
            C_ohlc.closePrice = closePrice;

            return C_ohlc;
        }



    }

}
