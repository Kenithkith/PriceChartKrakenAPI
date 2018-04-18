using System;

namespace KraKenClient
{
    public class C_client : Inf_client
    {
        private C_publicQuery C_publicQ;


        public C_client()
        {
            this.C_publicQ = new C_publicQuery();
        }

        public List<string> M_getTradeAsset()
        {

            // get json from Kraken
            var jsonFromKK = client.GetAssetPairs(null);

            // json to object
            var desJsonFromKK = JsonConvert.DeserializeObject(jsonFromKK.ToString());
            JToken jTokenBuffer = JObject.Parse(desJsonFromKK.ToString());
            JObject jObjectBuffer = jTokenBuffer["result"].Value<JObject>();

            // get a particular item
            List<string> listOfPair = jObjectBuffer.Properties().Select(p => p.Name).ToList();
            List<string> listOfAltname = new List<string>();
            foreach (string strPair in listOfPair)
            {
                if (skipDarkPool && strPair.Contains(".d"))
                    continue;

                listOfAltname.Add(jTokenBuffer["result"][strPair]["altname"].ToObject<string>());
            }

            return listOfAltname;

        }
    }
}
