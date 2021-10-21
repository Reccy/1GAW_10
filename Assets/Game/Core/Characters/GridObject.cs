using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridObject : MonoBehaviour
{
    private Grid m_grid;

    private Vector3Int m_currentCellPos;
    public Vector3Int CurrentCellPosition => m_currentCellPos;

    [SerializeField] private bool m_canPush = true;
    public bool CanPush => m_canPush;

    [SerializeField] private bool m_transparent = true;
    public bool IsTransparent => m_transparent;
    public bool IsOpaque => !m_transparent;

    private void Awake()
    {
        m_grid = LevelManager.Instance.Grid;

        // Setup position
        m_currentCellPos = m_grid.WorldToCell(transform.position);
        transform.position = m_grid.GetCellCenterWorld(m_currentCellPos);
    }

    private void OnEnable()
    {
        LevelManager.Instance.Register(this);
    }

    private void OnDisable()
    {
        LevelManager.Instance.Deregister(this);
    }

    public void MoveUp()
    {
        Vector3Int newPosition = m_currentCellPos + Vector3Int.up;

        TryMove(newPosition);
    }

    public void MoveRight()
    {
        Vector3Int newPosition = m_currentCellPos + Vector3Int.right;

        TryMove(newPosition);
    }

    public void MoveDown()
    {
        Vector3Int newPosition = m_currentCellPos + Vector3Int.down;

        TryMove(newPosition);
    }

    public void MoveLeft()
    {
        Vector3Int newPosition = m_currentCellPos + Vector3Int.left;

        TryMove(newPosition);
    }

    public void MoveTo(Vector3Int position)
    {
        TryMove(position);
    }

    private void TryMove(Vector3Int newPosition)
    {
        if (!IsWalkable(newPosition))
            return;

        var originalPos = m_currentCellPos;
        m_currentCellPos = newPosition;
        transform.position = m_grid.GetCellCenterWorld(m_currentCellPos);

        LevelManager.Instance.NotifyMoved(originalPos, newPosition, this);
    }

    private bool IsWalkable(Vector3Int position)
    {
        return LevelManager.Instance.IsWalkable(position);
    }
}
