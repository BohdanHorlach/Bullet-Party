using Fusion;
using General.Utils;
using System.Collections;
using UnityEngine;


namespace General.MiniGames.DeathMatch
{
    public class PlayerStun : NetworkBehaviour
    {
        [SerializeField] private SpriteFlicker _flicker;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private float _duration = 0.5f;

        private Coroutine _routine;


        private void Awake()
        {
            if (_flicker == null)
                _flicker = GetComponent<SpriteFlicker>();
        }


        public void Apply()
        {
            if (HasStateAuthority == false || _duration <= 0f)
                return;

            RPC_Apply(_duration);
        }


        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            StopLock();

            if (HasInputAuthority)
                InputReader.IsInputLocked = false;
        }


        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_Apply(float duration)
        {
            _flicker?.Play(duration);

            if (HasInputAuthority == false)
                return;

            StopLock();
            _routine = StartCoroutine(LockInput(duration));
        }


        private IEnumerator LockInput(float duration)
        {
            InputReader.IsInputLocked = true;
            _collider.enabled = false;

            yield return new WaitForSeconds(duration);

            InputReader.IsInputLocked = false;
            _collider.enabled = true;
            _routine = null;
        }


        private void StopLock()
        {
            if (_routine == null)
                return;

            StopCoroutine(_routine);
            _routine = null;
        }
    }
}
