using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace General.MiniGames.JumpIt
{
    public class LaserPoseTransition : MonoBehaviour
    {
        [SerializeField] private Transform _origin;
        [SerializeField] private List<Transform> _states = new();
        [SerializeField] private int _initialStateIndex;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private Ease _ease = Ease.InOutSine;
        [SerializeField] private bool _hideMarkersInPlayMode = true;

        private Sequence _activeSequence;
        private readonly Dictionary<string, Color> _currentColors = new();

        public int CurrentStateIndex { get; private set; } = -1;
        public int StateCount => _states?.Count ?? 0;
        public float TransitionDuration => _duration;
        public bool IsTransitioning => _activeSequence != null && _activeSequence.IsActive();
        public IReadOnlyDictionary<string, Color> CurrentColors => _currentColors;

        private void Awake()
        {
            if (_origin == null)
                _origin = transform.Find("Origin");

            if (_hideMarkersInPlayMode)
            {
                foreach (Transform state in _states)
                {
                    if (state != null)
                        state.gameObject.SetActive(false);
                }
            }

            int index = Mathf.Clamp(_initialStateIndex, 0, Mathf.Max(0, _states.Count - 1));
            ApplyState(index);
        }

        private void OnDestroy()
        {
            _activeSequence?.Kill();
        }

        public void TransitionToState(int stateIndex)
        {
            if (!IsValidState(stateIndex))
                return;

            Transform targetState = _states[stateIndex];
            if (targetState == null)
                return;

            _activeSequence?.Kill();
            _activeSequence = DOTween.Sequence().SetTarget(this);

            Dictionary<string, Transform> targetPoseMap = BuildPathMap(targetState);
            Dictionary<string, Transform> originPoseMap = BuildPathMap(_origin);
            Dictionary<string, SpriteRenderer> targetSpriteMap = BuildSpriteMap(targetState);
            Dictionary<string, SpriteRenderer> originSpriteMap = BuildSpriteMap(_origin);

            foreach (KeyValuePair<string, Transform> pair in originPoseMap)
            {
                if (!targetPoseMap.TryGetValue(pair.Key, out Transform targetTransform))
                    continue;

                Transform originTransform = pair.Value;
                _activeSequence.Join(originTransform
                    .DOLocalMove(targetTransform.localPosition, _duration)
                    .SetEase(_ease));
                _activeSequence.Join(originTransform
                    .DOLocalRotateQuaternion(targetTransform.localRotation, _duration)
                    .SetEase(_ease));
                _activeSequence.Join(originTransform
                    .DOScale(targetTransform.localScale, _duration)
                    .SetEase(_ease));
            }

            foreach (KeyValuePair<string, SpriteRenderer> pair in originSpriteMap)
            {
                if (!targetSpriteMap.TryGetValue(pair.Key, out SpriteRenderer targetRenderer))
                    continue;

                SpriteRenderer originRenderer = pair.Value;
                Color targetColor = targetRenderer.color;
                _activeSequence.Join(originRenderer
                    .DOColor(targetColor, _duration)
                    .SetEase(_ease));
            }

            int capturedIndex = stateIndex;
            _activeSequence.OnComplete(() =>
            {
                CurrentStateIndex = capturedIndex;
                RefreshColorState();
            });
        }

        public void TransitionToNextState()
        {
            if (_states == null || _states.Count == 0)
                return;

            int nextIndex = CurrentStateIndex < 0
                ? 0
                : (CurrentStateIndex + 1) % _states.Count;

            TransitionToState(nextIndex);
        }

        public void ApplyState(int stateIndex)
        {
            if (!IsValidState(stateIndex))
                return;

            Transform state = _states[stateIndex];
            if (state == null || _origin == null)
                return;

            ApplyPoseMap(BuildPathMap(state));
            ApplyColorMap(BuildSpriteMap(state));
            CurrentStateIndex = stateIndex;
            RefreshColorState();
        }

        public bool TryGetCurrentColor(string relativePath, out Color color)
        {
            return _currentColors.TryGetValue(relativePath, out color);
        }

        public void PlayToEnd()
        {
            if (_states == null || _states.Count == 0)
                return;

            TransitionToState(_states.Count - 1);
        }

        public void PlayToStart()
        {
            TransitionToState(0);
        }

        public void ApplyStartState()
        {
            ApplyState(0);
        }

        private bool IsValidState(int stateIndex)
        {
            return _origin != null
                   && _states != null
                   && _states.Count > 0
                   && stateIndex >= 0
                   && stateIndex < _states.Count;
        }

        private void ApplyPoseMap(Dictionary<string, Transform> sourceMap)
        {
            Dictionary<string, Transform> originMap = BuildPathMap(_origin);

            foreach (KeyValuePair<string, Transform> pair in originMap)
            {
                if (!sourceMap.TryGetValue(pair.Key, out Transform sourceTransform))
                    continue;

                Transform originTransform = pair.Value;
                originTransform.localPosition = sourceTransform.localPosition;
                originTransform.localRotation = sourceTransform.localRotation;
                originTransform.localScale = sourceTransform.localScale;
            }
        }

        private void ApplyColorMap(Dictionary<string, SpriteRenderer> sourceMap)
        {
            Dictionary<string, SpriteRenderer> originMap = BuildSpriteMap(_origin);

            foreach (KeyValuePair<string, SpriteRenderer> pair in originMap)
            {
                if (!sourceMap.TryGetValue(pair.Key, out SpriteRenderer sourceRenderer))
                    continue;

                pair.Value.color = sourceRenderer.color;
            }
        }

        private void RefreshColorState()
        {
            _currentColors.Clear();

            if (_origin == null)
                return;

            foreach (KeyValuePair<string, SpriteRenderer> pair in BuildSpriteMap(_origin))
                _currentColors[pair.Key] = pair.Value.color;
        }

        private static Dictionary<string, Transform> BuildPathMap(Transform root)
        {
            if (root == null)
                return new Dictionary<string, Transform>();

            var map = new Dictionary<string, Transform>();
            CollectPaths(root, root, string.Empty, map);
            return map;
        }

        private static Dictionary<string, SpriteRenderer> BuildSpriteMap(Transform root)
        {
            var map = new Dictionary<string, SpriteRenderer>();

            foreach (KeyValuePair<string, Transform> pair in BuildPathMap(root))
            {
                SpriteRenderer spriteRenderer = pair.Value.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                    map[pair.Key] = spriteRenderer;
            }

            return map;
        }

        private static void CollectPaths(
            Transform root,
            Transform current,
            string path,
            Dictionary<string, Transform> map)
        {
            if (current != root)
                map[path] = current;

            for (int i = 0; i < current.childCount; i++)
            {
                Transform child = current.GetChild(i);
                string childPath = string.IsNullOrEmpty(path)
                    ? child.name
                    : $"{path}/{child.name}";

                CollectPaths(root, child, childPath, map);
            }
        }
    }
}