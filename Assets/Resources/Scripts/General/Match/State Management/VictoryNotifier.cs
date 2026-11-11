using General.Player;
using System;
using System.Collections.Generic;


namespace General.Match.StateManagement
{
    public static class VictoryNotifier
    {
        public static Action<Dictionary<PlayerType, int>> OnVictory;


        public static void Notify(Dictionary<PlayerType, int> rateInfo)
        {
            OnVictory?.Invoke(rateInfo);
        }
    }
}