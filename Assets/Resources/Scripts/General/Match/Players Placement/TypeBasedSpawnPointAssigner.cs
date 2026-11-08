using Fusion;
using Fusion.Addons.Physics;
using General.Player;
using General.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace General.Match.PlayerPlacement
{
    [System.Serializable]
    public struct SpawnPlaceByType
    {
        public PlayerType PlayerType;
        public Transform Position;
    }


    public class TypeBasedSpawnPointAssigner : BaseSpawnPointAssigner
    {
        [SerializeField] private SpawnPlaceByType[] _spawnPlaceByTypes;

        private Dictionary<PlayerType, Transform> _spawnPosition;


        private void Awake()
        {
            _spawnPosition = DictionaryInitializer.InstantiateDictionary(
                _spawnPlaceByTypes,
                (place) => place.PlayerType,
                (place) => place.Position
            );
        }


        private PlayerType GetPlayerType(NetworkObject playerObj)
        {
            PlayerAvatar player = playerObj.GetComponent<PlayerAvatar>();

            return player != null ? player.PlayerType : PlayerType.Other;
        }


        public override void SetPlace(PlayerRef playerRef)
        {
            NetworkObject playerObj = Runner.GetPlayerObject(playerRef);
            
            if (playerObj == null) 
                return;

            PlayerType type = GetPlayerType(playerObj);
            Vector3 spawnPoint = _spawnPosition[type].position;

            var netRb = playerObj.GetComponent<NetworkRigidbody2D>();
            if (netRb != null)
                netRb.Teleport(spawnPoint);
        }
    }
}
