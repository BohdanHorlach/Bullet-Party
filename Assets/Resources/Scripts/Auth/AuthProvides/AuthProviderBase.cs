using Firebase.Auth;
using System;


namespace Authentification
{
    public abstract class AuthProviderBase
    {
        protected FirebaseAuth _auth;

        public AuthProviderBase()
        {
            _auth = FirebaseAuth.DefaultInstance;
        }


        public abstract void SignIn(Action<FirebaseUser> onSuccess, Action<string> onError);
    }
}