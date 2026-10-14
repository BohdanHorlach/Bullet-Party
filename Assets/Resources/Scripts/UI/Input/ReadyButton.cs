using General.Player;
using UnityEngine;


namespace UI.Input
{
    public class ReadyButton : MonoBehaviour
    {
        public void OnReadyClicked()
        {
            if (PlayerNetwork.Local == null)
            {
                Debug.LogWarning("Local player not spawned yet");
                return;
            }

            PlayerNetwork.Local.RPC_SetReady();
        }
    }
}