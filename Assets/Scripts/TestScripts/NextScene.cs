using UnityEngine;

public class NextScene : MonoBehaviour
{
    public void LoadNextScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
