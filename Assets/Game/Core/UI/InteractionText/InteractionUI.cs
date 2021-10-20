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
            if (m_pim.IsActionIntent)
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
            else if (m_pim.IsControlIntent)
            {
                if (m_pim.CurrentHighlightedBrain != null)
                {
                    m_text.text = $"Possess: {m_pim.CurrentHighlightedBrain.DisplayName}";
                }
                else
                {
                    m_text.text = "";
                }
            }
            else if (m_pim.IsHurtIntent)
            {
                if (m_pim.CurrentHighlightedBrain != null)
                {
                    if (m_pim.CurrentHighlightedBrain == m_pim.CurrentBrain)
                    {
                        m_text.text = $"Hurt Self";
                    }
                    else
                    {
                        m_text.text = $"Hurt: {m_pim.CurrentHighlightedBrain.DisplayName}";
                        
                        if (LevelManager.Instance.IsInGrabRange(m_pim.CurrentBrain.CurrentCellPosition, m_pim.CurrentHighlightedBrain.CurrentCellPosition))
                        {
                            m_text.text += $" (Melee for {m_pim.CurrentBrain.Attributes.MeleeWeapon.Damage} HP)";
                        }
                        else if (LevelManager.Instance.IsInLineOfSight(m_pim.CurrentBrain.CurrentCellPosition, m_pim.CurrentHighlightedBrain.CurrentCellPosition))
                        {
                            m_text.text += $" (Ranged for {m_pim.CurrentBrain.Attributes.RangedWeapon.Damage} HP)";
                        }
                        else
                        {
                            m_text.text += $" (Can't Hit!)";
                        }

                        if (m_pim.CurrentHighlightedBrain == m_pim.PlayerBrain)
                        {
                            m_text.text += $" (YOU!)";
                        }
                    }
                }
                else
                {
                    m_text.text = "";
                }
            }
            else
            {
                // Should never see this tbh
                m_text.text = "INVALID INTENT";
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
