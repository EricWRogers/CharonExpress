using System;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public String[] chairs;
    int customerTotal = 0;
    String[] names = {"Sawyer", "Zek", "Cooper", "John", "Joe"};
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chairs = new String[4];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        for (int i = 0; i < chairs.Length; i++)
        {
            if (chairs[i] == null)
            {
                chairs[i] = names[customerTotal];
            }
        }
        Debug.Log(chairs);
    }
}
