using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class History : MonoBehaviour
{
    Text history;
    List<string> messages = new List<string>();
    Coroutine c;

    void Awake()
    {
        history = GetComponent<Text>();
    }

    void Start()
    {
        c = StartCoroutine(RemoveMessages());
    }

    void Update()
    {
        string log = "";
        foreach (string s in messages)
        {
            log += s + "\n";
        }

        history.text = log;
    }

    public void AddMessage(string msg)
    {
        StopCoroutine(c);
        messages.Add(msg);
        c = StartCoroutine(RemoveMessages());
    }

    IEnumerator RemoveMessages()
    {
        while (true)
        {
            yield return new WaitForSeconds(5.0f);

            if (messages.Count > 0)
            {
                messages.RemoveAt(0);
            }
        }
    }
}
