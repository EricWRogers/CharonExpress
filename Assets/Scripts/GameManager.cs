using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static GameManager GetInstance()
    {
        return Instance;
    }
    public float maxScore = 5;
    public float avgScore = 1;
    public float ghostsScore = 0;
    public int index;
    public string letterGrade = "";
    public List<GameObject> ghosts = new List<GameObject>();

    public void Awake()
    {
        if (GameManager.Instance != this && GameManager.Instance != null)
        {
            Destroy(GameManager.Instance);
            Instance = this;
        }
        else
        {
            Instance = this;
        }

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
    public void SetGrade()
    {
        ghostsScore = 0;
        index = 0;
        if (ghosts != null)
        {
            for (int i = 0; i <= ghosts.Count; i++)
            {
                if (i != 0)
                {
                    Debug.Log(ghosts.Count);
                    if (ghosts[index] != null)
                    {
                        ghosts[index].GetComponent<GhostScript>().Pattience();
                        ghostsScore += ghosts[index].GetComponent<GhostScript>().score;
                        index++;
                    }
                    else if (ghosts[index] == null)
                    {
                        index++;
                    }
                    
                    
                }
            }
            avgScore = ghostsScore / ghosts.Count;
            if (avgScore <= 2)
            {
                letterGrade = "F";
            }
            else if (avgScore > 2 && avgScore <= 4)
            {
                letterGrade = "D";
            }
            else if (avgScore > 4 && avgScore <= 6)
            {
                letterGrade = "C";
            }
            else if (avgScore > 6 && avgScore <= 8)
            {
                letterGrade = "B";
            }
            else if (avgScore > 8 && avgScore <= 10)
            {
                letterGrade = "A";
            }
            else if (avgScore > 10)
            {
                letterGrade = "S";
            }
            ghosts = new List<GameObject>();
        }
    }
}
