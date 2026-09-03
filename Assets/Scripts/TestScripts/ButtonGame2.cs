using UnityEngine;

public class ButtonGame2 : MonoBehaviour
{
    public bool gameRunning = false;
    public GameObject gameUI;
    public GameObject timer;
    public float TimerTime = 100f;
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
