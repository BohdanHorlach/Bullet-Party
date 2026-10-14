using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;


public class PauseHandler : MonoBehaviour
{
    private static PauseHandler _instance;
    private static IPaused[] _pauseds;

    public bool IsPaused { get; private set; } = false;
    public Action<bool> OnPaused;


    public void Awake()
    {
        if (_instance != null)
            return;

        _instance = this;
        _pauseds = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                    .OfType<IPaused>()
                    .ToArray();
    }


    private void SetState(bool state)
    {
        IsPaused = state;
        OnPaused.Invoke(state);
    }


    public static void Play()
    {
        foreach (IPaused item in _pauseds)
            item.Resume();

        _instance.SetState(true);
    }


    public static void Pause()
    {
        foreach (IPaused item in _pauseds)
            item.Pause();

        _instance.SetState(false);
    }


    public void InputPause()
    {
        if (IsPaused)
            Play();
        else
            Pause();
    }


    public void InputPause(InputAction.CallbackContext context)
    {
        if (context.started == false)
            return;

        InputPause();
    }
}