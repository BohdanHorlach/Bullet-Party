using Fusion;
using General.Bullet;
using General.Player;
using System.Collections.Generic;
using UnityEngine;
using UI.MiniGames;
using General.Utils;
using General.Match.StateManagement;

namespace General.MiniGames.DeathMatch
{
    [System.Serializable]
    public struct CounterByType
    {
        public PlayerType PlayerType;
        public Counter Counter;
    }

    public class HitCounter : NetworkBehaviour
    {
        [SerializeField] private CounterByType[] _counters;
        [SerializeField] private ActionTimer _timer;
        private Dictionary<PlayerType, Counter> _score = null;


        public override void Spawned()
        {
            base.Spawned();
            if (Object.HasStateAuthority == false)
                return;

            BulletMediator.OnHitTarget += AwardPoint;

            _score = DictionaryInitializer.InstantiateDictionary(_counters, (counter) => counter.PlayerType,
                (counter) => counter.Counter);

            _timer.OnFinish += FinishGame;
        }


        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (Object.HasStateAuthority)
            {
                BulletMediator.OnHitTarget -= AwardPoint;
            }

            base.Despawned(runner, hasState);
        }


        private void AwardPoint(PlayerRef target, PlayerRef owner)
        {
            if (Object.HasStateAuthority == false)
                return;

            NetworkObject targetObject = Runner.GetPlayerObject(target);
            if (targetObject != null && targetObject.TryGetComponent(out PlayerStun stun))
                stun.Apply();

            NetworkObject ownerObject = Runner.GetPlayerObject(owner);
            PlayerAvatar avatar = ownerObject.GetComponent<PlayerAvatar>();

            _score.TryGetValue(avatar.PlayerType, out Counter score);
            score.Add(1);
        }


        private void FinishGame() {
            Dictionary<PlayerType, int> rate = new();

            foreach (KeyValuePair<PlayerType, Counter> score in _score)
            {
                PlayerType type = score.Key;
                Counter counter = score.Value;

                rate.Add(type, counter.CounterValue);
            }

            VictoryNotifier.Notify(rate);
        }
    }
}
