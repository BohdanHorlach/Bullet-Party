using System;
using UnityEngine;


namespace Searching
{
    public class ObjectFinderByMask : MonoBehaviour
    {
        [SerializeField] private LayerMask _layerMask;

        private Transform _lastDetected;

        public Action<Transform> OnObjectDetect;
        public Action<Transform> OnObjectStay;
        public Action<Transform> OnObjectLost;
        public bool IsHaveObjectInSpace => _lastDetected != null;
        public Transform LastDetected { get => _lastDetected; }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_layerMask.IsEnterOnMask(other.transform) == false)
                return;

            OnObjectDetect?.Invoke(other.transform);
            _lastDetected = other.transform;
        }


        private void OnTriggerStay2D(Collider2D other)
        {
            if (_layerMask.IsEnterOnMask(other.transform) == false)
                return;

            OnObjectStay?.Invoke(other.transform);
        }


        private void OnTriggerExit2D(Collider2D other)
        {
            if (_layerMask.IsEnterOnMask(other.transform) == false)
                return;

            OnObjectLost?.Invoke(other.transform);

            if (_lastDetected == other.transform)
                _lastDetected = null;
        }


        public void Reset()
        {
            _lastDetected = null;
        }
    }
}