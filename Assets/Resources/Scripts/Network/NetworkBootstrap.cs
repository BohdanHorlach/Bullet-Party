using Fusion;
using General.Match.StateManagement;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;


namespace Network
{
    public class NetworkBootstrap : MonoBehaviour
    {
        [SerializeField] private NetworkRunner _runnerPrefab;
        [SerializeField] private MatchCompletedHandler _matchCompletedHandler;
        [SerializeField] private int _titleSceneIndex = 1;

        [Header("Reconnect")]
        [SerializeField] private int _maxReconnectAttempts = 6;
        [SerializeField] private float _baseReconnectDelay = 1f;
        [SerializeField] private float _maxReconnectDelay = 8f;

        private const int MAX_PLAYERS_COUNT = 4;

        private NetworkRunner _runner;
        private bool _sessionActive;
        private CancellationTokenSource _reconnectCts;

        // параметри останньої користувацької сесії (потрібні для реконекту)
        private string _roomCode;
        private SceneRef _scene;
        private GameMode _mode;
        private byte[] _connectionToken;
        private string _disconnectMessage;


        public static NetworkBootstrap Instance;

        public bool IsReconnecting { get; private set; }
        public event Action<string> OnReconnectStatus;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }


        private void OnDestroy()
        {
            _reconnectCts?.Cancel();

            if (Instance == this)
                Instance = null;
        }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ResetStatics()
        {
            if (Instance == null)
                return;

            Instance._reconnectCts?.Cancel();

            if (Instance._runner != null)
                Instance._runner.Shutdown();

            Instance = null;
        }


        // Нова сесія за ініціативою гравця (кнопки Create/Join)
        public async Task<StartGameResult> StartGame(string roomCode, SceneRef scene, GameMode mode = GameMode.Client)
        {
            _reconnectCts?.Cancel();

            _roomCode = roomCode;
            _scene = scene;
            _mode = mode;
            _connectionToken = Guid.NewGuid().ToByteArray(); // той самий токен буде в усіх спробах реконекту

            StartGameResult result = await StartInternal(roomCode, scene, mode, _connectionToken);

            if (!result.Ok)
                _roomCode = null;

            return result;
        }


        private async Task<StartGameResult> StartInternal(string roomCode, SceneRef scene, GameMode mode, byte[] token)
        {
            await CleanupRunner();

            NetworkRunner runner = Instantiate(_runnerPrefab, transform);
            _runner = runner;
            _matchCompletedHandler.Initialize(runner);
            runner.ProvideInput = true;

            NetworkSceneManagerDefault sceneManager = runner.GetComponent<NetworkSceneManagerDefault>();

            StartGameResult result = await runner.StartGame(new StartGameArgs
            {
                GameMode = mode,
                SessionName = roomCode,
                Scene = scene,
                SceneManager = sceneManager,
                PlayerCount = MAX_PLAYERS_COUNT,
                ConnectionToken = token
            });

            if (!result.Ok)
            {
                Debug.LogWarning($"StartGame failed: {result.ShutdownReason} / {result.ErrorMessage}");

                // Fusion міг уже сам знищити раннер, CleanupRunner це враховує
                if (_runner == runner)
                    await CleanupRunner();

                return result;
            }

            _sessionActive = true;
            return result;
        }


        public async Task LoadNextScene(int gameIndex)
        {
            if (_runner != null && _runner.IsServer)
            {
                SceneRef sceneRef = SceneRef.FromIndex(gameIndex);
                await _runner.LoadScene(sceneRef, LoadSceneMode.Additive);
            }
        }


        public async Task SetNextScene(int gameIndex)
        {
            if (_runner != null && _runner.IsServer)
            {
                SceneRef sceneRef = SceneRef.FromIndex(gameIndex);
                await _runner.LoadScene(sceneRef, LoadSceneMode.Single);
            }
        }


        // Свідомий вихід гравця (кнопка "Leave"), також скасовує реконект
        public async Task LeaveSession()
        {
            _reconnectCts?.Cancel();
            _roomCode = null;

            await CleanupRunner();
            GoToTitle(null);
        }


