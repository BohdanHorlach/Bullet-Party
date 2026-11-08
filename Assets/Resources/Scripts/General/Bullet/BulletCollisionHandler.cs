using Fusion;
using General.MiniGames;
using General.Player;
using Searching;
using System;
using UnityEngine;


namespace General.Bullet
{
    [RequireComponent(typeof(Collider2D), typeof(ObjectFinderByMask))]
    public class BulletCollisionHandler : NetworkBehaviour
    {
        [SerializeField] private BulletMovement _bulletMovement;
        [SerializeField] private ObjectFinderByMask _playerFinder;
        [SerializeField] private float _knockbackForce = 10f;

        [Networked] private NetworkId OwnerID { get; set; }
        [Networked] private PlayerType OwnerType { get; set; }

        public Action<PlayerRef?> OnHit;


        private void Start()
        {
            if(Object.HasStateAuthority == false) 
            {
                _playerFinder.enabled = false;
                this.enabled = false;
            }
        }


        private void OnEnable()
        {
            _playerFinder.OnObjectDetect += Knockback;
        }


        private void OnDisable()
        {
            _playerFinder.OnObjectDetect -= Knockback;
        }


        private void Knockback(Transform obj)
        {
            if (Object.HasStateAuthority == false)
                return;

            if (obj.TryGetComponent(out PlayerKnockback knockback))
            {
                if (knockback.Object.Id == OwnerID)
                    return;

                knockback.ApplyKnockback(_bulletMovement.MoveDirection, _knockbackForce);
                OnHit?.Invoke(knockback.Object.InputAuthority);
            }
            else if(obj.parent.TryGetComponent(out ObstacleState obstacle))
            {
                if (obstacle.CanIgnored(OwnerType))
                    return;

                OnHit?.Invoke(null);
            }
        }


        public void Inialize(NetworkId ownerId, PlayerType ownerType)
        {
            OwnerID = ownerId;
            OwnerType = ownerType;
        }
    }
}