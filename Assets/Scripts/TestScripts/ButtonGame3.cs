using UnityEngine;

public class ButtonGame3 : MonoBehaviour
{
    public bool gameRunning = false;
    public GameObject gameUI;
    public GameObject timer;
    public float TimerTime = 1f;

    public void StartGame()
    {
        timer.SetActive(true);
        gameRunning = true;
    }

    public void ButtonPress()
    {
        if (gameRunning)
        {
            gameRunning = false;
            gameUI.SetActive(false);
            timer.SetActive(false);
        }
    }
}
