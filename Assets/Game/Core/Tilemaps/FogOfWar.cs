using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogOfWar : MonoBehaviour
{
    private BoundsInt m_bounds;
    private Tilemap Tilemap => LevelManager.Instance.FogOfWarTilemap;
    private PlayerInteractionManager m_pim;

    private Vector3Int m_prevCellPos = Vector3Int.zero;

    [SerializeField] private TileBase m_tile;

    private void Awake()
    {
        m_bounds = LevelManager.Instance.WalkableTilemap.cellBounds;
        m_pim = FindObjectOfType<PlayerInteractionManager>();
    }

    private void Start()
    {
        m_prevCellPos = m_pim.CurrentBrain.CurrentCellPosition;

        UpdateFog();
    }

    private void Update()
    {
        if (m_pim.CurrentBrain.CurrentCellPosition != m_prevCellPos)
        {
            m_prevCellPos = m_pim.CurrentBrain.CurrentCellPosition;
            UpdateFog();
        }
    }

    private void UpdateFog()
    {
        List<Vector3Int> visiblePositions = LevelManager.Instance.GetVisibleTilesFrom(m_prevCellPos);
        Vector3Int startPos = new Vector3Int(m_bounds.xMin, m_bounds.yMin, 0);
        Vector3Int endPos = new Vector3Int(m_bounds.xMax, m_bounds.yMax, 0);
        
        Fill(Tilemap, m_tile, startPos, endPos, visiblePositions);
    }

    private void Fill(Tilemap map, TileBase tile, Vector3Int start, Vector3Int end, List<Vector3Int> ignore)
    {
        map.ClearAllTiles();

        for (int x = start.x; x < end.x; ++x)
        {
            for (int y = start.y; y < end.y; ++y)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                if (!ignore.Contains(pos))
                {
                    map.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
        }
    }
}
