using Firebase.Auth;
//using GooglePlayGames;
//using GooglePlayGames.BasicApi;
using System;


namespace Authentification
{
    public class PlayGamesProvider : AuthProviderBase
    {
        public override void SignIn(Action<FirebaseUser> onSuccess, Action<string> onError)
        {
//#if UNITY_ANDROID
//            PlayGamesPlatform.Instance.Authenticate((status) =>
//            {
//                if (status != SignInStatus.Success)
//                {
//                    onError?.Invoke("Authenticate Failed");
//                    return;
//                }

//                PlayGamesPlatform.Instance.RequestServerSideAccess(false, authCode =>
//                {
//                    FirebaseAuth auth = FirebaseAuth.DefaultInstance;
//                    Credential credential = PlayGamesAuthProvider.GetCredential(authCode);
//                    auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
//                    {
//                        if (task.IsCanceled || task.IsFaulted)
//                            onError?.Invoke($"Failed sign in with credential: {task.Exception}");
//                        else
//                            onSuccess?.Invoke(task.Result);
//                    });
//                });
//            });
//#else
//            onError?.Invoke("Play Games is supported only on Android");
//#endif
        }
    }
}