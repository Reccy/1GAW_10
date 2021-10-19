using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Brain : MonoBehaviour
{
    private Character m_character;
    
    [Header("Setup")]
    [SerializeField] private InputSource m_input;
    public InputSource Input => m_input;

    [SerializeField] private UnityEvent OnActionPerformed;

    [Header("AI")]
    [SerializeField] private BrainState m_brainState;
    private enum BrainState { IDLE, AI, PC };

    private bool m_isPlayerCharacter = false;

    private void Awake()
    {
        m_character = GetComponent<Character>();

        m_isPlayerCharacter = m_brainState == BrainState.PC;
    }

    public void ReleaseControl()
    {
        if (m_isPlayerCharacter)
        {
            m_brainState = BrainState.IDLE;
        }
        else
        {
            m_brainState = BrainState.AI;
        }
    }

    public void AssumeControl()
    {
        m_brainState = BrainState.PC;
    }

    private void FixedUpdate()
    {
        switch (m_brainState)
        {
            case BrainState.IDLE:
                FixedUpdateIdle();
                break;
            case BrainState.PC:
                FixedUpdatePC();
                break;
            case BrainState.AI:
                FixedUpdateAI();
                break;
        }
    }

    private void FixedUpdateIdle()
    {

    }

    private void FixedUpdatePC()
    {
        HandleMovementPC();
        HandleActionsPC();
    }

    private void FixedUpdateAI()
    {

    }

    private void HandleMovementPC()
    {
        var m_moveHInput = m_input.MoveHInput();

        if (m_moveHInput == 1)
        {
            m_character.MoveRight();
        }
        else if (m_moveHInput == -1)
        {
            m_character.MoveLeft();
        }

        var m_moveVInput = m_input.MoveVInput();

        if (m_moveVInput == 1)
        {
            m_character.MoveUp();
        }
        else if (m_moveVInput == -1)
        {
            m_character.MoveDown();
        }
    }

    private void HandleActionsPC()
    {
        if (!m_input.ActionInput())
            return;

        if (OnActionPerformed == null)
        {
            Debug.Log("Cannot perform action - it is not set!"); // todo: expose this to UI with SFX
            return;
        }

        OnActionPerformed.Invoke();
    }
}
