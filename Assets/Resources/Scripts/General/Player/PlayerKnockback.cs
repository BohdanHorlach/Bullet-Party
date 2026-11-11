using Fusion;
using General.Match.StateManagement;
using UnityEngine;

namespace General.Player
{
    public class PlayerKnockback : NetworkBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private float _knockbackDuration = 0.25f;

        [Networked] private TickTimer KnockbackTimer { get; set; }

        public bool IsKnockback => !KnockbackTimer.ExpiredOrNotRunning(Runner);


        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority)
                return;

            if (GamePause.IsPaused)
            {
                _rigidbody.linearVelocity = Vector2.zero;
                _rigidbody.angularVelocity = 0f;
                return;
            }

            if (KnockbackTimer.Expired(Runner))
            {
                KnockbackTimer = default;
                _rigidbody.linearVelocity = Vector2.zero;
                _rigidbody.angularVelocity = 0f;
            }
        }


        public void ApplyKnockback(Vector2 direction, float force)
        {
            if (!HasStateAuthority || _rigidbody == null)
                return;

            KnockbackTimer = TickTimer.CreateFromSeconds(Runner, _knockbackDuration);
            _rigidbody.linearVelocity = direction.normalized * force;
            _rigidbody.angularVelocity = 0f;
        }
    }
}