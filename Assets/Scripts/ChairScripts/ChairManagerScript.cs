using System;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

public class ChairManager : MonoBehaviour
{
    public bool testBool;
    public GameObject[] chairs;
    [SerializeField] GameObject[] ghosts;

    //Keeps track of how many ghosts have been through in total
    public int customerTotal = 0;
    //customerTotal modulo by the length of chairs
    int customerModulo;

    //Sample list of names that is assigned to the chair/ghost
    String[] names = {"Sawyer", "Zek", "Cooper", "John", "Joe"};
    String[] tasks = {"Button", "Interact", "Zone"};


    void Start()
    {
        chairs = GameObject.FindGameObjectsWithTag("Chair");

        //Hides the ghosts. They will be toggled on when appropriate.
        foreach (GameObject ghost in ghosts)
        {
            ghost.SetActive(false);
        }
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
    }

    public void AssignGhost(GameObject chair)
    {
        customerTotal++;
        ChairScript chairScript = chair.GetComponent<ChairScript>();
        string taskToAssign = tasks[UnityEngine.Random.Range(0, tasks.Length)];
        chairScript.task = taskToAssign;
        Debug.Log("Assigned the task" + taskToAssign + " count: " + customerTotal);
    }
}
