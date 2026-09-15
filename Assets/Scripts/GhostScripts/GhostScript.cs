using UnityEngine;
using System;

public class GhostScript : MonoBehaviour
{
    public GameObject ghostObject;
    public GameObject chairManager;
    public GameObject player;
    public ChairScript chairScript;
    public String ghostName = "";
    public bool ghostActive = false;
    public String task = "";
    public float customerTimer;
    public float maxCustomerTimer;
    float ratio;
    public GameObject InteractUI;
    public GameObject meter;
    void Start()
    {
        player = GameObject.Find("Player");
        customerTimer = 0;
        ghostObject = transform.GetChild(1).gameObject;
        chairManager = GameObject.Find("ChairManager");
        InteractUI = GameObject.Find("InteractUI");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (customerTimer > 0)
        {
            customerTimer-= Time.deltaTime;
            ratio = customerTimer / maxCustomerTimer;
            meter.transform.localScale = new Vector3(ratio, 1.0f, 1.0f);
        } 
        else if (customerTimer <= 0 && ghostActive == true) 
        {
            chairScript.cooldownTimer = UnityEngine.Random.Range(5,10);
            chairScript.ghostActive = false;
            Debug.Log("I KILLED IT");
            Destroy(gameObject);
        }
    }

}
