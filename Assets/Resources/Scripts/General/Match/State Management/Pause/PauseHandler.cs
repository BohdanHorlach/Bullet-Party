using UnityEngine;
using UnityEngine.InputSystem;


namespace General.Match.StateManagement
{
    /// <summary>Optional input bridge. Bind InputPause to a Pause action in the Input System.</summary>
    public class PauseHandler : MonoBehaviour
    {
        public void InputPause()
        {
            GamePause.SetPaused(!GamePause.IsPaused);
        }

        public void InputPause(InputAction.CallbackContext context)
        {
            if (!context.started)
                return;

            InputPause();
        }
    }
}
