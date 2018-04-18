using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace KraKenClient
{
    public class C_client : Inf_client
    {
        private C_publicQuery C_publicQ;


        public C_client()
        {
            this.C_publicQ = new C_publicQuery();
        }

        public List<string> M_getTradeAsset(Boolean bool_skipDarkPool)
        {

            // get json from Kraken
            var jsonFromKK = this.C_publicQ.M_getActiveAsset(null);

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
    }
}
