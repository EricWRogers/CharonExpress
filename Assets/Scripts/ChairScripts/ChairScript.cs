using System;
using UnityEngine;
using UnityEngine.Rendering;

//The goal of this script is to serve as the "container" for the chair and store the data that pertains to that chair and that chair only
//For example, managing the timer on how long the ghost will be present, what task it will have, etc.
//It is easier to have it managed by a constant chair script instead of a ghost script that moves around the scene.


public class ChairScript : MonoBehaviour
{
    public GameObject chairManager;
    ChairManager chairManagerScript;
    public String ghostName = "";
    public bool ghostActive = false;
    public String task = "";
    public int customerTimer;
    public int cooldownTimer;
    public GameObject ghostObject;
    void Start()
    {
        cooldownTimer = UnityEngine.Random.Range(10, 500);
        customerTimer = 0;
        ghostObject = transform.GetChild(1).gameObject;
        chairManager = GameObject.Find("ChairManager");
        chairManagerScript = chairManager.GetComponent<ChairManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (customerTimer > 0)
        {
            customerTimer--;
        } 
        else if (cooldownTimer > 0)
        {
            cooldownTimer--;
        } 
        else if (cooldownTimer == 0 && ghostActive == false)
        {
            ghostActive = true;
            ghostObject.SetActive(true);
            Debug.Log("I GAVE IT LIFE");
            chairManagerScript.AssignGhost(gameObject);
            customerTimer = UnityEngine.Random.Range(500, 2000);
        }
        else if (customerTimer == 0 && ghostActive == true) 
        {
            ghostActive = false;
            ghostObject.SetActive(false);
            Debug.Log("I KILLED IT");
            cooldownTimer = UnityEngine.Random.Range(1000,500);
        } 
    }

/*   bool CheckTasks()
    {
        if (task != null)
        {
            if (task == "Interact")
            {
                return true;
            }
        }
    }
    */
}
