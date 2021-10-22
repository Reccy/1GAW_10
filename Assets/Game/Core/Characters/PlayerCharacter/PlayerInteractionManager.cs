using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reccy.ScriptExtensions;

public class PlayerInteractionManager : MonoBehaviour
{
    private Brain m_playerBrain;
    public Brain PlayerBrain => m_playerBrain;

    private Vector3Int m_selectionCellPosition;
    private GameObject m_highlighter;
    private SpriteRenderer m_highlighterSprite;

    private int m_currentInteractableIndex = 0;
    public Interactable CurrentInteractable
    {
        get
        {
            if (m_currentInteractionList == null || m_currentInteractionList.Count == 0)
                return null;

            return m_currentInteractionList[m_currentInteractableIndex];
        }
    }

    private List<Interactable> m_currentInteractionList;
    public List<Interactable> CurrentList => m_currentInteractionList;

    private Brain m_currentHighlightedBrain;
    public Brain CurrentHighlightedBrain => m_currentHighlightedBrain;

    private Stack<Brain> m_brainStack;
    public Brain CurrentBrain
    {
        get
        {
            if (m_brainStack == null)
                m_brainStack = new Stack<Brain>();

            if (m_brainStack.Count == 0)
                return null;

            return m_brainStack.Peek();
        }
    }


    public List<string> BrainStackNames
    {
        get
        {
            List<string> res = new List<string>();

            foreach (Brain b in m_brainStack)
            {
                res.Add(b.DisplayName);
            }

            return res;
        }
    }

    private Dictionary<Brain, Attributes.OnHealthExhaustedEvent> m_exhaustionEvents;

    [Header("Input Setup")]
    [SerializeField] private InputSource m_input;
    [SerializeField] private InputSource m_playerBrainInputSource;

    [Header("Highlighter Colors")]
    [SerializeField] private Color m_hurtColor;
    [SerializeField] private Color m_controlColor;
    [SerializeField] private Color m_interactColor;

    private enum SelectionState { IDLE, SELECTING_CELL, SELECTING_ACTION, PERFORMING };
    private SelectionState m_ps;

    public bool IsIdle => m_ps == SelectionState.IDLE;
    public bool IsSelectingCell => m_ps == SelectionState.SELECTING_CELL;
    public bool IsSelectingAction => m_ps == SelectionState.SELECTING_ACTION;
    public bool IsPerforming => m_ps == SelectionState.PERFORMING;

    private enum InteractionMode { ACTION, HURT, CONTROL }
    private InteractionMode m_im;

    public bool IsActionIntent => m_im == InteractionMode.ACTION;
    public bool IsHurtIntent => m_im == InteractionMode.HURT;
    public bool IsControlIntent => m_im == InteractionMode.CONTROL;

    private void Awake()
    { 
        m_playerBrain = GetComponent<Brain>();
        m_highlighter = LevelManager.Instance.Highlighter;
        m_highlighterSprite = m_highlighter.GetComponent<SpriteRenderer>();
        m_currentInteractionList = new List<Interactable>();
        m_exhaustionEvents = new Dictionary<Brain, Attributes.OnHealthExhaustedEvent>();

        m_brainStack = new Stack<Brain>();
        m_brainStack.Push(m_playerBrain);
        
        LevelManager.Instance.PossessionChainLine.SetStack(m_brainStack.ToList());

        ResetHighlighterPosition();
    }

    private void OnEnable()
    {
        m_playerBrain.Attributes.OnHealthExhausted += OnPlayerDeath;

        foreach (var brain in m_brainStack)
        {
            Attributes.OnHealthExhaustedEvent evt;
            if (m_exhaustionEvents.TryGetValue(brain, out evt))
            {
                brain.Attributes.OnHealthExhausted += evt;
            }
        }
    }

    private void OnDisable()
    {
        m_playerBrain.Attributes.OnHealthExhausted -= OnPlayerDeath;

        foreach (var brain in m_brainStack)
        {
            Attributes.OnHealthExhaustedEvent evt;
            if (m_exhaustionEvents.TryGetValue(brain, out evt))
            {
                brain.Attributes.OnHealthExhausted -= evt;
            }
        }
    }

    private void OnPlayerDeath()
    {
        while (m_brainStack.Count > 0)
        {
            m_brainStack.Pop().ReleaseControl();
        }

        LevelManager.Instance.PossessionChainLine.SetStack(new List<Brain>());

        Log("You have died... GAME OVER");

        Destroy(gameObject);
    }

