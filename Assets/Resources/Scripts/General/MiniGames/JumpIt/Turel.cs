using DG.Tweening;
using Fusion;
using General.Bots;
using UnityEngine;

namespace General.MiniGames.JumpIt
{
    public class Turel : JumpItTrapBase
    {
        private const float RotationSpeedEpsilon = 0.001f;

        [SerializeField] private AutoFire _autoFire;
        [SerializeField] private Vector3 _autoRotate;
        [SerializeField] private GameObject _view;

        [Header("Rotation")]
        [SerializeField] private float _acceleration = 0.5f;
        [SerializeField] private float _deceleration = 1f;

        [Header("Startup")]
        [SerializeField] private float _startDelay;
        [SerializeField] private float _activeDuration = 8f;

        [Networked] private Quaternion NetworkRotation { get; set; }
        [Networked] private float RotationSpeed { get; set; }
        [Networked] private float TargetRotationSpeed { get; set; }
        [Networked] private TickTimer ActiveTimer { get; set; }
        [Networked] private NetworkBool IsShuttingDown { get; set; }

        private Tween _startFireTween;

        private void ApplyRotation()
        {
            _autoFire.transform.localRotation = NetworkRotation;

            if (_view != null)
                _view.transform.localRotation = NetworkRotation;
        }

        public override void Spawned()
        {
            if (HasStateAuthority)
            {
                NetworkRotation = transform.localRotation;
                RotationSpeed = 0f;
                TargetRotationSpeed = 0f;
                IsShuttingDown = false;
            }

            ApplyRotation();
        }

        protected override void OnActivated()
        {
            ShowTrap();

            IsBusy = true;
            IsShuttingDown = false;
            StartTurel();
            ActiveTimer = TickTimer.CreateFromSeconds(Runner, _activeDuration);
        }

        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority)
            {
                if (IsBusy && !IsShuttingDown && ActiveTimer.Expired(Runner))
                    BeginShutdown();

                float rate = TargetRotationSpeed > RotationSpeed
                    ? _acceleration
                    : _deceleration;

                RotationSpeed = Mathf.MoveTowards(
                    RotationSpeed,
                    TargetRotationSpeed,
                    _acceleration * Runner.DeltaTime
                );

                Quaternion delta = Quaternion.Euler(
                    _autoRotate * RotationSpeed * Runner.DeltaTime
                );

                NetworkRotation *= delta;

                if (IsBusy && IsShuttingDown && RotationSpeed <= RotationSpeedEpsilon)
                    CompleteTrap();
            }

            ApplyRotation();
        }

        public void StartTurel()
        {
            if (!HasStateAuthority)
                return;

            TargetRotationSpeed = 1f;

            _startFireTween?.Kill();
            _startFireTween = DOVirtual.DelayedCall(_startDelay, () =>
            {
                if (Object != null && HasStateAuthority)
                    _autoFire.StartFire();
            });
        }

        public void StopTurel()
        {
            if (!HasStateAuthority)
                return;

            BeginShutdown();
        }

        private void BeginShutdown()
        {
            if (IsShuttingDown)
                return;

            IsShuttingDown = true;
            TargetRotationSpeed = 0f;

            _startFireTween?.Kill();
            _autoFire.StopFire();
        }

        public override void Render()
        {
            base.Render();
            ApplyRotation();
        }
    }
}