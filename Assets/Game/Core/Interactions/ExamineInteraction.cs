using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExamineInteraction : MonoBehaviour
{
    [SerializeField] private string m_examineText;

    public void Examine(Brain instigator, Interactable interactable)
    {
        LevelManager.Instance.TextLog.Log($"{instigator.DisplayName}: '{m_examineText}'");
        interactable.Finish();
    }
}
