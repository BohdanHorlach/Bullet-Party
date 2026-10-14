using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Network
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private PlayerSpawnConfig _spawnConfig;

        private readonly List<PlayerRef> _joinOrder = new();
        private Dictionary<PlayerRef, NetworkObject> _spawned = new Dictionary<PlayerRef, NetworkObject>();

        public static Action<Transform> OnPlayerSpawn;
        public static PlayerSpawner Instance;


        private void Awake()
        {
            if(Instance == null)
                Instance = this;
        }


        private (NetworkPrefabRef, Transform) GetPrefabData(PlayerRef player)
        {
            _joinOrder.Add(player);
            int index = _joinOrder.Count - 1;

            if (index >= _spawnConfig.Configs.Length)
                throw new Exception("Too many players connected");

            NetworkPrefabRef prefab = _spawnConfig.Configs[index].Prefab;
            Transform spawnPoint = _spawnConfig.Configs[index].Position;

            return (prefab, spawnPoint);
        }


        public NetworkObject Spawn(NetworkRunner runner, PlayerRef player)
        {
            if (!runner.IsServer)
                return null;

            (NetworkPrefabRef prefab, Transform spawnPoint) = GetPrefabData(player);

            NetworkObject networkPlayerObject = runner.Spawn(
                prefab, 
                Vector3.zero, 
                Quaternion.identity, 
                player,
                (runner, player) => { 
                    player.transform.SetParent(this.transform);
                    player.transform.position = spawnPoint.position;
                }
            );
            _spawned.Add(player, networkPlayerObject);
            runner.SetPlayerObject(player, networkPlayerObject);

            return networkPlayerObject;
        }


        public void Despawn(NetworkRunner runner, PlayerRef player)
        {
            if (!runner.IsServer)
                return;

            _joinOrder.Remove(player);

            if (!_spawned.TryGetValue(player, out NetworkObject networkObject))
                return;

            runner.Despawn(networkObject);
            _spawned.Remove(player);
        }
    }
}