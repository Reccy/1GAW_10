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
    }
}
