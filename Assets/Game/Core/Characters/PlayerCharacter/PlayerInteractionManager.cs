using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionManager : MonoBehaviour
{
    private GridObject m_char;
    private Brain m_playerBrain;
    private Vector3Int m_selectionCellPosition;
    private GameObject m_highlighter;
    
    private Interactable m_currentInteractable;
    public Interactable CurrentInteractable => m_currentInteractable;

    private Interactable m_currentSelected;
    public Interactable CurrentSelected => m_currentSelected;

    [SerializeField] private InputSource m_input;
    [SerializeField] private InputSource m_playerBrainInputSource;

    private enum SelectionState { IDLE, SELECTING, PERFORMING };
    private SelectionState m_ps;

    public bool IsIdle => m_ps == SelectionState.IDLE;
    public bool IsSelecting => m_ps == SelectionState.SELECTING;
    public bool IsPerforming => m_ps == SelectionState.PERFORMING;

    private void Awake()
    {
        m_char = GetComponent<GridObject>();
        m_playerBrain = GetComponent<Brain>();
        m_highlighter = LevelManager.Instance.Highlighter;
        ResetHighlighterPosition();
    }

    private void FixedUpdate()
    {
        if (m_input.ActionInput())
        {
            if (IsIdle)
            {
                BeginSelecting();
            }
            else if (IsSelecting)
            {
                DoInteract();
            }
        }
        else if (m_input.CancelInput())
        {
            if (m_currentInteractable != null && !m_currentInteractable.CanCancel)
                return;

            CancelInteraction();
        }
        else if (IsSelecting)
        {
            HandleMoveHighlighter();
        }
    }

    private void BeginSelecting()
    {
        m_ps = SelectionState.SELECTING;
        ResetHighlighterPosition();
        m_highlighter.SetActive(true);

        m_input.Clear();
        m_playerBrain.ReleaseControl();
    }

    private void DoInteract()
    {
        // Check for Interactable
        m_currentInteractable = LevelManager.Instance.GetInteractableAtPosition(m_selectionCellPosition);

        if (m_currentInteractable == null)
        {
            CancelInteraction();
            return;
        }

        m_ps = SelectionState.PERFORMING;
        m_highlighter.SetActive(false);

        m_currentInteractable.OnFinish.AddListener(AfterFinish);
        m_currentInteractable.Perform();
    }

    private void CancelInteraction()
    {
        m_ps = SelectionState.IDLE;
        m_highlighter.SetActive(false);

        if (m_currentInteractable != null)
        {
            m_currentInteractable.OnFinish.RemoveListener(CancelInteraction);
            m_currentInteractable.Cancel();
            m_currentInteractable = null;
        }

        m_playerBrain.AssumeControl();
    }

    private void AfterFinish()
    {
        m_ps = SelectionState.IDLE;
        m_highlighter.SetActive(false);

        if (m_currentInteractable != null)
        {
            m_currentInteractable.OnFinish.RemoveListener(CancelInteraction);
            m_currentInteractable = null;
        }

        m_playerBrain.AssumeControl();
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

        m_currentSelected = LevelManager.Instance.GetInteractableAtPosition(m_selectionCellPosition);
        m_highlighter.transform.position = LevelManager.Instance.Grid.GetCellCenterWorld(m_selectionCellPosition);
    }

    private void ResetHighlighterPosition()
    {
        m_selectionCellPosition = m_char.CurrentCellPosition;
        m_highlighter.transform.position = LevelManager.Instance.Grid.GetCellCenterWorld(m_char.CurrentCellPosition);
    }
}
