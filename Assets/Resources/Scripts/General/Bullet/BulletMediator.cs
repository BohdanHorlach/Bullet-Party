using Fusion;
using System;
using UnityEngine;


namespace General.Bullet
{
    public class BulletMediator : MonoBehaviour
    {
        [SerializeField] private BulletMovement _movement;
        [SerializeField] private BulletCollisionHandler _collisionHandler;

        private BulletSpawner _spawner;

        public static Action<PlayerRef, PlayerRef> OnHitTarget;

        private void OnEnable()
        {
            _collisionHandler.OnHit += ReleaseBullet;
        }


        private void OnDisable()
        {
            _collisionHandler.OnHit -= ReleaseBullet;
        }


        private void ReleaseBullet(PlayerRef? targetRef)
        {
            if(_spawner != null)
                _spawner.Release(_collisionHandler.Object);

            if (targetRef.HasValue == false)
                return;

            PlayerRef owner = _spawner != null ? _spawner.Object.InputAuthority : PlayerRef.None;
            OnHitTarget?.Invoke(targetRef.Value, owner);
        }


        public void Initialize(BulletSpawner spawner, Vector3 position, Vector3 direction)
        {
            _spawner = spawner;
            _movement.Initialize(position, direction);
            _collisionHandler.Inialize(spawner.Object.Id, spawner.BulletType);
        }
    }
}