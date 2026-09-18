using System;
using UnityEngine;
using UnityEngine.Rendering;

//The goal of this script is to serve as the "container" for the chair and store the data that pertains to that chair and that chair only
//For example, managing the timer on how long the ghost will be present, what task it will have, etc.
//It is easier to have it managed by a constant chair script instead of a ghost script that moves around the scene.


public class ChairScript : MonoBehaviour
{
    public GameObject chairManager;
    public GameObject ghostPrefab;
    public GameObject player;
    ChairManager chairManagerScript;
    public String ghostName = "";
    public bool ghostActive = false;
    public String task = "";
    float ratio;
    public float cooldownTimer;
    public GameObject ghostObject;
    public GameObject InteractUI;
    public GameObject meter;
    void Start()
    {
        player = GameObject.Find("Player");
        cooldownTimer = UnityEngine.Random.Range(1, 5);
        chairManager = GameObject.Find("ChairManager");
        InteractUI = GameObject.Find("InteractUI");
        chairManagerScript = chairManager.GetComponent<ChairManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer-= Time.deltaTime;
        } 
        else if (cooldownTimer <= 0 && ghostActive == false)
        {
            ghostObject = Instantiate(ghostPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            Debug.Log(ghostObject.name);
            ghostActive = true;
            Debug.Log(ghostObject.GetComponent<GhostScript>());
            GhostScript ghostScript = ghostObject.GetComponent<GhostScript>();
            ghostScript.chairScript = gameObject.GetComponent<ChairScript>();
            ghostScript.customerTimer = UnityEngine.Random.Range(5,10);
            Debug.Log("I GAVE IT LIFE");
            chairManagerScript.AssignGhost(ghostObject);
        }
    }
}
