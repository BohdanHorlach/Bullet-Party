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


        private void Awake()
        {
            DontDestroyOnLoad(this);
        }


        private void OnEnable()
        {
            VictoryNotifier.OnVictory += OnLevelComplete;
        }


        private void OnDisable()
        {
            VictoryNotifier.OnVictory -= OnLevelComplete;
        }


        private void LoadLobby()
        {
            RPC_GoToLobby();
        }


        private void OnLevelComplete(Dictionary<PlayerRef, int> rateInfo)
        {
            Invoke(nameof(LoadLobby), _pauseTime);
        }



        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public async void RPC_GoToLobby()
        {
            await NetworkBootstrap.Instance.SetNextScene(_lobbySceneIndex);
        }
    }
}