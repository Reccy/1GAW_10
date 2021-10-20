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
    private int m_uiHInput = 0;
    private int m_uiVInput = 0;
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

        if (m_player.GetButtonDown("UIUp"))
        {
            m_uiVInput = 1;
        }
        else if (m_player.GetButtonDown("UIDown"))
        {
            m_uiVInput = -1;
        }

        if (m_player.GetButtonDown("UILeft"))
        {
            m_uiHInput = -1;
        }
        else if (m_player.GetButtonDown("UIRight"))
        {
            m_uiHInput = 1;
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
        m_uiHInput = default;
        m_uiVInput = default;
        m_actionInput = default;
        m_cancelInput = default;
    }

    public int MoveVInput() => GetAndReset(ref m_moveVInput);
    public int MoveHInput() => GetAndReset(ref m_moveHInput);
    public int UIHInput() => GetAndReset(ref m_uiHInput);
    public int UIVInput() => GetAndReset(ref m_uiVInput);
    public bool ActionInput() => GetAndReset(ref m_actionInput);
    public bool CancelInput() => GetAndReset(ref m_cancelInput);

    private T GetAndReset<T>(ref T value)
    {
        var res = value;
        value = default;
        return res;
    }
}
