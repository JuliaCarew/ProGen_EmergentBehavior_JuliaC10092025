using System;
using System.Collections.Generic;
using UnityEngine;

public class StarManager : MonoBehaviour
{
    public GameObject starPrefab;
    public int initialStars = 50;

    public static List<Star> allStars = new List<Star>();

    void Start()
    {
        GenerateStars();
    }

    void Update()
    {
        ConnectStars();
    }
    
    void GenerateStars()
    {
        for (int i = 0; i < initialStars; i++)
        {
            Vector2 position = new Vector2(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f));
            Instantiate(starPrefab, position, Quaternion.identity);
            Debug.Log($"Generated Star at Position: {position}"); 
        }
    }


    void ConnectStars()
    {
        foreach (Star star in allStars)
        {
            star.ConnectToStars(allStars);
            Debug.Log($"Star at {star.transform.position} connected to {star.connectedStars.Count} stars.");
        }
    }
}
