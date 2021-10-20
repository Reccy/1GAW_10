using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionUI : MonoBehaviour
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
        if (m_pim.IsSelectingCell)
        {
            if (m_pim.CurrentList.Count == 1)
            {
                m_text.text = "Actions: " + m_pim.CurrentList[0].SelectedText;
            }
            else if (m_pim.CurrentList.Count > 1)
            {
                m_text.text = "Actions: " + m_pim.CurrentList[0].SelectedText + $" [+{m_pim.CurrentList.Count - 1}]";
            }
            else
            {
                m_text.text = "";
            }
        }
        else if (m_pim.IsSelectingAction)
        {
            m_text.text = $"<- Selected: {m_pim.CurrentInteractable.SelectedText} ->";
        }
        else if (m_pim.IsPerforming)
        {
            m_text.text = "Doing: " + m_pim.CurrentInteractable.SelectedText;
        }
        else
        {
            m_text.text = "";
        }
    }
}
