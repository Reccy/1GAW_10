using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerCharacter : MonoBehaviour
{
    private Character m_character;

    private const int PLAYER_ID = 0;
    private Player m_player;

    #region Inputs
    private int m_moveVInput = 0;
    private int m_moveHInput = 0;
    #endregion

    private void Awake()
    {
        m_player = ReInput.players.GetPlayer(PLAYER_ID);
        m_character = GetComponent<Character>();
    }

    private void Update()
    {
        CheckInputs();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void CheckInputs()
    {
        if (m_player.GetButtonDown("MoveUp"))
        {
            m_moveVInput = 1;
        }
        else if (m_player.GetButtonDown("MoveDown"))
        {
            m_moveVInput = -1;
        }

        if (m_player.GetButtonDown("MoveLeft"))
        {
            m_moveHInput = -1;
        }
        else if (m_player.GetButtonDown("MoveRight"))
        {
            m_moveHInput = 1;
        }
    }

    private void HandleMovement()
    {
        if (m_moveHInput == 1)
        {
            m_character.MoveRight();
            m_moveHInput = 0;
        }
        else if (m_moveHInput == -1)
        {
            m_character.MoveLeft();
            m_moveHInput = 0;
        }

        if (m_moveVInput == 1)
        {
            m_character.MoveUp();
            m_moveVInput = 0;
        }
        else if (m_moveVInput == -1)
        {
            m_character.MoveDown();
            m_moveVInput = 0;
        }
    }
}
