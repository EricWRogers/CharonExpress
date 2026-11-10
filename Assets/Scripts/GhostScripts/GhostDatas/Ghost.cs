using UnityEngine;


[CreateAssetMenu(menuName = "Ghosts/Ghost")]
public class Ghost : ScriptableObject
{
    [Header("Ghost")]
    [Tooltip("The name of this ghost")]
    public string ghostName;

    [Header("Sprite")]
    [Tooltip("Idle sprite for the ghost")]
    public Sprite idleSprite;
    public Sprite floatSprite;
    public Sprite profileSprite;

    [Header("Dialogue Graph")]
    [Tooltip("this is the dialogue for the ghost")]
    public DialogGraph dialogGraph;
    public bool needExtraTask = false;

}