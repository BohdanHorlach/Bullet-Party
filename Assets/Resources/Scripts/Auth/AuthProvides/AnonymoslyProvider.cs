using Firebase.Auth;
using System;


namespace Authentification
{
    public class AnonymoslyProvider : AuthProviderBase
    {
        public override void SignIn(Action<FirebaseUser> onSuccess, Action<string> onError)
        {
            _auth.SignInAnonymouslyAsync().ContinueWith(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    onError?.Invoke("Anonymous Auth failed: " + task.Exception);
                    return;
                }

                FirebaseUser user = task.Result.User;
                onSuccess?.Invoke(user);
            });
        }
    }
}