        // Викликається з NetworkCallbacksHandler.OnShutdown
        public void HandleRunnerShutdown(NetworkRunner runner, ShutdownReason reason)
        {
            if (runner != _runner || !_sessionActive)
                return;

            _sessionActive = false;
            _runner = null;
            _matchCompletedHandler.ResetRunner();

            if (runner != null)
                Destroy(runner.gameObject);

            if (CanReconnect(reason))
            {
                _ = ReconnectRoutine();
                return;
            }

            GoToTitle(GetDisconnectMessage(reason));
        }


        private bool CanReconnect(ShutdownReason reason)
        {
            if (_mode != GameMode.Client || string.IsNullOrEmpty(_roomCode) || _connectionToken == null)
                return false;

            switch (reason)
            {
                case ShutdownReason.ConnectionTimeout:
                case ShutdownReason.PhotonCloudTimeout:
                case ShutdownReason.OperationTimeout:
                case ShutdownReason.DisconnectedByPluginLogic:
                case ShutdownReason.Error:
                    return true;
                default:
                    return false;
            }
        }


        private async Task ReconnectRoutine()
        {
            _reconnectCts?.Dispose();
            _reconnectCts = new CancellationTokenSource();
            CancellationToken ct = _reconnectCts.Token;

            IsReconnecting = true;

            try
            {
                for (int attempt = 1; attempt <= _maxReconnectAttempts; attempt++)
                {
                    OnReconnectStatus?.Invoke($"Connection lost. Reconnecting ({attempt}/{_maxReconnectAttempts})...");

                    float delay = Mathf.Min(_baseReconnectDelay * Mathf.Pow(2f, attempt - 1), _maxReconnectDelay);
                    await Task.Delay(TimeSpan.FromSeconds(delay), ct);

                    StartGameResult result = await StartInternal(_roomCode, _scene, _mode, _connectionToken);

                    if (ct.IsCancellationRequested)
                    {
                        await CleanupRunner();
                        return;
                    }

                    if (result.Ok)
                    {
                        OnReconnectStatus?.Invoke(null);
                        return;
                    }

                    // кімнати вже немає, далі пробувати марно
                    if (result.ShutdownReason == ShutdownReason.GameNotFound ||
                        result.ShutdownReason == ShutdownReason.GameClosed)
                    {
                        GoToTitle("Room is closed");
                        return;
                    }

                    // GameIsFull може бути, поки хост ще не зрозумів, що ми відпали, тому пробуємо далі
                }

                GoToTitle("Connection lost");
            }
            catch (OperationCanceledException)
            {
                // користувач натиснув Leave
            }
            finally
            {
                IsReconnecting = false;
            }
        }


        private void GoToTitle(string message)
        {
            _roomCode = null;
            _connectionToken = null;
            _disconnectMessage = message;

            OnReconnectStatus?.Invoke(null);
            SceneManager.LoadScene(_titleSceneIndex);
        }


        private static string GetDisconnectMessage(ShutdownReason reason)
        {
            return reason switch
            {
                ShutdownReason.Ok => null,
                ShutdownReason.DisconnectedByPluginLogic => "Host closed the room",
                ShutdownReason.PhotonCloudTimeout => "Connection timed out",
                _ => $"Disconnected: {reason}"
            };
        }


        // Повідомлення про розрив, яке TitleScreen показує один раз
        public string ConsumeDisconnectMessage()
        {
            string message = _disconnectMessage;
            _disconnectMessage = null;
            return message;
        }


        private async Task CleanupRunner()
        {
            _sessionActive = false;

            NetworkRunner runner = _runner;
            _runner = null;
            _matchCompletedHandler.ResetRunner();

            if (runner == null) // Unity-перевірка: true і для вже знищеного об'єкта
                return;

            if (runner.IsRunning)
                await runner.Shutdown(destroyGameObject: true);

            if (runner != null)
                Destroy(runner.gameObject);
        }


        // Лише для тестів: імітує втрату з'єднання з причиною ConnectionTimeout
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public void DebugSimulateConnectionLoss()
        {
            if (_runner != null)
                _ = _runner.Shutdown(true, ShutdownReason.ConnectionTimeout);
        }
    }
}