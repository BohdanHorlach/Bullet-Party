using UnityEngine;


namespace General.Bullet
{
    public class BulletMediator : MonoBehaviour
    {
        [SerializeField] private BulletMovement _movement;
        [SerializeField] private BulletCollisionHandler _collisionHandler;

        private BulletSpawner _spawner;


        private void OnEnable()
        {
            _collisionHandler.OnHit += ReleaseBullet;
        }


        private void OnDisable()
        {
            _collisionHandler.OnHit -= ReleaseBullet;
        }


        private void ReleaseBullet()
        {
            if(_spawner != null)
                _spawner.Release(_collisionHandler.Object);
        }


        public void Initialize(BulletSpawner spawner, Vector3 position, Vector3 direction)
        {
            _spawner = spawner;
            _movement.Initialize(position, direction);
            _collisionHandler.Inialize(spawner.Object.Id);
        }
    }
}