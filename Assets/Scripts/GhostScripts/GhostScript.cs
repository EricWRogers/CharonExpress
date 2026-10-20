using UnityEngine;
using System;
using UnityEngine.Analytics;
using System.Diagnostics.CodeAnalysis;
using UnityEngine.Rendering;

public class GhostScript : MonoBehaviour
{
    public GameObject ghostObject;
    public ChairManager chairManager;
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
    public string gameID = "escort";

    public Material normalMaterial;
    public Material highlightMaterial;
    Renderer rend;

    static public bool gamePaused;


    //Variables for Escort Mission
    public bool beingEscorted = false;
    public bool playerInRange = false;
    public int direction = 1;  //1 represents target is to the right, -1 represents target is to the left
    public int movementSpeed = 10;
    public Vector3 targetPosition;
    public int maxScore = 10;
    public int score = 5;

    void Start()
    {
        player = GameObject.Find("Player");
        ghostObject = transform.GetChild(1).gameObject;
        chairManager = ChairManager.Instance;
        InteractUI = GameObject.Find("InteractUI");
        meter = transform.GetChild(1).gameObject;
        maxCustomerTimer = customerTimer;
        rend = GetComponent<Renderer>();
    }

    void FixedUpdate()
    {
        //Decrement the ghost's lifespan. If lifespan is elapsed, kill the ghost and set a new cooldown timer for it
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
            ChairManager.Instance.deathTotal++;
            Debug.Log("I KILLED IT");
            Destroy(gameObject);
        }

        if (beingEscorted)
        {
            travelStep();
        }
    }

    //When the escort task is assigned, determine the position and direction needed to go, then move the ghost to the aisle.
    public void assignedEscort(Vector3 inputTargetPosition)
    {
        targetPosition = inputTargetPosition;   
        beingEscorted = true;
        if (targetPosition.x < transform.position.x)
        {
            direction = -1;
        }
        else
        {
            direction = 1;
        } 
        transform.position = new Vector3(transform.position.x, transform.position.y, -0.5f);
    }

    //Called every FixedUpdate during escort mission
    void travelStep()
    {
        //If player is in-range, move the ghost toward its destination
        if (Vector3.Distance(transform.position, player.transform.position) < 2f)
        {
            transform.position += new Vector3(Time.deltaTime * movementSpeed * direction, 0, 0);
        }

        //If the ghost has reached its destination, reset variables, move the ghost to its proper spot, and end the task
        if ((direction == 1 && transform.position.x > targetPosition.x) || (direction == -1 && transform.position.x < targetPosition.x))
        {

            beingEscorted = false;
            chairScript.ghostActive = false;
            chairScript = ChairManager.Instance.chairs[ChairManager.Instance.escortChair].GetComponent<ChairScript>();
            chairScript.ghostActive = true;
            

            transform.position = chairScript.gameObject.transform.position + new Vector3(0, 1, 0); 
            ChairManager.Instance.FindNewEscortChair();
            Debug.Log("GhostScriptDebugStatement");
            QuestController.Instance.CompleteMicrogame(gameID);
            customerTimer += maxCustomerTimer / 3;
            maxCustomerTimer = customerTimer;
            foreach (MicroGameStart gameStart in
                FindObjectsByType<MicroGameStart>(FindObjectsSortMode.None))
            {
                if (gameStart.gameID == gameID)
                {
                    gameStart.ResetInteraction();
                    break;
                }
            }
        }
        else {
            return;
        }
    }

    public void EnableHighlight()
    {
        rend.material = highlightMaterial;
        return;
    }

    public void DisableHighlight()
    {
        rend.material = normalMaterial;
    }

}