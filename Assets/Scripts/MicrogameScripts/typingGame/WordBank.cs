using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WordBank : MonoBehaviour
{
    public List<string> originalWords = new List<string>()
    {
        "Katabasis", "Charon", "Express"
    };

    private List<string> copiedWords = new List<string>();
    private void Awake()
    {
        copiedWords.AddRange(originalWords);
        Shuffle(copiedWords);


    }

    private void Shuffle(List<string> strings)
    {
        for (int i = 0; i < strings.Count; i++)
        {
            int random = Random.Range(i, strings.Count);
            string temp = strings[i];

            strings[i] = strings[random];
            strings[random] = temp;
        }
    }

    public string GetWord()
    {
        string newWord = string.Empty;

        if (copiedWords.Count != 0)
        {
            newWord = copiedWords.Last();
            copiedWords.Remove(copiedWords.Last());
        }
        return newWord;
    }
}
