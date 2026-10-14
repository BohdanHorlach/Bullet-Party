using Fusion;
using System;
using System.Collections.Generic;


namespace General.Match.StateManagement
{
    public static class VictoryNotifier
    {
        public static Action<Dictionary<PlayerRef, int>> OnVictory;


        public static void Notify(Dictionary<PlayerRef, int> rateInfo)
        {
            OnVictory?.Invoke(rateInfo);
        }
    }
}