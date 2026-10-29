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

}