using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDoorInteraction : MonoBehaviour
{
    [SerializeField] private GameObject m_door;

    public void UnlockDoor(Brain instigator, Interactable interactable)
    {
        LevelManager.Instance.TextLog.Log($"{instigator.DisplayName} unlocks the door");

        m_door.SetActive(false);

        interactable.Finish();

        interactable.enabled = false;
    }
}
