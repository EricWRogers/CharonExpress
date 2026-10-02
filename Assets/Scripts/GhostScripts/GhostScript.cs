using UnityEngine;
using System;
using UnityEngine.Analytics;

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

    static public bool gamePaused;


    //Variables for Escort Mission
    public bool beingEscorted = false;
    public bool playerInRange = false;
    public int direction = 1;  //1 represents target is to the right, -1 represents target is to the left
    public int movementSpeed = 5;

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

        if (beingEscorted && playerInRange)
        {
            travelStep();
        }
    }

    public void assignedEscort(Vector3 targetPosition)
    {
        beingEscorted = true;
        if (targetPosition.x < transform.position.x)
        {
            direction = -1;
        }
        else
        {
            direction = 1;
        } 
        transform.position = new Vector3(transform.position.x, transform.position.y, -2.5f);
    }
    void travelStep()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < 10f)
        {
            transform.position += new Vector3(Time.deltaTime * movementSpeed * direction, 0, 0);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<player>() != null)
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.GetComponent<player>() != null)
        {
            playerInRange = false;
        }
    }
}