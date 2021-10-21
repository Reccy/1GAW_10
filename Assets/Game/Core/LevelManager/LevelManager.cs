using Reccy.DebugExtensions;
using Reccy.ScriptExtensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    [SerializeField] Grid m_grid;
    public Grid Grid => m_grid;

    [SerializeField] Tilemap m_walkableTilemap;
    public Tilemap WalkableTilemap => m_walkableTilemap;

    [SerializeField] Tilemap m_fogOfWarTilemap;
    public Tilemap FogOfWarTilemap => m_fogOfWarTilemap;

    [SerializeField] GameObject m_highlighter;
    public GameObject Highlighter => m_highlighter;

    [SerializeField] TextLog m_textLog;
    public TextLog TextLog => m_textLog;

    [SerializeField] private PossessionChainLine m_possessionChainLine;
    public PossessionChainLine PossessionChainLine => m_possessionChainLine;

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

        m_possessionChainLine.gameObject.SetActive(true);
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
        foreach (GridObject gridObject in m_gridObjects)
        {
            if (gridObject.CurrentCellPosition == position)
            {
                return gridObject.gameObject.GetComponent<Brain>();
            }
        }

        return null;
    }

    public GridObject GetGridObjectAtPosition(Vector3Int position)
    {
        foreach (GridObject gridObject in m_gridObjects)
        {
            if (gridObject.CurrentCellPosition == position)
            {
                return gridObject;
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
        if (m_walkableTilemap.GetTile(position) == null)
            return false;

        GridObject obj = GetGridObjectAtPosition(position);

        if (obj != null && !obj.CanPush)
            return false;

        return true;
    }

    public bool IsVisible(Vector3Int position)
    {
        if (m_walkableTilemap.GetTile(position) == null)
            return true;

        GridObject obj = GetGridObjectAtPosition(position);

        if (obj != null && obj.IsOpaque)
            return false;

        return true;
    }

    public bool IsInGrabRange(Vector3Int from, Vector3Int to)
    {
        var res = Pathfind(from, to, useDiagonals: true);
        float totalDistance = Vector3Int2.ListLength(res);

        return totalDistance <= 1.5f;
    }

    public bool IsInLineOfSight(Vector3Int from, Vector3Int to)
    {
        if (!IsVisible(to) || !IsVisible(from))
            return false;

        var optimalPath = Pathfind(from, to, useDiagonals: true);
        var line = Pathfind(from, to, useDiagonals: true, ignoreObstacles: true);

        return optimalPath.IsEqual(line);
    }

    public bool IsInFogOfWar(Vector3Int position)
    {
        return m_fogOfWarTilemap.GetTile(position) != null;
    }

    public List<Vector3Int> Pathfind(Vector3Int startPos, Vector3Int endPos, bool useDiagonals = false, bool ignoreObstacles = false)
    {
        var original = Pathfind((Vector2Int)startPos, (Vector2Int)endPos, useDiagonals, ignoreObstacles);

        List<Vector3Int> result = new List<Vector3Int>();

        foreach (var v in original)
        {
            result.Add((Vector3Int)v);
        }

        return result;
    }

    private List<Vector2Int> Pathfind(Vector2Int startPos, Vector2Int endPos, bool useDiagonals = false, bool ignoreObstacles = false)
    {
        // Prepare A*
        Vector2Int startCellIndex = startPos;
        Vector2Int endCellIndex = endPos;

        Vector2Int current;
        List<Vector2Int> frontier = new List<Vector2Int>();
        Dictionary<Vector2Int, float> cellPriority = new Dictionary<Vector2Int, float>();
        Dictionary<Vector2Int, float> currentCost = new Dictionary<Vector2Int, float>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        Vector2Int nullV2Int = new Vector2Int(int.MinValue, int.MinValue); // Fake "null" value to determine end of cameFrom

        // Start the algorithm
        frontier.Add(startCellIndex);
        cellPriority.Add(startCellIndex, 0);
        currentCost.Add(startCellIndex, 0);
        cameFrom.Add(startCellIndex, nullV2Int);

        while (frontier.Count > 0)
        {
            current = cellPriority.GetSmallest();
            cellPriority.Remove(current);

            if (current == endCellIndex)
                break;

            foreach (Vector2Int next in GetNeighbours(current, useDiagonals: useDiagonals, ignoreObstacles: ignoreObstacles))
            {
                float newCost = currentCost[current] + Vector2Int.Distance(current, next);

                if (!currentCost.ContainsKey(next) || newCost < currentCost[next])
                {
                    currentCost[next] = newCost;

                    float priority = newCost + Vector2Int.Distance(next, endCellIndex);

                    if (!cellPriority.ContainsKey(next))
                        cellPriority.Add(next, priority);
                    else
                        cellPriority[next] = priority;

                    frontier.Add(next);

                    if (!cameFrom.ContainsKey(next))
                        cameFrom.Add(next, current);
                    else
                        cameFrom[next] = current;
                }
            }
        }

        // Start backtracking through cameFrom to find the cell
        List<Vector2Int> path = new List<Vector2Int>();

        current = endCellIndex;
        while (cameFrom[current] != nullV2Int)
        {
            path.Add(new Vector2Int(current.x, current.y));
            current = cameFrom[current];
        }

        // Add original start and end positions to path
        path.Add(startPos);
        path.Reverse();
        path.Add(endPos);

        return path;
    }

    private List<Vector3Int> GetNeighbours(Vector3Int cellPos, bool useDiagonals = false, bool ignoreObstacles = false)
    {
        var original = GetNeighbours((Vector2Int)cellPos, useDiagonals: useDiagonals, ignoreObstacles: ignoreObstacles);

        List<Vector3Int> result = new List<Vector3Int>();

        foreach (var v in original)
        {
            result.Add((Vector3Int)v);
        }

        return result;
    }

    private List<Vector2Int> GetNeighbours(Vector2Int cellPos, bool useDiagonals = false, bool ignoreObstacles = false)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        void AddIfValid(Vector2Int position)
        {
            if (ignoreObstacles || m_walkableTilemap.GetTile((Vector3Int)position) != null)
                result.Add(position);
        }

        AddIfValid(cellPos + Vector2Int.up);
        AddIfValid(cellPos + Vector2Int.left);
        AddIfValid(cellPos + Vector2Int.right);
        AddIfValid(cellPos + Vector2Int.down);

        if (useDiagonals)
        {
            AddIfValid(cellPos + Vector2Int.up + Vector2Int.left);
            AddIfValid(cellPos + Vector2Int.up + Vector2Int.right);
            AddIfValid(cellPos + Vector2Int.down + Vector2Int.left);
            AddIfValid(cellPos + Vector2Int.down + Vector2Int.right);
        }

        return result;
    }

    public List<Vector3Int> GetVisibleTilesFrom(Vector3Int origin)
    {
        List<Vector3Int> result = new List<Vector3Int>();

        List<Vector3Int> scannedTiles = new List<Vector3Int>();
        Stack<Vector3Int> frontier = new Stack<Vector3Int>();

        frontier.Push(origin);
        scannedTiles.Add(origin);

        while (frontier.Count > 0)
        {
            Vector3Int tileTest = frontier.Pop();

            if (IsInLineOfSight(origin, tileTest))
            {
                result.Add(tileTest);

                var neighbours = GetNeighbours(tileTest, useDiagonals: true);

                foreach (var n in neighbours)
                {
                    if (!scannedTiles.Contains(n))
                    {
                        frontier.Push(n);
                        scannedTiles.Add(n);
                    }
                }
            }
        }

        return result;
    }
}
