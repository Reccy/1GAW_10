using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ControlText : MonoBehaviour
{
    private TMP_Text m_text;
    private PlayerInteractionManager m_pim;

    private void Awake()
    {
        m_text = GetComponent<TMP_Text>();
        m_pim = FindObjectOfType<PlayerInteractionManager>();
    }

    private void Update()
    {
        var names = m_pim.BrainStackNames;

        m_text.text = "=== Current Brain ===\n";
        
        for (int i = 0; i < names.Count; ++i)
        {
            if (i == 1)
            {
                m_text.text += "\n=== Stack ===\n";
            }
            
            m_text.text += $"{names[i]}\n";
        }
    }
}
