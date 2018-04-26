using System;
using System.Collections.Generic;

namespace src.KraKenClient
{
    public interface Inf_client
    {
        List<string> M_giveListOfTradeAsset(Boolean bool_skipDarkPool);

        Model.C_Ohlc M_giveOhlc(string strPair, double dbBase);
    }


}
