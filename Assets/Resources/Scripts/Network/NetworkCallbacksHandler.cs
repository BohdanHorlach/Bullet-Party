using Fusion;
using Fusion.Sockets;
using General.Match.StateManagement;
using General.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Network
{
    public class NetworkCallbacksHandler : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private float _reconnectGraceSeconds = 45f; // трохи більше за загальний час спроб клієнта; 0 вимикає

        private readonly Dictionary<PlayerRef, string> _tokenByPlayer = new Dictionary<PlayerRef, string>();
        private readonly Dictionary<string, NetworkObject> _objectByToken = new Dictionary<string, NetworkObject>();
        private readonly Dictionary<string, Coroutine> _graceByToken = new Dictionary<string, Coroutine>();


        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (!runner.IsServer)
                return;

            string token = GetToken(runner, player);

            // гравець повернувся: віддаємо йому старий об'єкт
            if (token != null && _objectByToken.TryGetValue(token, out NetworkObject existing) && existing != null)
            {
                CancelGrace(token);
                existing.AssignInputAuthority(player);
                _tokenByPlayer[player] = token;

                Debug.Log($"<color=cyan>[NetworkCallbacksHandler]</color> - Player {player} reconnected");
                return;
            }

            Debug.Log("Spawn Player");
            NetworkObject spawned = PlayerSpawner.Instance.Spawn(runner, player);

            if (token != null && spawned != null)
            {
                _tokenByPlayer[player] = token;
                _objectByToken[token] = spawned;
            }
        }


        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (!runner.IsServer)
                return;

            // PlayerRef після реконекту буде новим, тому стару готовність прибираємо
            if (ReadyManager.Instance != null)
                ReadyManager.Instance.RemovePlayer(player);

            if (_reconnectGraceSeconds > 0f &&
                _tokenByPlayer.Remove(player, out string token) &&
                _objectByToken.TryGetValue(token, out NetworkObject obj) &&
                obj != null)
            {
                obj.RemoveInputAuthority();
                _graceByToken[token] = StartCoroutine(DespawnAfterGrace(runner, token, obj));
                return;
            }

            _tokenByPlayer.Remove(player);
            PlayerSpawner.Instance.Despawn(runner, player);
        }


        private IEnumerator DespawnAfterGrace(NetworkRunner runner, string token, NetworkObject obj)
        {
            yield return new WaitForSeconds(_reconnectGraceSeconds);

            _graceByToken.Remove(token);
            _objectByToken.Remove(token);

            if (runner != null && runner.IsRunning && obj != null)
                runner.Despawn(obj);
        }


        private void CancelGrace(string token)
        {
            if (_graceByToken.Remove(token, out Coroutine routine) && routine != null)
                StopCoroutine(routine);
        }


        private static string GetToken(NetworkRunner runner, PlayerRef player)
        {
            byte[] raw = runner.GetPlayerConnectionToken(player);
            return raw == null || raw.Length == 0 ? null : Convert.ToBase64String(raw);
        }


        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            NetworkInputData data = new NetworkInputData();
            data.direction = InputReader.MoveDirection;

            input.Set(data);
        }


        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log($"<color=cyan>[NetworkCallbacksHandler]</color> - OnShutdown: {shutdownReason}");

            if (NetworkBootstrap.Instance != null)
                NetworkBootstrap.Instance.HandleRunnerShutdown(runner, shutdownReason);
        }


        public void OnSceneLoadDone(NetworkRunner runner)
        {
            Debug.Log($"<color=cyan>[NetworkCallbacksHandler]</color> - OnSceneLoadDone: {SceneManager.GetActiveScene().name}");
        }


        public void OnSceneLoadStart(NetworkRunner runner)
        {
            Debug.Log("<color=cyan>[NetworkCallbacksHandler]</color> - Scene load started");
        }


        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ReadOnlySpan<byte> data) { }
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    }
}