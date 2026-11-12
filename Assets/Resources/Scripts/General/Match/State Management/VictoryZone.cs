using Fusion;
using General.Player;
using Searching;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace General.Match.StateManagement
{
    public class VictoryZone : NetworkBehaviour
    {
        [SerializeField] private ObjectFinderByMask _playerDetecter;
        [SerializeField] private int _maxExpOnRate = 20;
        [SerializeField] private float _enableDuration = 5f;
        [SerializeField, Range(0.01f, 1f)] private float _completionSpeed;

        private Transform _lastDetected = null;
        private float _completeTime = 0f;
        private bool _stopped = false;


        private void Awake()
        {
            gameObject.SetActive(false);
            Invoke(nameof(EnableZone), _enableDuration);
        }


        private void OnEnable()
        {
            if (!HasStateAuthority)
                return;

            _playerDetecter.OnObjectStay += OnPlayerStay;
            _playerDetecter.OnObjectLost += OnPlayerLost;
        }


        private void EnableZone()
        {
            if (Runner.IsServer)
                gameObject.SetActive(true);
        }


        private Dictionary<PlayerType, int> CallculateRate()
        {
            Dictionary<PlayerType, int> rate = new();
            Dictionary<PlayerType, float> distances = new();
            Vector3 targetPosition = transform.position;

            IEnumerable<PlayerRef> players = Runner.ActivePlayers;

            foreach(PlayerRef player in players)
            {
                if (Runner.TryGetPlayerObject(player, out NetworkObject obj))
                {
                    PlayerAvatar avatar = obj.GetComponent<PlayerAvatar>();
                    float distance = Vector3.Distance(obj.transform.position, targetPosition);
                    distances[avatar.PlayerType] = distance;
                }
            }

            float minDistance = distances.Values.Min();
            float maxDistance = distances.Values.Max();

            float distanceRange = Mathf.Max(maxDistance - minDistance, 0.0001f);

            foreach (var pair in distances)
            {
                float t = (pair.Value - minDistance) / distanceRange;
                int score = Mathf.RoundToInt(Mathf.Lerp(_maxExpOnRate, 0, t));
                rate[pair.Key] = score;
            }

            return rate;
        }


        private void CompleteLevel()
        {
            if (_completeTime < 1f || _stopped)
                return;

            var rate = CallculateRate();
            VictoryNotifier.Notify(rate);
            GamePause.SetPaused(true);            
            _stopped = true;

            _playerDetecter.OnObjectStay -= OnPlayerStay;
            _playerDetecter.OnObjectLost -= OnPlayerLost;
            OnPlayerLost(_lastDetected);
        }


        private void OnPlayerStay(Transform player)
        {
            if (_stopped)
                return;

            if (_lastDetected == null || _lastDetected != player)
            {
                _lastDetected = player;
                _completeTime = 0f;
            }

            _completeTime += _completionSpeed;
            CompleteLevel();
        }


        private void OnPlayerLost(Transform player)
        {
            if (_lastDetected == player)
                _lastDetected = null;
        }
    }
}