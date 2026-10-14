using Network;
using TMPro;
using UnityEngine;


namespace UI.Network
{
    public class ReconnectOverlay : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private TMP_Text _label;


        private void OnEnable()
        {
            if (NetworkBootstrap.Instance != null)
                NetworkBootstrap.Instance.OnReconnectStatus += OnStatus;

            _panel.SetActive(false);
        }


        private void OnDisable()
        {
            if (NetworkBootstrap.Instance != null)
                NetworkBootstrap.Instance.OnReconnectStatus -= OnStatus;
        }


        private void OnStatus(string message)
        {
            bool show = !string.IsNullOrEmpty(message);
            _panel.SetActive(show);

            if (show)
                _label.text = message;
        }
    }
}