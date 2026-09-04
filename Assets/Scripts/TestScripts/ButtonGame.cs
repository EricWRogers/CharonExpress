using UnityEngine;

public class ButtonGame : MonoBehaviour
{
    public bool gameRunning = false;
    public GameObject gameUI;
    public GameObject timer;
    public GameObject player;
    public float TimerTime = 10f;
    public void StartGame()
    {
        gameRunning = true;
        player.GetComponent<player>().freeze = true;
    }

    public void ButtonPress()
    {
        if (gameRunning)
        {
            gameRunning = false;
            gameUI.SetActive(false);
            timer.SetActive(false);
            player.GetComponent<player>().freeze = false;
        }
    }
}
