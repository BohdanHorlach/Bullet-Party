using Fusion;
using UnityEngine;


namespace General.Match.PlayerPlacement
{
    public class SpawnCoordinator : NetworkBehaviour
    {
        [SerializeField] private BaseSpawnPointAssigner _spawnPointAssigner;
        [SerializeField] protected Transform _trackingTarget = null;

        public static SpawnCoordinator Instance;


        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
        }


        private void AssignPlayers()
        {
            foreach (var player in Runner.ActivePlayers)
            {
                _spawnPointAssigner.SetPlace(player);
            }
        }


        private void AttachTracking()
        {
            foreach (var player in Runner.ActivePlayers)
            {
                if (player != null && player == Runner.LocalPlayer)
                {
                    NetworkObject localPlayer = Runner.GetPlayerObject(player);
                    _trackingTarget.SetParent(localPlayer.transform, false);
                }
            }
        }


        public override void Spawned()
        {
            base.Spawned();
            AttachTracking();

            if (!Object.HasStateAuthority)
                return;

            AssignPlayers();
        }
    }
}