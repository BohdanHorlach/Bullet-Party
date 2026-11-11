using System.Collections;
using General.Bullet;
using General.Match.StateManagement;
using General.Utils;
using UnityEngine;


namespace General.Combat
{
    public class FireHandler : MonoBehaviour
    {
        [SerializeField] private BulletSpawner _bulletSpawner;
        [SerializeField] private float _fireRate = 0.2f;

        protected Vector2 _currentDirection = Vector2.zero;
        private Coroutine _fireRoutine;


        protected virtual void OnEnable()
        {
            InputReader.OnFire += HandleFireInput;
        }


        protected virtual void OnDisable()
        {
            InputReader.OnFire -= HandleFireInput;
        }


        private IEnumerator FireLoop()
        {
            while (_currentDirection != Vector2.zero)
            {
                if (!GamePause.IsPaused && _bulletSpawner != null)
                    _bulletSpawner.Fire(_currentDirection);

                yield return new WaitForSeconds(_fireRate);
            }

            _fireRoutine = null;
        }


        protected void StopFiring()
        {
            if (_fireRoutine != null)
            {
                StopCoroutine(_fireRoutine);
                _fireRoutine = null;
            }
        }


        protected void StartFiring(Vector2 direction)
        {
            _currentDirection = direction;

            if (_fireRoutine == null)
                _fireRoutine = StartCoroutine(FireLoop());
        }


        private void HandleFireInput(Vector2 direction)
        {
            if (direction == Vector2.zero)
            {
                _currentDirection = Vector2.zero;
                StopFiring();
                return;
            }

            StartFiring(direction);
        }
    }
}