    private void OnBrainDeath(Brain deadBrain)
    {
        do
        {
            var brain = m_brainStack.Pop();

            UnregisterExhaustionEvent(brain);
            brain.ReleaseControl();

            LevelManager.Instance.PossessionChainLine.SetStack(m_brainStack.ToList());

            Log($"Lost control of {brain.DisplayName}");
        }
        while (m_brainStack.Contains(deadBrain));

        CurrentBrain.AssumeControl();
    }

    private void FixedUpdate()
    {
        if (IsIdle)
        {
            if (m_input.CancelInput())
            {
                if (m_brainStack.Count > 1)
                {
                    ReleaseControl();
                }
            }
            else if (m_input.ActionInput())
            {
                m_im = InteractionMode.ACTION;
                m_highlighterSprite.color = m_interactColor;
                BeginSelecting();
            }
            else if (m_input.ControlInput())
            {
                m_im = InteractionMode.CONTROL;
                m_highlighterSprite.color = m_controlColor;
                BeginSelecting();
            }
            else if (m_input.HurtInput())
            {
                m_im = InteractionMode.HURT;
                m_highlighterSprite.color = m_hurtColor;
                BeginSelecting();
            }
        }
        else if (IsSelectingCell)
        {
            if (m_input.CancelInput())
            {
                if (CurrentInteractable != null && !CurrentInteractable.CanCancel)
                    return;

                CancelInteraction();
            }
            else if (IsActionIntent)
            {
                if (m_input.ActionInput())
                {
                    if (m_currentInteractionList.Count == 0)
                    {
                        CancelInteraction();
                        return;
                    }
                    else if (m_currentInteractionList.Count == 1)
                    {
                        PerformAction();
                        return;
                    }
                    else
                    {
                        OpenActionMenu();
                        return;
                    }
                }
                else
                {
                    HandleSelectionHighlighter();
                }
            }
            else if (IsControlIntent)
            {
                if (m_input.ActionInput())
                {
                    if (m_currentHighlightedBrain == null)
                    {
                        CancelInteraction();
                        return;
                    }
                    else
                    {
                        AssumeControl();
                    }
                }
                else
                {
                    HandleSelectionHighlighter();
                }
            }
            else if (IsHurtIntent)
            {
                if (m_input.ActionInput())
                {
                    if (m_currentHighlightedBrain == null)
                    {
                        CancelInteraction();
                        return;
                    }
                    else
                    {
                        PerformHarm();
                        return;
                    }
                }
                else
                {
                    HandleSelectionHighlighter();
                }
            }
        }
        else if (IsSelectingAction)
        {
            if (m_input.CancelInput())
            {
                if (CurrentInteractable != null && !CurrentInteractable.CanCancel)
                    return;

                CancelInteraction();
            }
            else if (IsActionIntent)
            {
                if (m_input.ActionInput())
                {
                    PerformAction();
                }
                else
                {
                    HandleActionMenuHighlighter();
                }
            }
        }
    }

    private void BeginSelecting()
    {
        m_ps = SelectionState.SELECTING_CELL;
        ResetHighlighterPosition();
        m_highlighter.SetActive(true);

        m_input.Clear();
        CurrentBrain.ReleaseControl();
    }

    private void OpenActionMenu()
    {
        if (m_currentInteractionList.Count == 0)
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
        CurrentInteractable.Perform(CurrentBrain);
    }

    private void PerformHarm()
    {
        m_ps = SelectionState.IDLE;

        m_highlighter.SetActive(false);

        Weapon weapon;
        if (LevelManager.Instance.IsInGrabRange(CurrentBrain.CurrentCellPosition, m_currentHighlightedBrain.CurrentCellPosition))
        {
            weapon = CurrentBrain.Attributes.MeleeWeapon;
        }
        else if (LevelManager.Instance.IsInLineOfSight(CurrentBrain.CurrentCellPosition, m_currentHighlightedBrain.CurrentCellPosition))
        {
            weapon = CurrentBrain.Attributes.RangedWeapon;
        }
        else
        {
            CancelInteraction();
            return;
        }

        Log($"{CurrentBrain.DisplayName} {weapon.GetAttackVerb()} {m_currentHighlightedBrain.DisplayName} for {weapon.Damage}HP");

        CurrentBrain.AssumeControl();
        
        m_currentHighlightedBrain.Attributes.Harm(weapon.Damage);
    }

