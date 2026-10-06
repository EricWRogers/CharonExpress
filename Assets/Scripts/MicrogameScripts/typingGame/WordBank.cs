using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WordBank : MonoBehaviour
{
    public List<WordObject> words;

    private List<WordObject> copiedWords = new List<WordObject>();
    private void Awake()
    {
        
        Shuffle(words,copiedWords);


    }

    private void Shuffle(List<WordObject> strings, List<WordObject> tempwords)
    {
        List<WordObject> temp = new List<WordObject>();
        temp.AddRange(strings);

        for (int i = 0; i < strings.Count; i++)
        {
            int index = Random.Range(0, temp.Count - 1);
            tempwords.Add(temp[index]);
            temp.RemoveAt(index);
        }
    }
    private void Shuffle(List<string> strings, List<string> tempwords)
    {
        List<string> temp = new List<string>();
        temp.AddRange(strings);

        for (int i = 0; i < strings.Count; i++)
        {
            int index = Random.Range(0, temp.Count - 1);
            tempwords.Add(temp[index]);
            temp.RemoveAt(index);
        }
    }

    public WordObject GetWord()
    {
        WordObject newWord = null;

        if (copiedWords.Count != 0)
        {
            newWord = copiedWords[copiedWords.Count - 1];
            copiedWords.Remove(copiedWords.Last());
        }
        return newWord;
    }
}
