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

    //Keeps track of how many ghosts have been through in total
    public int customerTotal = 0;
    //public int chair
    //customerTotal modulo by the length of chairs
    int customerModulo;

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
        chairs = GameObject.FindGameObjectsWithTag("Chair");
        escortChair = UnityEngine.Random.Range(0, chairs.Length);
        //Hides the ghosts. They will be toggled on when appropriate.

        for (int i = 0; i < chairs.Length; i++)
        {
            if (i == escortChair)
            {
                chairs[i].GetComponent<ChairScript>().reserved = true;
            }
            else
            {
                chairs[i].GetComponent<ChairScript>().reserved = false;
                chairs[i].GetComponent<ChairScript>().cooldownTimer = UnityEngine.Random.Range(3,40);
            }
        }
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        for (int i = 0; i < chairs.Length; i++) {
            if (i != escortChair)
            {     
                ChairScript chairScript = chairs[i].GetComponent<ChairScript>();
                if (chairScript.cooldownTimer > 0)
                {
                    chairScript.cooldownTimer-= Time.deltaTime;
                } 
                else if (chairScript.cooldownTimer <= 0 && chairScript.ghostActive == false)
                {
                    chairScript.ghostObject = Instantiate(chairScript.ghostPrefab, chairScript.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                    Debug.Log(chairScript.ghostObject.name);
                    chairScript.ghostActive = true;
                    Debug.Log(chairScript.ghostObject.GetComponent<GhostScript>());
                    GhostScript ghostScript = chairScript.ghostObject.GetComponent<GhostScript>();
                    ghostScript.chairScript = chairScript;
                    ghostScript.customerTimer = UnityEngine.Random.Range(15,20);
                    Debug.Log("I GAVE IT LIFE");
                    AssignGhost(chairScript.ghostObject);
                }
            }
        }
    }

    public void AssignGhost(GameObject chair)
    {
        customerTotal++;
        Dialogue chairScript = chair.GetComponent<Dialogue>();
        DialogGraph taskToAssign = tasks[UnityEngine.Random.Range(0, tasks.Length)];
        chairScript.SetDialogGraph(taskToAssign);
        Debug.Log("Assigned the task" + taskToAssign + " count: " + customerTotal);
    }
}
