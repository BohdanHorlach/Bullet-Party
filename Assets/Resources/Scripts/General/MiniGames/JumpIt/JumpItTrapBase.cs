using Fusion;
using UnityEngine;

namespace General.MiniGames.JumpIt
{
    public abstract class JumpItTrapBase : NetworkBehaviour, IJumpItTrap
    {
        [SerializeField] private bool _hideWhenIdle = true;

        [Networked] public NetworkBool IsBusy { get; protected set; }
        [Networked] protected NetworkBool IsShown { get; set; }

        bool IJumpItTrap.IsBusy => IsBusy;

        public bool TryActivateTrap()
        {
            if (!HasStateAuthority || IsBusy)
                return false;

            OnActivated();
            return IsBusy;
        }

        public void ActivateTrap()
        {
            TryActivateTrap();
        }

        protected abstract void OnActivated();

        protected void CompleteTrap()
        {
            if (!HasStateAuthority)
                return;

            IsBusy = false;
            IsShown = false;
        }

        protected void ShowTrap()
        {
            if (!HasStateAuthority)
                return;

            IsShown = true;

            if (_hideWhenIdle && !gameObject.activeSelf)
                gameObject.SetActive(true);
        }

        public override void Render()
        {
            if (_hideWhenIdle)
                gameObject.SetActive(IsShown);
        }
    }
}