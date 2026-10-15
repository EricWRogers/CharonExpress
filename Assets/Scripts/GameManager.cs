using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    float maxScore = 0;
    public List<GameObject> ghosts = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindGameObjectWithTag("Ghost") != null)
        {
            ghosts.Add(GameObject.FindGameObjectWithTag("Ghost"));
        }
    }
}
