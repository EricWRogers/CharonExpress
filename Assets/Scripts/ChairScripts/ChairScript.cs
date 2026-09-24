using System;
using UnityEngine;
using UnityEngine.Rendering;


public class ChairScript : MonoBehaviour
{
    public GameObject chairManager;
    public GameObject ghostPrefab;
    public GameObject player;
    ChairManager chairManagerScript;
    public String ghostName = "";
    public bool ghostActive = false;
    public String task = "";
    float ratio;
    public float cooldownTimer;
    public GameObject ghostObject;
    public GameObject InteractUI;
    public GameObject meter;
    void Start()
    {
        player = GameObject.Find("Player");
        cooldownTimer = UnityEngine.Random.Range(1, 5);
        chairManager = GameObject.Find("ChairManager");
        InteractUI = GameObject.Find("InteractUI");
        chairManagerScript = chairManager.GetComponent<ChairManager>();
    }

//
//
//
//
//
//          NOTICE!!!!
//          This script serves as a container, processing on instantiation happens in ChairMangerScript.
//          For the ghost's individual timer, see GhostScript
//          NOTICE!!!!
//
//
//
    void FixedUpdate()
    {}
}
