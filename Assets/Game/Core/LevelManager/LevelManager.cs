using Reccy.ScriptExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    [SerializeField] Grid m_grid;
    public Grid Grid => m_grid;

    [SerializeField] Tilemap m_walkableTilemap;
    public Tilemap WalkableTilemap => m_walkableTilemap;

    [SerializeField] GameObject m_highlighter;
    public GameObject Highlighter => m_highlighter;

    [SerializeField] TextLog m_textLog;
    public TextLog TextLog => m_textLog;

    private List<GridObject> m_gridObjects;
    private List<Interactable> m_interactables;

    public static LevelManager Instance;

    // Poor Man's Singleton
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Another LevelManager was instantiated!", this);
            return;
        }

        Instance = this;

        m_gridObjects = new List<GridObject>();
        m_interactables = new List<Interactable>();
    }

    public void Register(GridObject c)
    {
        m_gridObjects.Add(c);
    }

    public void Deregister(GridObject c)
    {
        m_gridObjects.Remove(c);
    }

    public void Register(Interactable inter)
    {
        m_interactables.Add(inter);
    }

    public void Deregister(Interactable inter)
    {
        m_interactables.Remove(inter);
    }

    public void NotifyMoved(Vector3Int from, Vector3Int to, GridObject gridObj)
    {
        foreach (GridObject obj in m_gridObjects)
        {
            if (obj == gridObj)
                continue;

            if (obj.CurrentCellPosition == to)
            {
                // Character needs to move out of position
                List<Vector3Int> targets = FindAdjacentWalkableSpaces(obj.CurrentCellPosition);
                targets.Remove(from);
                Vector3Int target = targets.SelectRandom();

                obj.MoveTo(target);
            }
        }
    }

    public Brain GetBrainAtPosition(Vector3Int position)
    {
        foreach (GridObject character in m_gridObjects)
        {
            if (character.CurrentCellPosition == position)
            {
                return character.gameObject.GetComponent<Brain>();
            }
        }

        return null;
    }

    public List<Interactable> GetInteractablesAtPosition(Vector3Int position)
    {
        List<Interactable> res = new List<Interactable>();

        foreach (Interactable interactable in m_interactables)
        {
            if (interactable.CurrentCellPosition == position)
            {
                res.Add(interactable);
            }
        }

        return res;
    }

    private List<Vector3Int> FindAdjacentWalkableSpaces(Vector3Int position)
    {
        List<Vector3Int> results = new List<Vector3Int>();
        
        void AddIfWalkable(Vector3Int testPos)
        {
            if (IsWalkable(testPos))
            {
                results.Add(testPos);
            }
        }

        AddIfWalkable(position + Vector3Int.up);
        AddIfWalkable(position + Vector3Int.left);
        AddIfWalkable(position + Vector3Int.right);
        AddIfWalkable(position + Vector3Int.down);

        return results;
    }

    public bool IsWalkable(Vector3Int position)
    {
        return m_walkableTilemap.GetTile(position) != null;
    }
}
