using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using General.Utils;
using System.Collections.Generic;


namespace UI.AlertSystem
{
    [Serializable]
    public struct AlertIcons
    {
        public AlertStatus Key;
        public Sprite Value;
    }


    public class AlertWindow : MonoBehaviour
    {
        [SerializeField] private AlertIcons[] _iconsData;
        [SerializeField] private RectTransform _transform;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _icon;

        private Dictionary<AlertStatus, Sprite> _icons;
        private RectTransform _showPosition;
        private RectTransform _hidePosition;
        private float _duration;
        private float _hideDelay;
        private bool _isHide = true;

        public static Action<AlertWindow> OnHide;


        private void AutoHide(bool autoHide)
        {
            if (autoHide)
                DOVirtual.DelayedCall(_hideDelay, Hide);
        }


        public void Initialize(float duration, float hideDelay, RectTransform showPosition, RectTransform hidePosition)
        {
            _transform.position = hidePosition.position;
            _showPosition = showPosition;
            _hidePosition = hidePosition;
            _duration = duration;
            _hideDelay = hideDelay;

            _icons = DictionaryInitializer.InstantiateDictionary(
                _iconsData,
                item => item.Key,
                item => item.Value
            );
        }


        public void Show(string message, bool autoHide = true, AlertStatus status = AlertStatus.Success)
        {
            _text.text = message;
            _isHide = false;
            _icon.sprite = _icons[status];
            _transform.DOAnchorPos(_showPosition.anchoredPosition, _duration)
                .OnComplete(() => { AutoHide(autoHide); });
        }


        public void Hide()
        {
            if (_isHide)
                return;

            _transform.DOAnchorPos(_hidePosition.anchoredPosition, _duration);
            OnHide?.Invoke(this);
            _isHide = true;
        }
    }
}