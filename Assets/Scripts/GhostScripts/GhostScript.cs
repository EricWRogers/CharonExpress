using UnityEngine;
using System;

public class GhostScript : MonoBehaviour
{
    public GameObject ghostObject;
    public GameObject chairManager;
    public GameObject player;
    public ChairScript chairScript;
    public String ghostName = "";
    public String task = "";
    public float customerTimer;
    public float maxCustomerTimer;
    public float ratio;
    public GameObject InteractUI;
    public GameObject meter;
    public int chairIndex;

    public bool gamePaused;

    void Start()
    {
        player = GameObject.Find("Player");
        ghostObject = transform.GetChild(1).gameObject;
        chairManager = GameObject.Find("ChairManager");
        InteractUI = GameObject.Find("InteractUI");
        meter = transform.GetChild(1).gameObject;
        maxCustomerTimer = customerTimer;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (customerTimer > 0 && !gamePaused)
        {
            customerTimer-= Time.deltaTime;
            ratio = customerTimer / maxCustomerTimer;
            meter.transform.localScale = new Vector3(ratio, 0.2f, 1.0f);
        } 
        else if (customerTimer <= 0 && chairScript.ghostActive) 
        {
            chairScript.cooldownTimer = UnityEngine.Random.Range(5,10);
            chairScript.ghostActive = false;
            Debug.Log("I KILLED IT");
            Destroy(gameObject);
        }
    }

}
