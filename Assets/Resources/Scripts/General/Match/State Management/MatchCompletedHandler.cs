using Fusion;
using General.MiniGames;
using General.Player;
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


        private void CalculateRating(Dictionary<PlayerType, int> rateInfo)
        {
            if (!_runner.IsSceneAuthority)
                return;

            Dictionary<PlayerType, int> normalized =
                PlayerScoreCalculator.Normalize(rateInfo);

            foreach (var score in normalized)
            {
                MatchRanking.Instance.AddScore(
                    score.Key,
                    score.Value
                );
            }
        }


        private void OnLevelComplete(Dictionary<PlayerType, int> rateInfo)
        {
            CalculateRating(rateInfo);
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