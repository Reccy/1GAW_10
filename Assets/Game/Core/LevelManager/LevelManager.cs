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

    private List<Character> m_characters;

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

        m_characters = new List<Character>();
    }

    public void RegisterCharacter(Character c)
    {
        m_characters.Add(c);
    }

    public void DeregisterCharacter(Character c)
    {
        m_characters.Remove(c);
    }

    public void NotifyMoved(Vector3Int from, Vector3Int to, Character c)
    {
        foreach (Character character in m_characters)
        {
            if (character == c)
                continue;

            if (character.CurrentCellPosition == to)
            {
                character.MoveTo(from);
            }
        }
    }
}
