using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace General
{
    public class Bootstrapper : MonoBehaviour
    {
        [SerializeField] private int _sceneIndex = 1;

        private const float LOAD_THRESHOLD = 0.9f;


        private void Start()
        {
            StartCoroutine(LoadSceneCoroutine());
        }


        private IEnumerator LoadSceneCoroutine()
        {
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(_sceneIndex);
            loadOp.allowSceneActivation = false;

            while (loadOp.progress < LOAD_THRESHOLD)
            {
                float progress = Mathf.Clamp01(loadOp.progress / LOAD_THRESHOLD);
                yield return null;
            }

            loadOp.allowSceneActivation = true;
        }
    }
}