using System;
using System.Collections.Generic;

namespace KraKenClient
{
    public interface Inf_client
    {
        List<string> M_giveListOfTradeAsset(Boolean bool_skipDarkPool);
    }
}
