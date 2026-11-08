using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

namespace General.Match.PlayerPlacement
{
    public class RandomSpawnPointAssigner : BaseSpawnPointAssigner
    {
        [SerializeField] private Transform[] _spawnPoints;

        [Networked] public int LastPointIndex { get; private set; }


        private void Awake()
        {
            ShuffleArray(_spawnPoints);
        }


        private void ShuffleArray<T>(T[] array)
        {
            for (int i = _spawnPoints.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                T temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }


        public override void SetPlace(PlayerRef playerRef)
        {
            NetworkObject playerObj = Runner.GetPlayerObject(playerRef);

            if (playerObj == null)
                return;

            Transform spawnPoint = _spawnPoints[LastPointIndex];
            var netRb = playerObj.GetComponent<NetworkRigidbody2D>();
            if (netRb != null)
                netRb.Teleport(spawnPoint.position);

            LastPointIndex++;
        }
    }
}