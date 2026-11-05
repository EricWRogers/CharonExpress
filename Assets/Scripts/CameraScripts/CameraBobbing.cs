using UnityEngine;

public class CameraBobbing : MonoBehaviour
{
    int bobCooldown = 30;
    void Start()
    {
        
    }
    void FixedUpdate()
    {
        if (GhostScript.gamePaused == false)
        {
            bobCooldown--;
            if (bobCooldown <=0)
            {
                Debug.Log("Bobbed");
                TweenHelper.CameraBob(transform);
                bobCooldown = UnityEngine.Random.Range(50, 100);
            }
        }
    }
}
