using UnityEngine;


namespace General.Match.StateManagement
{
    /// <summary>Single entry point for global match pause (players + bullets).</summary>
    public static class GamePause
    {
        public static bool IsPaused =>
            GamePauseState.Instance != null && GamePauseState.Instance.IsGamePaused;

        public static void SetPaused(bool paused)
        {
            if (GamePauseState.Instance == null)
            {
                Debug.LogWarning("[GamePause] Add the Game Pause prefab to the active minigame scene.");
                return;
            }

            GamePauseState.Instance.RequestPaused(paused);
        }
    }
}
