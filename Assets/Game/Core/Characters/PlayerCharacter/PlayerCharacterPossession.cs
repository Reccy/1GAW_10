using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterPossession : MonoBehaviour
{
    private Character m_char;
    private Brain m_playerBrain;
    private Brain m_controlledBrain;
    private Vector3Int m_selectionCellPosition;
    private GameObject m_highlighter;

    [SerializeField] private InputSource m_input;
    [SerializeField] private InputSource m_playerBrainInputSource;

    private enum PossessionState { IDLE, LOOKING, POSSESSING };
    private PossessionState m_ps;

    public bool IsIdle => m_ps == PossessionState.IDLE;
    public bool IsLooking => m_ps == PossessionState.LOOKING;
    public bool IsPossessing => m_ps == PossessionState.POSSESSING;

    private void Awake()
    {
        m_char = GetComponent<Character>();
        m_playerBrain = GetComponent<Brain>();
        m_highlighter = LevelManager.Instance.Highlighter;
        ResetHighlighterPosition();
    }

    private void ResetHighlighterPosition()
    {
        m_selectionCellPosition = m_char.CurrentCellPosition;
        m_highlighter.transform.position = LevelManager.Instance.Grid.GetCellCenterWorld(m_char.CurrentCellPosition);
    }

    public void PlayerCharacterPossessionAction()
    {
        if (IsIdle)
        {
            IdleAction();
        }
    }

    private void FixedUpdate()
    {
        // Extra input logic since the Player Brain is not handling the input when in this state...
        if (m_input.ActionInput())
        {
            if (IsLooking)
            {
                LookingAction();
            }
            else if (IsPossessing)
            {
                PossessingAction();
            }
        }

        if (m_input.CancelInput())
        {
            if (IsLooking)
            {
                LookingCancel();
            }
            else if (IsPossessing)
            {
                PossessingCancel();
            }
        }

        if (IsIdle)
        {
            IdleFixedUpdate();
        }
        else if (IsLooking)
        {
            LookingActionFixedUpdate();
        }
        else if (IsPossessing)
        {
            IsPossessingFixedUpdate();
        }
    }

    private void IdleAction()
    {
        m_ps = PossessionState.LOOKING;
        ResetHighlighterPosition();
        m_highlighter.SetActive(true);

        m_playerBrain.ReleaseControl();

        // Clear to prevent old inputs from being read once highlighter is set active
        m_input.Clear();
    }

    private void LookingAction()
    {
        m_controlledBrain = LevelManager.Instance.GetBrainAtPosition(m_selectionCellPosition);

        if (m_controlledBrain == null)
        {
            LookingCancel();
            return;
        }

        m_ps = PossessionState.POSSESSING;
        m_highlighter.SetActive(false);
        m_controlledBrain.Input.Clear();
        m_controlledBrain.AssumeControl();
    }

    private void PossessingAction()
    {
        // ignore
    }

    private void LookingCancel()
    {
        m_ps = PossessionState.IDLE;
        m_highlighter.SetActive(false);
        m_playerBrainInputSource.Clear();
        m_playerBrain.AssumeControl();
    }

    private void PossessingCancel()
    {
        m_ps = PossessionState.IDLE;
        m_playerBrainInputSource.Clear();
        m_controlledBrain.ReleaseControl();
        m_playerBrain.AssumeControl();
    }

    private void IdleFixedUpdate()
    {

    }

    private void LookingActionFixedUpdate()
    {
        HandleMoveHighlighter();
    }

    private void IsPossessingFixedUpdate()
    {

    }

    private void HandleMoveHighlighter()
    {
        var h = m_input.MoveHInput();
        var v = m_input.MoveVInput();

        if (h == 1)
        {
            m_selectionCellPosition += Vector3Int.right;
        }
        else if (h == -1)
        {
            m_selectionCellPosition += Vector3Int.left;
        }
        else if (v == 1)
        {
            m_selectionCellPosition += Vector3Int.up;
        }
        else if (v == -1)
        {
            m_selectionCellPosition += Vector3Int.down;
        }

        m_highlighter.transform.position = LevelManager.Instance.Grid.GetCellCenterWorld(m_selectionCellPosition);
    }
}
