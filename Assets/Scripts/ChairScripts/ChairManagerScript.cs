using System;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

public class ChairManager : MonoBehaviour
{
    public static ChairManager Instance;

    public static ChairManager GetInstance()
    {
        return Instance;
    }
    public bool testBool;
    public GameObject[] chairs;
    public GameObject ghostPrefab;
    //escortChair marks which chair object will be left open for the "escort" task
    public int escortChair;
    public bool escortAssigned = false;

    //Keeps track of how many ghosts have been through in total
    public int customerTotal = 0;
    public int deathTotal = 0;
    public int activeGhosts = 0;
    public int maxGhostCount = 2;
    //customerTotal modulo by the length of chairs

    //Sample list of names that is assigned to the chair/ghost
    String[] names = {"Sawyer", "Zek", "Cooper", "John", "Joe"};
    public DialogGraph[] tasks = {};

    public void Awake()
    {
        if (ChairManager.Instance != this && ChairManager.Instance != null)
        {
            Destroy(ChairManager.Instance);
            Instance = this;
        }
        else
        {
            Instance = this;
        }

    }

    void Start()
    {
        //Grab list of all chairs
        chairs = GameObject.FindGameObjectsWithTag("Chair");
        //Determine one chair to be the "reserved" chair for the escort task
        escortChair = UnityEngine.Random.Range(0, chairs.Length);

        //For all unreserved chairs, assign a delay for the ghosts to show up
        for (int i = 0; i < chairs.Length; i++)
        {
            if (i == escortChair)
            {
                chairs[i].GetComponent<ChairScript>().reserved = true;
            }
            else
            {
                chairs[i].GetComponent<ChairScript>().reserved = false;
                chairs[i].GetComponent<ChairScript>().cooldownTimer = UnityEngine.Random.Range(1,5);
            }
        }
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        //Iterate through each chair by index
        for (int i = 0; i < chairs.Length; i++) {

            //If the chair is not reserved for escort, decrement cooldown. If cooldown below zero, instantiate a ghost and assign it a task
            if (i != escortChair)
            {     
                ChairScript chairScript = chairs[i].GetComponent<ChairScript>();
                if (chairScript.cooldownTimer > 0)
                {
                    chairScript.cooldownTimer-= Time.deltaTime;
                } 
                else if (chairScript.cooldownTimer <= 0 && chairScript.ghostActive == false && activeGhosts < maxGhostCount)
                {
                    activeGhosts++;
                    chairScript.ghostObject = Instantiate(chairScript.ghostPrefab, chairScript.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                    chairScript.ghostActive = true;
                    GhostScript ghostScript = chairScript.ghostObject.GetComponent<GhostScript>();
                    ghostScript.chairScript = chairScript;
                    ghostScript.customerTimer = UnityEngine.Random.Range(45,60);
                    AssignGhost(chairScript.ghostObject);
                }
            }
        }
    }

    //Assign a task to the ghost passed in
    public void AssignGhost(GameObject ghost)
    {
        customerTotal++;
        Dialogue chairScript = ghost.GetComponent<Dialogue>();
        DialogGraph taskToAssign = tasks[UnityEngine.Random.Range(0, tasks.Length)];
        Debug.Log("To the task assigned was this" + taskToAssign.name);
        if (taskToAssign.name == "EscortGraph" && !escortAssigned)
        {
            escortAssigned = true;
        }
        else if (taskToAssign.name == "EscortGraph" && escortAssigned)
        {
            while (taskToAssign.name == "EscortGraph")
            {
                taskToAssign = tasks[UnityEngine.Random.Range(0, tasks.Length)];
            }
        }
        chairScript.SetDialogGraph(taskToAssign);
        Debug.Log("Assigned the task" + taskToAssign + " count: " + customerTotal);
        GameManager.Instance.ghosts.Add(ghost);
    }

    //Called when an escort task is completed. Determines a new, empty chair to be reserved.
    public void FindNewEscortChair()
    {
        chairs[escortChair].GetComponent<ChairScript>().reserved = false;
        for (int i = 0; i < chairs.Length; i++)
        {
            ChairScript tempChairScript = chairs[i].GetComponent<ChairScript>();
            if ( tempChairScript.ghostActive != true)
            {
                escortChair = i;
                tempChairScript.reserved = true;
            }
        }
    }


    public void NewDay(int day)
    {
        activeGhosts = 0;
        if (day % 2 == 0) maxGhostCount += UnityEngine.Random.Range(1,2);
        //Day Timer is extended within the EndDayUI script on odd days
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (GameObject ghost in ghosts)
        {
            Destroy(ghost);
        }
        for (int i = 0; i < chairs.Length; i++)
        {
            ChairScript tempChairScript = chairs[i].GetComponent<ChairScript>();
            tempChairScript.ghostActive = false;
            //If the chair is not reserved for escort, decrement cooldown. If cooldown below zero, instantiate a ghost and assign it a task
            if (i != escortChair && activeGhosts < maxGhostCount)
            {                         
                activeGhosts++;
                tempChairScript.ghostObject = Instantiate(tempChairScript.ghostPrefab, tempChairScript.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                tempChairScript.ghostActive = true;
                GhostScript ghostScript = tempChairScript.ghostObject.GetComponent<GhostScript>();
                ghostScript.chairScript = tempChairScript;
                ghostScript.customerTimer = UnityEngine.Random.Range(45,60);
                AssignGhost(tempChairScript.ghostObject);                
            }
        }
    }
}
