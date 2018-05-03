using System.Net;
using System.IO;
using Jayrock.Json;
using Jayrock.Json.Conversion;

public class C_publicQuery
{
    // Get list of tradable asset pairs
    public JsonObject M_getActiveAsset()
    {
        return (JsonObject)QueryPublic("AssetPairs");
    }

    // Get Ohlc data of a particular asset
    public JsonObject M_getOhlc(string strPair, int intInterval, long lgSince)
    {
        string strParameter = string.Format("pair={0}", strPair) 
                            + string.Format("&interval={0}", intInterval.ToString())
                            + string.Format("&since={0}", lgSince.ToString());

        return (JsonObject)QueryPublic("OHLC", strParameter);
    }

    // call KraKen API
    private JsonObject QueryPublic(string strMethod, string strOtherProp = null)
    {
        string strAddress = string.Format("{0}/{1}/public/{2}", C_config.strUrl, C_config.strVersion, strMethod);
        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(strAddress);
        webRequest.ContentType = "application/x-www-form-urlencoded";
        webRequest.Method = "POST";

        if (strOtherProp != null)
        {
            using (var writer = new StreamWriter(webRequest.GetRequestStream()))
            {
                writer.Write(strOtherProp);
            }
        }

        C_config.rgRateGate.WaitToProceed();
        using (WebResponse webResponse = webRequest.GetResponse())
        {
            using (Stream str = webResponse.GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(str))
                {
                    return (JsonObject)JsonConvert.Import(sr);
                }
            }
        }
    }
}