using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using static UnityEngine.InputSystem.InputAction;

public class EndOfDemo : MonoBehaviour
{
    public string sceneName;
    public GameObject MainMenuButton;
    
    public void Start()
    {
        EventSystem.current.SetSelectedGameObject(MainMenuButton);
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
