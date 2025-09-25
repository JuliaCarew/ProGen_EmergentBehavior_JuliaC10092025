using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Star : MonoBehaviour
{
    #region variables

    [Header("Aging Settings")]
    public float maxBrightness = 1f;
    public float minLifetime = 5f;
    public float maxLifetime = 50f;
    private float lifetime;
    public float age;
    public float brightness = 0.1f;
    // fuel - for interesting emergent rules
    public float baseFuel = 1000f;        // base lifetime in frames or seconds
    public float coolingFactor = 1f;      // multiplier on how fast it dies when in constellation with many stars

    [Header("Emergent Properties")]
    public float size = 1f;
    private int giantThreshold;
    public bool isGiant = false;

    [Header("Black Hole Settings")]
    public float deathTimer = 10f; // how long before it can die
    public float unconnectedTimer = 0f; // how long it's been without connections
    public float unconnectedThreshold = 3f; // time until black hole
    public GameObject blackHolePrefab;

    [Header("Connections")]
    public int currentSegments = 0;
    public int maxSegments = 5;
    public List<Star> connectedStars = new List<Star>(); // stars will need to track their connections to other stars
    public bool isAlive = true;

    [Header("Detection / Movement")]
    public float detectionRadius = 1f;
    public Vector2 freeMovementDir;
    [HideInInspector] public Vector2 lastPosition; // get previous movement for constellation movement

    private SpriteRenderer sr;

    #endregion

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        UpdateAppearance();
        StarManager.allStars.Add(this);
    }

    void Start()
    {
        lifetime = UnityEngine.Random.Range(minLifetime, maxLifetime);
        freeMovementDir = UnityEngine.Random.insideUnitCircle.normalized;
        lastPosition = transform.position;
        giantThreshold = UnityEngine.Random.Range(2, 4);

        // initialise lifetime
        baseFuel = 1000f;
        coolingFactor = 1f;
    }

    void OnDestroy()
    {
        // unregister when dying 
        StarManager.allStars.Remove(this);
    }

    void Update()
    {
        if (!isAlive) return;

        age += Time.deltaTime;

        // brightness increases with age
        float normalisedAge = (age * coolingFactor) / lifetime;
        brightness = Mathf.Lerp(0.1f, maxBrightness, normalisedAge);

        // die when lifetime exceeded
        if (normalisedAge >= 1f)
        {
            StartCoroutine(Die());
            return;
        }

        Blink();
        FreeMove();
        //UpdateVisuals();

        // Giant star check
        if (!isGiant && connectedStars.Count >= giantThreshold)
        {
            Debug.Log($"{name} became GIANT!");
            BecomeGiant();
        }

        // Unconnected star timer
        if (connectedStars.Count == 0)
        {
            unconnectedTimer += Time.deltaTime;
            if (unconnectedTimer >= unconnectedThreshold)
            {
                Debug.Log($"{name} became BLACK HOLE!");
                BecomeBlackHole();
            }
        }
        else
        {
            unconnectedTimer = 0f;
        }

        lastPosition = transform.position; // store position
    }

    void FreeMove()
    {
        if (connectedStars.Count == 0)
        {
            transform.Translate(freeMovementDir * Time.deltaTime);
        }
    }

    void BecomeGiant()
    {
        isGiant = true;
        size *= 2.5f;
        brightness *= 1.9f;
        deathTimer *= 8f; // last longer
        UpdateAppearance();
    }

    void BecomeBlackHole()
    {
        // spawn a black hole prefab at this star’s position
        Instantiate(blackHolePrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void UpdateAppearance()
    {
        if (sr != null)
        {
            sr.color = Color.white * brightness;
            transform.localScale = Vector3.one * size;
        }
    }

    public void OnConnectedStarAdded()
    {
        // Called when a star connects to this one
        brightness *= 1.2f; // brighten 20% on first connection
        deathTimer *= 0.9f; // cool faster
        UpdateAppearance();
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
        
        // boost brightness 20%
        brightness = Mathf.Min(brightness + 0.2f, 1f);
        targetStar.brightness = Mathf.Min(targetStar.brightness + 0.2f, 1f);

        // increase cooling factor for both stars
        coolingFactor += 0.1f; // each new link burns 10% faster
        targetStar.coolingFactor += 0.1f;
    }

    public IEnumerator Die()
    {
        isAlive = false;
        // remove all connections
        foreach (Star star in connectedStars)
            star.connectedStars.Remove(this);

        connectedStars.Clear();

        float t = 0f;
        Color startColor = sr.color;

        if (t < 1f)
        {
            t += Time.deltaTime;
            sr.color = Color.Lerp(startColor, Color.black, t);
            yield return null;
        }
        Destroy(gameObject);
    }

    void Blink()
    {
        if (UnityEngine.Random.value < 0.01f) // 1% chance each frame
        {
            int ev = UnityEngine.Random.Range(0, 3);
            switch (ev)
            {
                case 0: // die early
                    StartCoroutine(Die());
                    break;
                case 1: // dim
                    brightness *= 0.5f;
                    break;
                case 2: // disconnect
                    if (connectedStars.Count > 0)
                    {
                        Star other = connectedStars[0];
                        connectedStars.Remove(other);
                        other.connectedStars.Remove(this);
                    }
                    break;
            }
        }
    }
}
// make stars blink between brightness levels