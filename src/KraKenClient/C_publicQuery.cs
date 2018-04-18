using System;
using KraKenClient;
using System.Net;
using System.IO;
using Jayrock.Json;
using System.Collections.Generic;
using Jayrock.Json.Conversion;

public class C_publicQuery
{
    public C_publicQuery()
    {
    }

    //Get a public list of tradable asset pairs
    public JsonObject M_getActiveAsset(List<string> pairs)
    {
        return (JsonObject)QueryPublic("Assets");

    }

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

        //Make the request
        try
        {
            //Wait for RateGate
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
        catch (WebException wex)
        {
            using (HttpWebResponse response = (HttpWebResponse)wex.Response)
            {
                using (Stream str = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(str))
                    {
                        if (response.StatusCode != HttpStatusCode.InternalServerError)
                        {
                            throw;
                        }
                        return (JsonObject)JsonConvert.Import(sr);
                    }
                }
            }

        }
    }


}