using System;
using Unity.VisualScripting;
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

    public bool reserved = false;
    Collider[] hitColliders;
    public float radius = 1f;
    public Ray rayed;
    public bool hit;

    void Start()
    {
    }
    void Update()
    {
        //GetGhosty();
    }

/*
    void GetGhosty()
    {
        Physics.SphereCast(rayed, radius);
        foreach (var collider in hitColliders)
        {
            Debug.Log(collider.gameObject.tag);
            if (reserved && collider.CompareTag("Ghost"))
            {
                Debug.Log("ghost is here");
                GhostScript ghostScript = collider.GetComponent<GhostScript>();
                ghostScript.beingEscorted = false;
                collider.gameObject.transform.position = transform.position + new Vector3(0, 1, 0);
            }
            else
            {
                return;
            }
        }
    }
    */

}