    private void AssumeControl()
    {
        m_ps = SelectionState.IDLE;

        m_highlighter.SetActive(false);

        CurrentBrain.ReleaseControl();

        m_brainStack.Push(m_currentHighlightedBrain);

        CurrentBrain.AssumeControl();

        RegisterExhaustionEvent(CurrentBrain);

        LevelManager.Instance.PossessionChainLine.SetStack(m_brainStack.ToList());

        Log($"Gained control of {CurrentBrain.DisplayName}");
    }

    private void ReleaseControl()
    {
        Log($"Released control of {CurrentBrain.DisplayName}");

        m_ps = SelectionState.IDLE;

        m_highlighter.SetActive(false);

        CurrentBrain.ReleaseControl();

        UnregisterExhaustionEvent(CurrentBrain);

        m_brainStack.Pop();

        LevelManager.Instance.PossessionChainLine.SetStack(m_brainStack.ToList());

        CurrentBrain.AssumeControl();
    }

    private void UnregisterExhaustionEvent(Brain brain)
    {
        brain.Attributes.OnHealthExhausted -= m_exhaustionEvents[brain];
        m_exhaustionEvents.Remove(brain);
    }

    private void RegisterExhaustionEvent(Brain brain)
    {
        m_exhaustionEvents.Add(brain, delegate(){ OnBrainDeath(brain); });
        CurrentBrain.Attributes.OnHealthExhausted += m_exhaustionEvents[brain];
    }

    private void CancelInteraction()
    {
        m_ps = SelectionState.IDLE;
        m_highlighter.SetActive(false);

        if (CurrentInteractable != null)
        {
            CurrentInteractable.OnFinish.RemoveListener(CancelInteraction);
            CurrentInteractable.Cancel();
            m_currentInteractionList.Clear();
            m_currentInteractableIndex = 0;
        }

        CurrentBrain.AssumeControl();
    }

    private void AfterFinish()
    {
        m_ps = SelectionState.IDLE;
        m_highlighter.SetActive(false);

        if (CurrentInteractable != null)
        {
            CurrentInteractable.OnFinish.RemoveListener(CancelInteraction);
            m_currentInteractionList.Clear();
            m_currentInteractableIndex = 0;
        }

        CurrentBrain.AssumeControl();
    }

    private void HandleSelectionHighlighter()
    {
        var h = m_input.UIHInput();
        var v = m_input.UIVInput();

        void MoveSelection(Vector3Int newPosition)
        {
            if (!LevelManager.Instance.IsInFogOfWar(newPosition))
            {
                m_selectionCellPosition = newPosition;
            }
        }

        if (h == 1)
        {
            MoveSelection(m_selectionCellPosition + Vector3Int.right);
        }
        else if (h == -1)
        {
            MoveSelection(m_selectionCellPosition + Vector3Int.left);
        }
        else if (v == 1)
        {
            MoveSelection(m_selectionCellPosition + Vector3Int.up);
        }
        else if (v == -1)
        {
            MoveSelection(m_selectionCellPosition + Vector3Int.down);
        }

        if (IsActionIntent)
        {
            m_currentInteractionList = LevelManager.Instance.GetInteractablesAtPosition(m_selectionCellPosition);
        }
        else if (IsControlIntent)
        {
            m_currentHighlightedBrain = LevelManager.Instance.GetBrainAtPosition(m_selectionCellPosition);

            // Can't select brain if it's already in a control chain
            if (m_brainStack.Contains(m_currentHighlightedBrain))
                m_currentHighlightedBrain = null;
        }
        else if (IsHurtIntent)
        {
            m_currentHighlightedBrain = LevelManager.Instance.GetBrainAtPosition(m_selectionCellPosition);
        }

        m_highlighter.transform.position = LevelManager.Instance.Grid.GetCellCenterWorld(m_selectionCellPosition);
    }

    private void HandleActionMenuHighlighter()
    {
        var h = m_input.UIHInput();
        var v = m_input.UIVInput();

        void SetIdx(int idx) => m_currentInteractableIndex = Mathf2.Mod(idx, m_currentInteractionList.Count);

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
        m_selectionCellPosition = CurrentBrain.CurrentCellPosition;
        m_highlighter.transform.position = LevelManager.Instance.Grid.GetCellCenterWorld(CurrentBrain.CurrentCellPosition);
    }

    private void Log(string str) => LevelManager.Instance.TextLog.Log(str);
}
