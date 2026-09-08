using System.Collections.Generic;
using UnityEngine;
using XNode;

public class DialogAnswerSegments : DialogSegment
{
    [Output(dynamicPortList = true)]
    public List<string> Answers;
	
	public override object GetValue(NodePort port) {

		if(port.fieldName.Contains("Answers"))
        {
			for(int i =0; i< Answers.Count; i++)
            {
                if(port.fieldName == "Answers " + i)
                {
                    return Answers[i];
                }
            }
		}
		return null; // Replace this
	}
}
