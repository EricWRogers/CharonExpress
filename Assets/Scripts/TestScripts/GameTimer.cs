using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public Slider timerBar;
    public float sliderTimer;
    private float currentTime;
    public bool stopTimer = false;
    public void StartGameTimer()
    {
        timerBar.maxValue = sliderTimer;
        timerBar.value = sliderTimer;
        currentTime = sliderTimer;

        StartCoroutine(timerstart());
    }
    IEnumerator timerstart()
    {

        while (stopTimer == false)
        {
            currentTime -= Time.deltaTime;
            yield return new WaitForSeconds(0.001f);

            if (currentTime <= 0)
            {
                //lose game
                stopTimer = true;
            }

            if (stopTimer == false)
            {
                timerBar.value = currentTime;
            }
        }
    }
}
