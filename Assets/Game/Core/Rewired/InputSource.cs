using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class InputSource : MonoBehaviour
{
    private const int PLAYER_ID = 0;
    private Player m_player;

    private int m_moveVInput = 0;
    private int m_moveHInput = 0;
    private bool m_actionInput = false;
    private bool m_cancelInput = false;

    private void Awake()
    {
        m_player = ReInput.players.GetPlayer(PLAYER_ID);
    }

    private void Update()
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

        if (m_player.GetButtonDown("Action"))
        {
            m_actionInput = true;
        }

        if (m_player.GetButtonDown("Cancel"))
        {
            m_cancelInput = true;
        }
    }

    public void Clear()
    {
        m_moveHInput = default;
        m_moveVInput = default;
        m_actionInput = default;
        m_cancelInput = default;
    }

    public int MoveVInput()
    {
        var res = m_moveVInput;
        m_moveVInput = 0;
        return res;
    }

    public int MoveHInput()
    {
        var res = m_moveHInput;
        m_moveHInput = 0;
        return res;
    }

    public bool ActionInput()
    {
        var res = m_actionInput;
        m_actionInput = false;
        return res;
    }

    public bool CancelInput()
    {
        var res = m_cancelInput;
        m_cancelInput = false;
        return res;
    }
}
