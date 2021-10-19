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
        if (m_pim.IsSelecting)
        {
            if (m_pim.CurrentSelected != null)
            {
                m_text.text = "Selected: " + m_pim.CurrentSelected.SelectedText;
            }
            else
            {
                m_text.text = "";
            }
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
