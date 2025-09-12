using System.Collections.Generic;
using UnityEngine;

public class Constellation : MonoBehaviour
{
    public List<Star> stars = new List<Star>();
    public Vector2 movementDirection;

    void Update()
    {
        MoveConstellation();
        HandleAging();
    }

    void MoveConstellation()
    {
        foreach (Star star in stars)
        {
            if (star.isAlive)
            {
                star.transform.Translate(movementDirection * Time.deltaTime);
            }
        }
    }

    void HandleAging()
    {
        foreach (Star star in stars)
        {
            if (star.isAlive)
            {
                star.age++;
                //if (star.age > 100) star.Blink();
            }
        }
    }
}
