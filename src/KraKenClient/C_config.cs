using JackLeitch.RateGate;
using System;
using System.Configuration;

public static class C_config
{
    public static string strUrl = ConfigurationManager.AppSettings["KrakenBaseAddress"];
    public static int strVersion = int.Parse(ConfigurationManager.AppSettings["KrakenApiVersion"]);
    public static string strKey = ConfigurationManager.AppSettings["KrakenKey"];
    public static string strSecret = ConfigurationManager.AppSettings["KrakenSecret"];
    //RateGate was taken from http://www.jackleitch.net/2010/10/better-rate-limiting-with-dot-net/
    public static RateGate rgRateGate = new RateGate(1, TimeSpan.FromSeconds(5));
}
