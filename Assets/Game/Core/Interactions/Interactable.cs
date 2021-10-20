using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(GridObject))]
public class Interactable : MonoBehaviour
{
    private GridObject m_obj;
    public Vector3Int CurrentCellPosition => m_obj.CurrentCellPosition;

    [Header("Options")]
    [SerializeField] private bool m_canCancel = true;
    public bool CanCancel
    {
        get => m_canCancel;
        set => m_canCancel = value;
    }

    [SerializeField] private string m_selectedText = "DEFAULT";
    public string SelectedText => m_selectedText;

    [Header("Events")]
    public UnityEvent<Brain, Interactable> OnPerform;
    public UnityEvent OnFinish;
    public UnityEvent OnCancel;

    private void Awake()
    {
        m_obj = GetComponent<GridObject>();
    }

    private void OnEnable()
    {
        LevelManager.Instance.Register(this);
    }

    private void OnDisable()
    {
        LevelManager.Instance.Deregister(this);
    }

    public void Perform(Brain instigator)
    {
        if (OnPerform != null)
            OnPerform.Invoke(instigator, this);
        else
            Debug.LogWarning($"Empty Perform() Called on {gameObject.name}", this);
    }

    public void Finish()
    {
        if (OnFinish != null)
            OnFinish.Invoke();
        else
            Debug.LogWarning($"Empty Finish() Called on {gameObject.name}", this);
    }

    public void Cancel()
    {
        if (!CanCancel)
        {
            Debug.LogError($"Something attempted to Cancel this non-cancelable Interactable", this);
            return;
        }

        if (OnCancel != null)
            OnCancel.Invoke();
        else
            Debug.LogWarning($"Empty Cancel() Called on {gameObject.name}", this);
    }
}
