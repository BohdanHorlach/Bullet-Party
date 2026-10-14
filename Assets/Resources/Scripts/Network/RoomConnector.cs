using Fusion;
using General.Utils;
using System;
using System.Threading.Tasks;
using UI.AlertSystem;
using UI.Input;
using UnityEngine;


namespace Network
{
    public class RoomConnector : MonoBehaviour
    {
        [SerializeField] private InputFieldValidator _roomCodeField;
        [SerializeField] private int _lobbyIndex = 2;

        private const int MAX_RECREATE_RETRIES = 5;

        private bool _isConnecting;

        public Action<string> OnError;


        private void Start()
        {
            // якщо ми повернулися на TitleScreen через розрив сесії
            string message = NetworkBootstrap.Instance != null
                ? NetworkBootstrap.Instance.ConsumeDisconnectMessage()
                : null;

            if (!string.IsNullOrEmpty(message))
                ShowError(message);
        }


        private Task<StartGameResult> Connect(string roomCode, GameMode mode)
        {
            SceneRef scene = SceneRef.FromIndex(_lobbyIndex);
            return NetworkBootstrap.Instance.StartGame(roomCode, scene, mode);
        }


        private async Task TryConnect(string roomCode, GameMode mode = GameMode.Client)
        {
            StartGameResult result = await Connect(roomCode, mode);

            if (result.Ok)
                return;

            if (mode == GameMode.Host)
            {
                int retries = 0;

                while (result.ShutdownReason == ShutdownReason.GameIdAlreadyExists && retries < MAX_RECREATE_RETRIES)
                {
                    retries++;

                    roomCode = CodeGenerator.GenerateCode();
                    result = await Connect(roomCode, mode);

                    if (result.Ok)
                        return;
                }

                throw new Exception("Failed to create room. Please try again.");
            }

            string error = result.ShutdownReason switch
            {
                ShutdownReason.GameNotFound => "Room not found",
                ShutdownReason.GameIsFull => "Room is full",
                _ => $"Failed to connect {result.ErrorMessage}"
            };

            throw new Exception(error);
        }


        // calls from UI buttons
        public async void GenerateRoom()
        {
            if (_isConnecting)
                return;

            _isConnecting = true;

            try
            {
                await TryConnect(CodeGenerator.GenerateCode(), GameMode.Host);
            }
            catch (Exception error)
            {
                ShowError(error.Message);
            }
            finally
            {
                _isConnecting = false;
            }
        }


        // calls from UI buttons
        public async void Connect()
        {
            if (_isConnecting)
                return;

            _isConnecting = true;

            try
            {
                if (_roomCodeField.TryGetInput(out string roomCode))
                {
                    await TryConnect(roomCode);
                }
                else
                {
                    ShowError("Invalid room code");
                }
            }
            catch (Exception error)
            {
                ShowError(error.Message);
            }
            finally
            {
                _isConnecting = false;
            }
        }


        public void TestPreload()
        {
            _ = NetworkBootstrap.Instance.LoadNextScene(3);
        }


        public void TestActivateScene()
        {
            _ = NetworkBootstrap.Instance.SetNextScene(3);
        }


        private void ShowError(string message)
        {
            OnError?.Invoke(message);
            AlertsSystem.Instance.InvokeAlert(message, false, AlertStatus.Error);
        }
    }
}