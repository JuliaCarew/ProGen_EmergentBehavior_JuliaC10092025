using System.Collections.Generic;
using UnityEngine;

public class Constellation : MonoBehaviour
{
    public List<Star> stars = new List<Star>();
    public Vector2 movementDirection;

    void Update()
    {
        if (stars.Count == 0) return;

        UpdateMoveDirection();
        MoveConstellation();
    }

    void UpdateMoveDirection()
    {
        // find oldest star and move in the direction it is going
        Star oldest = null;
        int maxAge = int.MinValue;

        foreach (Star s in stars)
        {
            if (s.isAlive && s.age > maxAge)
            {
                maxAge = (int)s.age;
                oldest = s;
            }
        }

        if (oldest != null)
        {
            // derive velocity from position change
            Vector2 velocity = ((Vector2)oldest.transform.position - oldest.lastPosition) / Time.deltaTime;
            movementDirection = velocity.normalized;; // follow oldest star
        }
    }

    void MoveConstellation()
    {
        if (movementDirection == Vector2.zero) return;

        foreach (Star star in stars)
        {
            if (star.isAlive)
            {
                star.transform.Translate(movementDirection * Time.deltaTime);
            }
        }
    }
}
