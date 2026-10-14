using Fusion;
using Network;
using UnityEngine;


namespace General.Player
{
    public class PlayerMovement : NetworkBehaviour
    {
        [SerializeField] private PlayerKnockback _knockback;
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private float _speed = 15f;


        public override void FixedUpdateNetwork()
        {
            if (_knockback.IsKnockback)
                return;

            if (GetInput(out NetworkInputData data))
                _rigidbody.linearVelocity = data.direction.normalized * _speed;
        }
    }
}