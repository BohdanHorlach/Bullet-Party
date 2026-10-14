using Fusion;
using System;
using System.Collections.Generic;


namespace General.Match.StateManagement
{
    public class ReadyManager : NetworkBehaviour
    {
        private readonly HashSet<PlayerRef> _readyPlayers = new HashSet<PlayerRef>();

        public event Action OnAllPlayersReady;
        public static ReadyManager Instance { get; private set; }


        private void Awake()
        {
            Instance = this;
        }


        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }


        public void SetReady(PlayerRef player)
        {
            if (!_readyPlayers.Add(player))
                return;

            CheckAllReady();
        }


        public void RemovePlayer(PlayerRef player)
        {
            _readyPlayers.Remove(player);
        }


        public void ResetReadyState()
        {
            _readyPlayers.Clear();
        }


        private void CheckAllReady()
        {
            foreach (PlayerRef player in Runner.ActivePlayers)
            {
                if (!_readyPlayers.Contains(player))
                    return;
            }

            OnAllPlayersReady?.Invoke();
        }
    }
}