using Fusion;
using General.MiniGames;
using UnityEngine;

namespace General.MiniGames.JumpIt
{
    public class TrapsMediator : NetworkBehaviour
    {
        private enum Phase
        {
            Prepare,
            Running,
            Finished,
        }

        [SerializeField] private ActionTimer _actionTimer;
        [SerializeField] private JumpItTrapBase[] _traps;
        [SerializeField] private float _prepareDelay = 3f;

        [Networked] private Phase CurrentPhase { get; set; }
        [Networked] private TickTimer PrepareTimer { get; set; }

        private int _lastTrapIndex = -1;

        public override void Spawned()
        {
            if (!HasStateAuthority)
                return;

            CurrentPhase = Phase.Prepare;
            PrepareTimer = TickTimer.CreateFromSeconds(Runner, _prepareDelay);
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority)
                return;

            switch (CurrentPhase)
            {
                case Phase.Prepare:
                    if (PrepareTimer.Expired(Runner))
                        CurrentPhase = Phase.Running;
                    break;

                case Phase.Running:
                    if (_actionTimer == null || !_actionTimer.IsMatchRunning(Runner))
                    {
                        CurrentPhase = Phase.Finished;
                        break;
                    }

                    if (IsAnyTrapBusy())
                        break;

                    if (!TryActivateRandomTrap())
                        break;
                    break;
            }
        }

        private bool IsAnyTrapBusy()
        {
            if (_traps == null)
                return false;

            foreach (JumpItTrapBase trap in _traps)
            {
                if (trap == null || !IsTrapSpawned(trap))
                    continue;

                if (trap.IsBusy)
                    return true;
            }

            return false;
        }

        private bool TryActivateRandomTrap()
        {
            if (_traps == null || _traps.Length == 0)
                return false;

            int startIndex = Random.Range(0, _traps.Length);

            for (int offset = 0; offset < _traps.Length; offset++)
            {
                int index = (startIndex + offset) % _traps.Length;

                if (index == _lastTrapIndex && _traps.Length > 1)
                    continue;

                JumpItTrapBase trap = _traps[index];
                if (trap == null || !IsTrapSpawned(trap))
                    continue;

                if (trap.TryActivateTrap())
                {
                    _lastTrapIndex = index;
                    return true;
                }
            }

            return false;
        }

        private static bool IsTrapSpawned(JumpItTrapBase trap)
        {
            return trap.Object != null && trap.Object.IsValid;
        }
    }
}