using Fusion;
using UnityEngine;


[System.Serializable]
public struct PlayerSpawnConfig
{
    [System.Serializable]
    public struct SpawnConfig
    {
        public NetworkPrefabRef Prefab;
        public Transform Position;
    }

    public SpawnConfig[] Configs; // index = PlayerType
}
