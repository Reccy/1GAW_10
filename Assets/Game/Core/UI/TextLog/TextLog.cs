using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextLog : MonoBehaviour
{
    private TMP_Text m_text;

    private void Awake()
    {
        m_text = GetComponent<TMP_Text>();
    }

    public void AddLine(string line)
    {
        m_text.text += $"\n\n{line}";
    }
}
