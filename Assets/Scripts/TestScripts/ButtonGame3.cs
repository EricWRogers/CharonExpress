using UnityEngine;

public class ButtonGame3 : MonoBehaviour
{
    public bool gameRunning = false;
    public GameObject gameUI;
    public GameObject timer;
    public GameObject player;
    public float TimerTime = 10f;
    public GameTimer timerscript;
    public QuestController questController;
    public string gameID = "ButtonGame3";

    public void StartGame()
    {
        
        timerscript.sliderTimer = TimerTime;
        timerscript.StartGameTimer();

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

            WinGame();
            player.GetComponent<player>().freeze = false;
        }
    }
    public void WinGame()
    {
    QuestController.Instance.CompleteMicrogame(gameID);
    }
}
