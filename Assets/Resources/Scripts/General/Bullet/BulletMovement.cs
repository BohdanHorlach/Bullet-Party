using Fusion;
using UnityEngine;


namespace General.Bullet
{
    public class BulletMovement : NetworkBehaviour
    {
        [SerializeField] private float _speed = 25f;
        [SerializeField] private float _smoothTime = 0.1f;
        private Vector3 _velocity;

        [Networked] private Vector3 NetworkedPosition { get; set; }
        [Networked] private Vector3 Direction { get; set; }

        public Vector3 MoveDirection => Direction;


        public void Initialize(Vector3 position, Vector3 direction)
        {
            NetworkedPosition = position;
            Direction = direction;
            transform.position = position;
        }


        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            if (Object.HasStateAuthority)
            {
                NetworkedPosition += Direction * _speed * Runner.DeltaTime;
            }
        }


        public override void Render()
        {
            base.Render();

            if (!Object.HasStateAuthority)
            {
                transform.position = Vector3.SmoothDamp(
                    transform.position,
                    NetworkedPosition,
                    ref _velocity,
                    _smoothTime
                );
            }
            else
            {
                transform.position = NetworkedPosition;
            }
        }
    }
}