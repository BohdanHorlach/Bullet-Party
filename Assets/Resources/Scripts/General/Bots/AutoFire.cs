using System.Collections;
using General.Combat;
using UnityEngine;

namespace General.Bots
{
    public class AutoFire : FireHandler
    {
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
            _currentDirection = transform.up;
        }


        public void StartFire()
        {
            StartCoroutine(StartWhenNetworkReady());
        }


        public void StopFire()
        {
            StopFiring();
        }
    }
}
