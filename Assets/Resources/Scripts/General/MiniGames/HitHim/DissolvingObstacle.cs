using DG.Tweening;
using Fusion;
using UnityEngine;

namespace General.MiniGames.DeathMatch
{
    public class DissolvingObstacle : NetworkBehaviour
    {
        private enum DissolvingState
        {
            Active,
            Transition,
            Inactive,
        }

        [SerializeField] private SpriteFlicker _flicker;
        [SerializeField] private ObstacleState _obstacleState;

        [SerializeField] private float _cooldown = 3f;
        [SerializeField, Range(0.1f, 5f)] private float _delay = 0.1f;

        [Networked]
        private DissolvingState State { get; set; }

        [Networked]
        private NetworkBool TargetActive { get; set; }

        [Networked]
        private TickTimer Timer { get; set; }

        private DissolvingState _lastRenderedState;
        private bool _hasRenderedState;

        public override void Spawned()
        {
            if (Object.HasStateAuthority)
            {
                State = DissolvingState.Active;

                DOVirtual.DelayedCall(_delay, () =>
                {
                    if (Object != null && Object.HasStateAuthority)
                    {
                        Timer = TickTimer.CreateFromSeconds(
                            Runner, _cooldown);
                    }
                });
            }

            _obstacleState.SetEnable(State != DissolvingState.Inactive);

            _lastRenderedState = State;
            _hasRenderedState = true;
        }

        public override void FixedUpdateNetwork()
        {
            if (!Object.HasStateAuthority || !Timer.Expired(Runner))
                return;

            switch (State)
            {
                case DissolvingState.Active:
                    State = DissolvingState.Transition;
                    TargetActive = false;
                    Timer = TickTimer.CreateFromSeconds(
                        Runner, _cooldown);
                    break;

                case DissolvingState.Transition:
                    State = TargetActive
                        ? DissolvingState.Active
                        : DissolvingState.Inactive;

                    Timer = TickTimer.CreateFromSeconds(
                        Runner, _cooldown);
                    break;

                case DissolvingState.Inactive:
                    State = DissolvingState.Transition;
                    TargetActive = true;
                    Timer = TickTimer.CreateFromSeconds(
                        Runner, _cooldown);
                    break;
            }
        }

        public override void Render()
        {
            if (_hasRenderedState && State == _lastRenderedState)
                return;

            _lastRenderedState = State;
            _hasRenderedState = true;

            switch (State)
            {
                case DissolvingState.Active:
                    _flicker.Stop(false);
                    _obstacleState.SetEnable(true);
                    break;

                case DissolvingState.Transition:
                    _obstacleState.SetEnable(false);
                    _flicker.Play(_cooldown);
                    break;

                case DissolvingState.Inactive:
                    _flicker.Stop(false);
                    _obstacleState.SetEnable(false);
                    break;
            }
        }
    }
}