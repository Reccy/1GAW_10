using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reccy.ScriptExtensions;

public class PlayerInteractionManager : MonoBehaviour
{
    private GridObject m_char;
    private Brain m_playerBrain;
    private Vector3Int m_selectionCellPosition;
    private GameObject m_highlighter;

    private int m_currentInteractableIndex = 0;
    public Interactable CurrentInteractable
    {
        get
        {
            if (m_currentList == null || m_currentList.Count == 0)
                return null;

            return m_currentList[m_currentInteractableIndex];
        }
    }

    private List<Interactable> m_currentList;
    public List<Interactable> CurrentList => m_currentList;

    private Interactable m_currentSelected;
    public Interactable CurrentSelected => m_currentSelected;

    [SerializeField] private InputSource m_input;
    [SerializeField] private InputSource m_playerBrainInputSource;

    private enum SelectionState { IDLE, SELECTING_CELL, SELECTING_ACTION, PERFORMING };
    private SelectionState m_ps;

    public bool IsIdle => m_ps == SelectionState.IDLE;
    public bool IsSelectingCell => m_ps == SelectionState.SELECTING_CELL;
    public bool IsSelectingAction => m_ps == SelectionState.SELECTING_ACTION;
    public bool IsPerforming => m_ps == SelectionState.PERFORMING;

    private void Awake()
    {
        m_char = GetComponent<GridObject>();
        m_playerBrain = GetComponent<Brain>();
        m_highlighter = LevelManager.Instance.Highlighter;
        m_currentList = new List<Interactable>();
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
            else if (IsSelectingCell)
            {
                if (m_currentList.Count == 0)
                {
                    CancelInteraction();
                }
                if (m_currentList.Count == 1)
                {
                    PerformAction();
                }
                else
                {
                    OpenActionMenu();
                }
            }
            else if (IsSelectingAction)
            {
                PerformAction();
            }
        }
        else if (m_input.CancelInput())
        {
            if (CurrentInteractable != null && !CurrentInteractable.CanCancel)
                return;

            CancelInteraction();
        }
        else if (IsSelectingCell)
        {
            HandleMoveHighlighter();
        }
        else if (IsSelectingAction)
        {
            HandleActionHighlighter();
        }
    }

    private void BeginSelecting()
    {
        m_ps = SelectionState.SELECTING_CELL;
        ResetHighlighterPosition();
        m_highlighter.SetActive(true);

        m_input.Clear();
        m_playerBrain.ReleaseControl();
    }

    private void OpenActionMenu()
    {
        if (m_currentList.Count == 0)
        {
            CancelInteraction();
            return;
        }

        m_ps = SelectionState.SELECTING_ACTION;
        m_currentInteractableIndex = 0;
    }

    private void PerformAction()
    {
        m_ps = SelectionState.PERFORMING;

        m_highlighter.SetActive(false);

        CurrentInteractable.OnFinish.AddListener(AfterFinish);
        CurrentInteractable.Perform();
    }

    private void CancelInteraction()
    {
        m_ps = SelectionState.IDLE;
        m_highlighter.SetActive(false);

        if (CurrentInteractable != null)
        {
            CurrentInteractable.OnFinish.RemoveListener(CancelInteraction);
            CurrentInteractable.Cancel();
            m_currentList.Clear();
            m_currentInteractableIndex = 0;
        }

        m_playerBrain.AssumeControl();
    }

    private void AfterFinish()
    {
        m_ps = SelectionState.IDLE;
        m_highlighter.SetActive(false);

        if (CurrentInteractable != null)
        {
            CurrentInteractable.OnFinish.RemoveListener(CancelInteraction);
            m_currentList.Clear();
            m_currentInteractableIndex = 0;
        }

        m_playerBrain.AssumeControl();
    }

    private void HandleMoveHighlighter()
    {
        var h = m_input.UIHInput();
        var v = m_input.UIVInput();

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

        m_currentList = LevelManager.Instance.GetInteractablesAtPosition(m_selectionCellPosition);
        m_highlighter.transform.position = LevelManager.Instance.Grid.GetCellCenterWorld(m_selectionCellPosition);
    }

    private void HandleActionHighlighter()
    {
        var h = m_input.UIHInput();
        var v = m_input.UIVInput();

        void SetIdx(int idx) => m_currentInteractableIndex = Mathf2.Mod(idx, m_currentList.Count);

        if (h == 1)
        {
            SetIdx(m_currentInteractableIndex + 1);
        }
        else if (h == -1)
        {
            SetIdx(m_currentInteractableIndex - 1);
        }
        else if (v == 1)
        {
            SetIdx(m_currentInteractableIndex + 1);
        }
        else if (v == 1)
        {
            SetIdx(m_currentInteractableIndex - 1);
        }
    }

    private void ResetHighlighterPosition()
    {
        m_selectionCellPosition = m_char.CurrentCellPosition;
        m_highlighter.transform.position = LevelManager.Instance.Grid.GetCellCenterWorld(m_char.CurrentCellPosition);
    }
}
