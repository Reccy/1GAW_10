using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Character : MonoBehaviour
{
    private Grid m_grid;
    private Tilemap m_walkableTilemap;

    private Vector3Int m_currentCellPos;
    public Vector3Int CurrentCellPosition => m_currentCellPos;

    private void Awake()
    {
        m_grid = LevelManager.Instance.Grid;
        m_walkableTilemap = LevelManager.Instance.WalkableTilemap;

        // Setup position
        m_currentCellPos = m_grid.WorldToCell(transform.position);
        transform.position = m_grid.GetCellCenterWorld(m_currentCellPos);
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

    private void TryMove(Vector3Int newPosition)
    {
        if (!IsWalkable(newPosition))
            return;

        m_currentCellPos = newPosition;
        transform.position = m_grid.GetCellCenterWorld(m_currentCellPos);
    }

    private bool IsWalkable(Vector3Int position)
    {
        return m_walkableTilemap.GetTile(position) != null;
    }
}
