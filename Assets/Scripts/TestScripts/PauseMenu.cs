using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using static UnityEngine.InputSystem.InputAction;

public class pauseMenu : MonoBehaviour
{
    public bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject pausebutton;

    public string sceneName;
    public PlayerThings inputActions;

    private void Awake()
    {
        inputActions = new PlayerThings();

        inputActions.Player.Pause.performed += OnPause;

    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
    // Update is called once per frame
    void Update()
    {

        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }*/

    }

    public void Resume()
    {

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

    }

    void Pause()
    {

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        EventSystem.current.SetSelectedGameObject(pausebutton);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);


    }
    public void QuitGame()
    {
        Debug.Log("quit");
        Application.Quit();
    }
    public void credits()
    {
        pauseMenuUI.SetActive(true);
    }
    public void exitCredits()
    {
        pauseMenuUI.SetActive(false);
    }
    public void OnPause(CallbackContext ctx)
    {
        if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
    }
}
