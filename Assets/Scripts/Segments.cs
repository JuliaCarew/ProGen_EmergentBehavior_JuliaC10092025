using UnityEngine;
using System.Collections.Generic;

public class Segments : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Star star;

    public float maxDrawDistance = 2f;

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
        if (star == null) return;

        List<Star> validStars = new List<Star>();
        foreach (Star connectedStar in star.connectedStars)
        {
            if (connectedStar == null || connectedStar.gameObject == null) continue;

            float distance = Vector2.Distance(transform.position, connectedStar.transform.position);
            if (distance <= maxDrawDistance)
                validStars.Add(connectedStar);
        }

        // clean up any null / dead stars
        star.connectedStars.RemoveAll(s => s == null || s.gameObject == null);

        if (validStars.Count > 0)
        {
            lineRenderer.positionCount = validStars.Count * 2;
            int index = 0;
            foreach (Star connectedStar in validStars)
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
