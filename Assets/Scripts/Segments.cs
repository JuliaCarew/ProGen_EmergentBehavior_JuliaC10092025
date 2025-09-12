using UnityEngine;
using System.Collections.Generic;

public class Segments : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Star star;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        star = GetComponent<Star>();

        lineRenderer.widthMultiplier = 0.05f;
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
    }

    void Update()
    {
        DrawSegments();
    }
    
    void DrawSegments()
    {
        if (star.connectedStars.Count > 0)
        {
            lineRenderer.positionCount = star.connectedStars.Count * 2;
            int index = 0;
            foreach (Star connectedStar in star.connectedStars)
            {
                lineRenderer.SetPosition(index++, transform.position);
                lineRenderer.SetPosition(index++, connectedStar.transform.position);
            }
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }
}
