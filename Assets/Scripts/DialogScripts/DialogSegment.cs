using UnityEngine;
using System;
using XNode;
using System.Collections.Generic;
using TMPro;


[Serializable]
public struct Connection {}
public class DialogSegment : Node
{
    public Sprite portrait;

    public string speakerName;
    
    [Input]
    public Connection input;

    public string[] DialogText;

     public float textSpeed;

    [Output]

    public Connection output;

    public override object GetValue(NodePort port)
    {
        return null;
    }


    
}
