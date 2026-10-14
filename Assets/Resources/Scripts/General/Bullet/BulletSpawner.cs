using Fusion;
using General.Player;
using Network;
using UnityEngine;


namespace General.Bullet
{
    public class BulletSpawner : NetworkBehaviour
    {
        [SerializeField] private Transform _player;
        [SerializeField] private NetworkObjectPool _bulletPool;
        [SerializeField] private PlayerType _bulletType;



        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_ActivateBullet(int bulletIndex, Vector3 position, Vector2 direction)
        {
            NetworkObject networkObject = _bulletPool.Get(bulletIndex);
            BulletMediator bullet = networkObject.GetComponent<BulletMediator>();
            Debug.Log(position);
            bullet.Initialize(this, position, direction);
        }


        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RPC_SpawnBullet(PlayerType type, Vector3 position, Vector2 direction)
        {
            if (type != _bulletType)
                return;

            Debug.Log(position);

            int bulletIndex = _bulletPool.FindInactiveIndex();
            RPC_ActivateBullet(bulletIndex, position, direction);
        }


        public void Fire(Vector2 fireDirection)
        {
            if (fireDirection == Vector2.zero)
                return;
            
            if (Object.HasInputAuthority)
                RPC_SpawnBullet(_bulletType, _player.position, fireDirection.normalized);
        }


        public void Release(NetworkObject bullet)
        {
            _bulletPool.Release(bullet);
        }
    }
}