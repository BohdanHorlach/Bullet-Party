using System.Collections;
using General.Combat;
using UnityEngine;

namespace General.Bots
{
    public class AutoFire : FireHandler
    {
        [SerializeField] private Vector3 _autoRotate;

        protected override void OnEnable()
        {
            StartCoroutine(StartWhenNetworkReady());
        }

        protected override void OnDisable()
        {
            StopFiring();
        }

        private IEnumerator StartWhenNetworkReady()
        {
            while (Spawner == null || !Spawner.IsReadyForFire)
                yield return null;

            StartFiring(transform.up);
        }

        private void Update()
        {
            transform.localRotation *= Quaternion.Euler(_autoRotate * Time.deltaTime);
            _currentDirection = transform.up;
        }
    }
}
