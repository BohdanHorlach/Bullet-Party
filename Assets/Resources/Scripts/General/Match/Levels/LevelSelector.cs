using Fusion;
using General.Match.StateManagement;
using Network;
using UnityEngine;


namespace General.Match.Levels
{
    public class LevelSelector : NetworkBehaviour
    {
        [SerializeField] private int[] _levelIndexes;


        public override void Spawned()
        {
            if (!Runner.IsServer)
            {
                gameObject.SetActive(false);
                return;
            }

            ReadyManager.Instance.OnAllPlayersReady += LoadRandomLevel;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (ReadyManager.Instance != null)
                ReadyManager.Instance.OnAllPlayersReady -= LoadRandomLevel;
        }


        public async void LoadRandomLevel()
        {
            int randomLevelIndex = Random.Range(0, _levelIndexes.Length);
            int levelIndex = _levelIndexes[randomLevelIndex];
            Debug.Log($"load level with {levelIndex} index");
            await NetworkBootstrap.Instance.SetNextScene(levelIndex);
        }
    }
}