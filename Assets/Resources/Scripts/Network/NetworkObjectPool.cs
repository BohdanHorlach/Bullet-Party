using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Network
{
    public class NetworkObjectPool : NetworkBehaviour
    {
        [SerializeField] private NetworkPrefabRef _prefab;
        [SerializeField] private int _initialSize = 100;
        [SerializeField, Range(100, 500)] private int _maxSize = 500;

        private List<NetworkObject> _pool = new List<NetworkObject>();
        private int _lastForceReleased = -1;

        [Networked, Capacity(500)] private NetworkArray<NetworkId> ObjectIds { get; }
        [Networked, Capacity(500)] private NetworkArray<bool> Activated { get; }
        [Networked] private int CurrentSize { get; set; }

        public bool IsSpawned => Object != null && Object.IsValid;


        public override void Spawned()
        {
            base.Spawned();
            Initialize();
        }


        private void Initialize()
        {
            Debug.Log(Runner);
            if (Object.HasStateAuthority == false)
                return;

            for (int i = 0; i < _initialSize; i++)
            {
                CreateNew(false);
            }

            CurrentSize = _initialSize;
        }


        public int FindInactiveIndex()
        {
            if (!IsSpawned || !HasStateAuthority)
                return -1;

            for (int i = 0; i < _initialSize; i++)
            {
                if (!Activated.Get(i))
                    return i;
            }
            return -1;
        }


        private void SetActive(int index, bool isActive)
        {
            Activated.Set(index, isActive);
            _pool[index].gameObject.SetActive(isActive);
        }


        private NetworkObject CreateNew(bool isActive)
        {
            NetworkObject obj = Runner.Spawn(_prefab, Vector3.zero, Quaternion.identity);
            _pool.Add(obj);

            int index = _pool.IndexOf(obj);

            ObjectIds.Set(index, obj.Id);
            Activated.Set(index, isActive);
            SetActive(index, isActive);

            obj.transform.SetParent(transform);

            return obj;
        }


        private NetworkObject ForceLastRelease()
        {
            _lastForceReleased = (_lastForceReleased + 1) % _maxSize;
            SetActive(_lastForceReleased, true);

            return _pool[_lastForceReleased];
        }


        private void RefreshPool(int index)
        {
            var id = ObjectIds.Get(index);

            if (id.Raw == 0)
                return;

            if (index >= _pool.Count)
                _pool.Add(Runner.FindObject(id));
        }


        private void RefreshActive(int index)
        {
            var obj = _pool[index];

            if (obj == null)
                return;

            bool active = Activated.Get(index);
            if (obj.gameObject.activeSelf != active)
                obj.gameObject.SetActive(active);
        }


        private void UpdatePoolInfo()
        {
            for (int i = 0; i < CurrentSize; i++)
            {
                RefreshPool(i);
                RefreshActive(i);
            }
        }


        public NetworkObject Get(int index)
        {
            if (index == -1)
            {
                if (_pool.Count == _maxSize)
                    return ForceLastRelease();
                else
                    return CreateNew(true);
            }

            SetActive(index, true);
            NetworkObject obj = _pool[index];
           
            return obj;
        }


        public NetworkObject Get()
        {
            int index = FindInactiveIndex();
            return Get(index);
        }


        public void Release(NetworkObject obj)
        {
            if (!Object.HasStateAuthority)
                return;

            int index = _pool.IndexOf(obj);
            if (index == -1)
                return;

            SetActive(index, false);
        }


        public override void Render()
        {
            if (Object.HasStateAuthority)
                CurrentSize = _pool.Count;
            else
                UpdatePoolInfo();
        }
    }
}