using UnityEngine;

public class PuddleScript : MonoBehaviour
{
    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.name == "Player")
        {
            player player = collider.gameObject.GetComponent<player>();
            player.slowedCoefficient = 0.3f;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.name == "Player")
        {
            player player = collider.gameObject.GetComponent<player>();
            player.slowedCoefficient = 1.0f;
        }
    }

}
