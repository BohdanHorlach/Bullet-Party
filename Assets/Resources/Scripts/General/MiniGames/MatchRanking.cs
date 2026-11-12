using Fusion;
using General.Player;
using UnityEngine;


namespace General.MiniGames
{

    public class MatchRanking : NetworkBehaviour
    {
        [Networked, Capacity(4)]
        public NetworkDictionary<PlayerType, int> Ranking { get; }

        public static MatchRanking Instance { get; private set; }

        
        public override void Spawned()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }


        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (Instance == this)
                Instance = null;
        }


        public void AddScore(PlayerType player, int score)
        {
            if (!HasStateAuthority)
                return;

            int currentScore = Ranking.ContainsKey(player)
                ? Ranking.Get(player)
                : 0;

            if (Ranking.ContainsKey(player))
                Ranking.Set(player, currentScore + score);
            else
                Ranking.Add(player, score);
        }

        
        public int GetScore(PlayerType player)
        {
            return Ranking.ContainsKey(player)
                ? Ranking.Get(player)
                : 0;
        }
    }
}