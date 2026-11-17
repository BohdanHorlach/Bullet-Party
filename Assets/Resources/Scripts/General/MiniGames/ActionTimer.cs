using Fusion;
using System;
using TMPro;
using UnityEngine;


namespace General.MiniGames
{
    public class ActionTimer : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private float _duration = 60f;
        [Networked] private TickTimer Timer { get; set; }

        public Action OnFinish;
        public bool IsRunning => Timer.IsRunning;

        public bool IsMatchRunning(NetworkRunner runner)
        {
            return Timer.IsRunning && !Timer.ExpiredOrNotRunning(runner);
        }


        public override void Spawned()
        {
            if (HasStateAuthority)
            {
                Timer = TickTimer.CreateFromSeconds(
                    Runner,
                    _duration
                );
            }
        }


        public override void Render()
        {
            if (!Timer.IsRunning)
                return;

            float remaining = Timer.RemainingTime(Runner) ?? 0f;

            _timerText.text = Mathf.CeilToInt(remaining).ToString();
        }


        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority && Timer.Expired(Runner))
            {
                Timer = TickTimer.None;
                OnFinish?.Invoke();
            }
        }
    }
}
