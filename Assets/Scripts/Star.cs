using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Star : MonoBehaviour
{
    public int age;
    public float brightness = 0.1f;

    public int currentSegments = 0;
    public int maxSegments = 5;

    // stars will need to track their connections to other stars
    public List<Star> connectedStars = new List<Star>();
    public bool isAlive = true;

    // detection
    public float detectionRadius = 1f;

    void Start()
    {
        StarManager.allStars.Add(this);
        age = 0;
    }
    void Update()
    {
        
    }

    void Age()
    {
        age++;
        if (age > 1000) Die(); // star dies after 1000 frames
        if (isAlive)
        {
            Blink();
        }
    }

    public void ConnectToStars(List<Star> allStars)
    {
        List<Star> nearbyStars = new List<Star>();

        // detect nearby stars
        foreach (Star otherStar in allStars)
        {
            if (otherStar == this || !otherStar.isAlive || connectedStars.Contains(otherStar))
                continue;

            float distance = Vector2.Distance(transform.position, otherStar.transform.position);
            if (distance <= detectionRadius)
            {
                nearbyStars.Add(otherStar);
            }
        }

        nearbyStars.Sort((a, b) => b.age.CompareTo(a.age)); // sort by age, oldest first

        // connect to nearby stars
        foreach (Star target in nearbyStars)
        {
            if (connectedStars.Count >= maxSegments) break;
            if (target.connectedStars.Count >= target.maxSegments) continue;

            float distance = Vector2.Distance(transform.position, target.transform.position);
            AttachSegment(target);
            currentSegments++;
        }
    }

    public void AttachSegment(Star targetStar)
    {
        if (connectedStars.Count >= maxSegments || targetStar == this || connectedStars.Contains(targetStar))
            return;

        if (targetStar.connectedStars.Count >= targetStar.maxSegments)
            return;

        connectedStars.Add(targetStar); // add the target star to the connections
        targetStar.connectedStars.Add(this); // target star also needs to know about the connection

        brightness += 0.2f; // increase brightness by 20% for each segment
        targetStar.brightness += 0.2f;
    }

    public void Blink()
    {
        float blinkChance = UnityEngine.Random.value;
        if (blinkChance < 0.3f) brightness = 0.5f; // 30% chance to go to 50% brightness
        else if (blinkChance < 0.6f) brightness = 0.1f; // 30% chance to go to 10% brightness
        //else Die(); // 40% chance to die
    }

    public void Die()
    {
        isAlive = false;
        // remove all connections
        foreach (Star star in connectedStars)
        {
            star.connectedStars.Remove(this);
            star.brightness -= 0.2f; // decrease brightness of connected stars
        }
        connectedStars.Clear();
        brightness = 0f; // star is dead, no brightness

        // will need to notify segments to stop drawing lines
        // delete instance of star prefab
    }
}
// make stars blink between brightness levels