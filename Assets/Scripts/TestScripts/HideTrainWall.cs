using System.Collections.Generic;
using UnityEngine;

public class HideTrainWall : MonoBehaviour
{
    public List<GameObject> wallsToDelete;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject wall in wallsToDelete)
            {
                if (wall != null)
                {
                    wall.SetActive(false);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject wall in wallsToDelete)
            {
                if (wall != null)
                {
                    wall.SetActive(true);
                }
            }
        }
    }
}