using System.Collections;
using UnityEngine;


namespace General.MiniGames.DeathMatch
{
    public class SpriteFlicker : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private float _interval = 0.08f;
        [SerializeField] private float _dimmedAlpha = 0.25f;

        private Coroutine _routine;
        private Color _originalColor;


        private void Awake()
        {
            if (_sprite == null)
                _sprite = GetComponentInChildren<SpriteRenderer>();

            if (_sprite != null)
                _originalColor = _sprite.color;
        }


        public void Play(float duration)
        {
            if (_sprite == null || duration <= 0f)
                return;

            if (_routine != null)
                StopCoroutine(_routine);

            _routine = StartCoroutine(Flicker(duration));
        }


        public void Stop(bool withReset = true)
        {
            Color color = withReset ? _originalColor : _sprite.color;

            StopCoroutine(_routine);
            _routine = null;

            _sprite.color = color;
        }


        private void OnDisable()
        {
            if (_routine != null)
            {
                Stop();
            }
        }


        private IEnumerator Flicker(float duration)
        {
            float elapsed = 0f;
            bool dimmed = false;

            while (elapsed < duration)
            {
                dimmed = !dimmed;

                Color color = _originalColor;
                color.a = dimmed ? _originalColor.a * _dimmedAlpha : _originalColor.a;
                _sprite.color = color;

                yield return new WaitForSeconds(_interval);
                elapsed += _interval;
            }

            _sprite.color = _originalColor;
            _routine = null;
        }
    }
}
