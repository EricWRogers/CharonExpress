using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DialogueActionSetup : MonoBehaviour
{
    [SerializeField] private DialogueAction action;
    [SerializeField] private UnityEvent onInvoke = new UnityEvent();

    private IEnumerator Start()
    {
        Dialogue dialogue = null;

        // Wait until the Dialogue prefab has spawned.
        while (dialogue == null)
        {
            dialogue = FindFirstObjectByType<Dialogue>();

            if (dialogue == null)
                yield return null;
        }

        dialogue.RegisterAction(action, onInvoke.Invoke);
    }
}