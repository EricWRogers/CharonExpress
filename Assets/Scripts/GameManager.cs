using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static GameManager GetInstance()
    {
        return Instance;
    }
    float maxScore = 0;
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
}
