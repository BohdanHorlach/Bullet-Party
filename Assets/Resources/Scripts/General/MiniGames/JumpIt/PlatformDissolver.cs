using Fusion;
using UnityEngine;

namespace General.MiniGames.JumpIt
{
    public class PlatformDissolver : JumpItTrapBase
    {
        private enum StepPhase
        {
            Idle,
            Dissolving,
            Recovery,
        }

        private const int MaxPatternLength = 16;

        [SerializeField] private GameObject[] _platforms;
        [SerializeField] private int _patternLengthMin = 3;
        [SerializeField] private int _patternLengthMax = 5;
        [SerializeField] private float _dissolveDuration = 2f;
        [SerializeField] private float _gapAfterReappear = 0.5f;

        [Networked, Capacity(4)] private NetworkArray<bool> States => default;
        [Networked, Capacity(MaxPatternLength)] private NetworkArray<int> PatternIndices => default;

        [Networked] private StepPhase Phase { get; set; }
        [Networked] private int PatternLength { get; set; }
        [Networked] private int PatternStep { get; set; }
        [Networked] private int CurrentPlatformIndex { get; set; }
        [Networked] private TickTimer PhaseTimer { get; set; }

        public override void Spawned()
        {
            if (!HasStateAuthority)
                return;

            EnsureAllPlatformsActive();
        }

        protected override void OnActivated()
        {
            if (_platforms == null || _platforms.Length == 0)
            {
                CompleteTrap();
                return;
            }

            PatternLength = Random.Range(_patternLengthMin, _patternLengthMax + 1);
            PatternLength = Mathf.Min(PatternLength, MaxPatternLength);

            for (int i = 0; i < PatternLength; i++)
                PatternIndices.Set(i, Random.Range(0, _platforms.Length));

            PatternStep = 0;
            Phase = StepPhase.Idle;
            PhaseTimer = default;
            IsBusy = true;
            EnsureAllPlatformsActive();
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority || !IsBusy)
                return;

            if (Phase == StepPhase.Idle)
            {
                BeginStep();
                return;
            }

            if (!PhaseTimer.Expired(Runner))
                return;

            AdvancePhase();
        }

        public override void Render()
        {
            if (_platforms == null)
                return;

            int count = Mathf.Min(_platforms.Length, States.Length);
            for (int i = 0; i < count; i++)
            {
                GameObject platform = _platforms[i];
                if (platform != null)
                    platform.SetActive(States[i]);
            }
        }

        private void BeginStep()
        {
            CurrentPlatformIndex = PatternIndices.Get(PatternStep);
            CurrentPlatformIndex = Mathf.Clamp(CurrentPlatformIndex, 0, _platforms.Length - 1);

            States.Set(CurrentPlatformIndex, false);
            Phase = StepPhase.Dissolving;
            PhaseTimer = TickTimer.CreateFromSeconds(Runner, _dissolveDuration);
        }

        private void AdvancePhase()
        {
            switch (Phase)
            {
                case StepPhase.Dissolving:
                    States.Set(CurrentPlatformIndex, true);
                    Phase = StepPhase.Recovery;
                    PhaseTimer = TickTimer.CreateFromSeconds(Runner, _gapAfterReappear);
                    break;

                case StepPhase.Recovery:
                    PatternStep++;

                    if (PatternStep >= PatternLength)
                    {
                        Phase = StepPhase.Idle;
                        EnsureAllPlatformsActive();
                        CompleteTrap();
                    }
                    else
                    {
                        BeginStep();
                    }

                    break;
            }
        }

        private void EnsureAllPlatformsActive()
        {
            for (int i = 0; i < _platforms.Length && i < States.Length; i++)
                States.Set(i, true);
        }
    }
}