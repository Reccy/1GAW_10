using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteFollower : MonoBehaviour
{
    private GameObject m_follower;

    private float m_followSpeed = 25.0f;

    private void Awake()
    {
        m_follower = transform.parent.gameObject;
        gameObject.name = transform.parent.name + " Sprite";
        transform.parent = null;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, m_follower.transform.position, Time.deltaTime * m_followSpeed);
    }
}
