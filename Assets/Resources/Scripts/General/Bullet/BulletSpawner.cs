using Fusion;
using General.Match.StateManagement;
using General.Player;
using Network;
using UnityEngine;


namespace General.Bullet
{
    /// <summary>Players fire via input + RPC; bots / scene turrets set <see cref="_isBot"/> and fire on state authority only.</summary>
    public class BulletSpawner : NetworkBehaviour
    {
        [SerializeField] private Transform _player;
        [SerializeField] private NetworkObjectPool _bulletPool;
        [SerializeField] private PlayerType _bulletType;
        [SerializeField] private bool _isBot = false;


        public PlayerType BulletType => _bulletType;

        public bool IsReadyForFire =>
            Object != null
            && Object.IsValid
            && Runner != null
            && Runner.IsRunning
            && _bulletPool != null
            && _bulletPool.IsSpawned;


        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_ActivateBullet(int bulletIndex, Vector3 position, Vector2 direction)
        {
            NetworkObject networkObject = _bulletPool.Get(bulletIndex);
            BulletMediator bullet = networkObject.GetComponent<BulletMediator>();
            bullet.Initialize(this, position, direction);
        }


        private void SpawnBullet(Vector3 position, Vector2 direction)
        {
            if (!HasStateAuthority || !IsReadyForFire)
                return;

            int bulletIndex = _bulletPool.FindInactiveIndex();
            RPC_ActivateBullet(bulletIndex, position, direction);
        }


        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RPC_SpawnBullet(PlayerType type, Vector3 position, Vector2 direction)
        {
            if (type != _bulletType)
                return;

            SpawnBullet(position, direction);
        }


        public void Fire(Vector2 fireDirection)
        {
            if (GamePause.IsPaused || fireDirection == Vector2.zero || !IsReadyForFire)
                return;

            Vector3 origin = _player != null ? _player.position : transform.position;
            Vector2 direction = fireDirection.normalized;

            if (_isBot)
            {
                SpawnBullet(origin, direction);
                return;
            }

            if (HasInputAuthority)
                RPC_SpawnBullet(_bulletType, origin, direction);
        }


        public void Release(NetworkObject bullet)
        {
            _bulletPool.Release(bullet);
        }
    }
}