using General.Player;
using UnityEngine;

namespace General.MiniGames
{
    public class ObstacleState : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private PlayerType _ignoredType;
        [SerializeField, Range(0, 1)] private float _disableAlpha;

        public void SetEnable(bool enable)
        {
            Color color = _spriteRenderer.color;
            color.a = enable ? 1f : _disableAlpha;

            _spriteRenderer.color = color;
            _collider.enabled = enable;
        }

        public bool CanIgnored(PlayerType type)
        {
            return _ignoredType == type;
        }
    }
}