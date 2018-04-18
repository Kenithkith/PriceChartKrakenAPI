using System;

public static class C_config
{

    public string strUrl = ConfigurationManager.AppSettings["KrakenBaseAddress"];
    public int strVersion = int.Parse(ConfigurationManager.AppSettings["KrakenApiVersion"]);
    public string strKey = ConfigurationManager.AppSettings["KrakenKey"];
    public string strSecret = ConfigurationManager.AppSettings["KrakenSecret"];
    //RateGate was taken from http://www.jackleitch.net/2010/10/better-rate-limiting-with-dot-net/
    public RateGate rgRateGate = new RateGate(1, TimeSpan.FromSeconds(5));
}
