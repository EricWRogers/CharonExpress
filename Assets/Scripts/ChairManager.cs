using System;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public bool testBool;
    public GameObject[] chairs;
    [SerializeField] GameObject[] ghosts;
    int customerTotal = 0;
    String[] names = {"Sawyer", "Zek", "Cooper", "John", "Joe"};
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chairs = GameObject.FindGameObjectsWithTag("Chair");
        foreach (GameObject ghost in ghosts)
        {
            ghost.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        for (int i = 0; i < chairs.Length; i++)
        {
            if (chairs[i] != null)
            {
                chairs[i].GetComponent<ChairScript>().name = names[customerTotal % chairs.Length];
                if (testBool) 
                {
                    ghosts[i].SetActive(true);
                }
                else
                {
                    ghosts[i].SetActive(false);
                }
                Debug.Log(chairs[i]);
                customerTotal++;
            }
        }
    }
}
