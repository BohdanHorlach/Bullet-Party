using UnityEngine;

namespace General.MiniGames.JumpIt
{
    public class SpriteSortByHeight : MonoBehaviour
    {
        [SerializeField] private Transform _root;
        [SerializeField] private float _unitsPerOrder = 0.01f;
        [SerializeField] private int _baseSortingOrder;

        private SpriteRenderer[] _spriteRenderers;

        private void Awake()
        {
            if (_root == null)
                _root = transform;

            _spriteRenderers = _root.GetComponentsInChildren<SpriteRenderer>(true);
        }

        private void LateUpdate()
        {
            if (_spriteRenderers == null || _spriteRenderers.Length == 0)
                return;

            float unitsPerOrder = Mathf.Max(_unitsPerOrder, 0.0001f);

            foreach (SpriteRenderer spriteRenderer in _spriteRenderers)
            {
                if (spriteRenderer == null)
                    continue;

                float y = spriteRenderer.transform.position.y;
                spriteRenderer.sortingOrder = _baseSortingOrder + Mathf.RoundToInt(-y / unitsPerOrder);
            }
        }
    }
}