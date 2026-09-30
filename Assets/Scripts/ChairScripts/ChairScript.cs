using System;
using Mono.Cecil.Cil;
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

    void Start()
    {
    }

    void OnTriggerEnter(Collider collider)
    {
        if (reserved && collider?.GetComponent<GhostScript>())
        {
            GhostScript ghostScript = collider.GetComponent<GhostScript>();
            ghostScript.beingEscorted = false;
            collider.gameObject.transform.position = transform.position + new Vector3(0, 1, 0);
        }
    }
}
