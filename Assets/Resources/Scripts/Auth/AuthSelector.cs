using UnityEngine;


namespace Authentification
{
    public class AuthSelector : MonoBehaviour
    {
        private void Start()
        {
            AnonymAuth();
        }


        public void GooglePlayAuth()
        {
            Debug.Log("Google Play Games Auth");
            AuthHandler.Instance.SignInWithGPGS();
        }


        public void AnonymAuth()
        {
            Debug.Log("Anonym Auth");
            AuthHandler.Instance.SignInAnonymously();
        }
    }
}