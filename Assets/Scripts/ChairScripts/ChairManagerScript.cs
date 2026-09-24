using System;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

public class ChairManager : MonoBehaviour
{
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


    void Start()
    {
        chairs = GameObject.FindGameObjectsWithTag("Chair");
        UnityEngine.Random.Range(0, chairs.Length);
        //Hides the ghosts. They will be toggled on when appropriate.
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        foreach (GameObject chair in chairs)
        {
            ChairScript chairScript = chair.GetComponent<ChairScript>();
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
                ghostScript.customerTimer = UnityEngine.Random.Range(5,10);
                Debug.Log("I GAVE IT LIFE");
                AssignGhost(chairScript.ghostObject);
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
