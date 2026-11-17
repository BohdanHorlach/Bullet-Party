using Fusion;
using UnityEngine;

namespace General.MiniGames.JumpIt
{
    public class LaserTrap : JumpItTrapBase
    {
        [SerializeField] private LaserPoseTransition _laser;
        [SerializeField] private float _holdDuration = 1f;
        [SerializeField] private int _sequenceStartIndex;

        [Networked] private int SyncedStateIndex { get; set; }
        [Networked] private int SequenceStep { get; set; }
        [Networked] private TickTimer StepTimer { get; set; }

        private int _lastRenderedState = int.MinValue;

        protected override void OnActivated()
        {
            if (_laser == null || _laser.StateCount <= 0)
            {
                CompleteTrap();
                return;
            }

            ShowTrap();
            IsBusy = true;
            SequenceStep = Mathf.Clamp(_sequenceStartIndex, 0, _laser.StateCount - 1);
            BeginSequenceStep();
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority || !IsBusy)
                return;

            if (!StepTimer.Expired(Runner))
                return;

            BeginSequenceStep();
        }

        public override void Render()
        {
            base.Render();

            if (_laser == null)
                return;

            if (_lastRenderedState == SyncedStateIndex)
                return;

            _lastRenderedState = SyncedStateIndex;

            if (SyncedStateIndex == 0)
                _laser.ApplyState(0);
            else
                _laser.TransitionToState(SyncedStateIndex);
        }

        private void BeginSequenceStep()
        {
            int stateCount = _laser.StateCount;

            if (SequenceStep >= stateCount)
            {
                SyncedStateIndex = 0;
                CompleteTrap();
                return;
            }

            SyncedStateIndex = SequenceStep;
            SequenceStep++;

            float stepDuration = _laser.TransitionDuration + _holdDuration;
            StepTimer = TickTimer.CreateFromSeconds(Runner, stepDuration);
        }
    }
}