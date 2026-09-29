using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WordObject", menuName = "Scriptable Objects/WordObject")]
public class WordObject : ScriptableObject
{
    public string word = "";
    public List<string> letters;
}
