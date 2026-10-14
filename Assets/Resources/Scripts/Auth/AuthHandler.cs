using Firebase.Auth;
using System;
using UnityEngine;


namespace Authentification
{
    public enum AuthType { Anonymous, PlayGames }


    public class AuthHandler : MonoBehaviour
    {
        public static AuthHandler Instance { get; private set; }


        private void Awake()
        {
            if (Instance != null)
                Destroy(this);
            else
                Instance = this;
        }


        private AuthProviderBase CreateProvider(AuthType type)
        {
            switch (type)
            {
                case AuthType.PlayGames: return new PlayGamesProvider();
                case AuthType.Anonymous:
                default: return new AnonymoslyProvider();
            }
        }


        public void SignIn(AuthType type, Action<FirebaseUser> onSuccess = null, Action<string> onError = null)
        {
            FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

            if (user != null)
            {
                onSuccess?.Invoke(user);
                return;
            }

            AuthProviderBase provider = CreateProvider(type);
            provider.SignIn(user =>
            {
                Debug.Log($"Signed in: {user.UserId}");
                onSuccess?.Invoke(user);
            }, error =>
            {
                Debug.LogError(error);
                onError?.Invoke(error);
            });
        }


        public void SignInAnonymously()
        {
            SignIn(AuthType.Anonymous, null, null);
        }


        public void SignInWithGPGS()
        {
            SignIn(AuthType.PlayGames, null, null);
        }
    }
}