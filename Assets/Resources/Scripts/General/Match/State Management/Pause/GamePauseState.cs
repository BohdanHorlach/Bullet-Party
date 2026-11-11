using Fusion;
using General.Utils;
using UnityEngine;


namespace General.Match.StateManagement
{
    /// <summary>Replicated match pause; use <see cref="GamePause"/> to change state from anywhere.</summary>
    public class GamePauseState : NetworkBehaviour
    {
        public static GamePauseState Instance { get; private set; }

        [Networked, OnChangedRender(nameof(OnPausedRender))]
        public NetworkBool IsGamePaused { get; private set; }

        public override void Spawned()
        {
            Instance = this;
            ApplyLocalInput(IsGamePaused);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (Instance != this)
                return;

            Instance = null;
            ApplyLocalInput(false);
        }

        public void RequestPaused(bool paused)
        {
            if (Runner == null || !Runner.IsRunning)
                return;

            if (HasStateAuthority)
                IsGamePaused = paused;
            else
                RPC_RequestPaused(paused);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_RequestPaused(bool paused, RpcInfo info = default)
        {
            IsGamePaused = paused;
        }

        private void OnPausedRender()
        {
            ApplyLocalInput(IsGamePaused);
        }

        private static void ApplyLocalInput(bool paused)
        {
            InputReader.IsInputLocked = paused;
        }
    }
}
