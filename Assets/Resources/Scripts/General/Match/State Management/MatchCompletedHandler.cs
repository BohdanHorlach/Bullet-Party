using Fusion;
using Network;
using System.Collections.Generic;
using UnityEngine;


namespace General.Match.StateManagement
{
    public class MatchCompletedHandler : MonoBehaviour
    {
        [SerializeField] private float _pauseTime = 2f;
        [SerializeField] private int _lobbySceneIndex;
        private NetworkRunner _runner;


        private void OnEnable()
        {
            if(_runner != null)
                VictoryNotifier.OnVictory -= OnLevelComplete;
        }


        private void OnDisable()
        {
            if (_runner != null)
                VictoryNotifier.OnVictory -= OnLevelComplete;
        }


        private async void LoadLobby()
        {
            if (_runner.IsSceneAuthority)
            {
                await NetworkBootstrap.Instance.SetNextScene(_lobbySceneIndex);
            }
        }


        private void OnLevelComplete(Dictionary<PlayerRef, int> rateInfo)
        {
            Invoke(nameof(LoadLobby), _pauseTime);
        }


        public void Initialize(NetworkRunner runner)
        {
            _runner = runner;
            VictoryNotifier.OnVictory += OnLevelComplete;
        }


        public void ResetRunner()
        {
            _runner = null;
            VictoryNotifier.OnVictory -= OnLevelComplete;
        }
    }
}