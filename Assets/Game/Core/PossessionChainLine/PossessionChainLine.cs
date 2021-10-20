using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class PossessionChainLine : MonoBehaviour
{
    private List<Brain> m_brains;
    private Polyline m_line;

    private void Awake()
    {
        m_line = GetComponent<Polyline>();
    }

    public void SetStack(List<Brain> brains)
    {
        m_brains = brains;
    }

    private void Update()
    {
        List<Vector3> points = new List<Vector3>();

        foreach (Brain b in m_brains)
        {
            points.Add(b.transform.position + Vector3.back);
        }

        m_line.SetPoints(points);
    }
}
