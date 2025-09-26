using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public float pullRadius = 3f; // area ppulled
    public float pullForce = 5f; // how strong the pull is
    public float lifeTime = 10f; // how long before it dies
    public int starsOnDeath = 15; // how many stars to spawn when destroyed

    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
            return;
        }

        // find all colliders in range
        for (int i = StarManager.allStars.Count - 1; i >= 0; i--)
        {
            Star star = StarManager.allStars[i];
            if (star == null || !star.isAlive) continue;

            float dist = Vector2.Distance(transform.position, star.transform.position);
            if (dist <= pullRadius)
            {
                // PULL stars toward the black hole
                Vector2 dir = ((Vector2)transform.position - (Vector2)star.transform.position).normalized;
                star.freeMovementDir += dir * pullForce * Time.deltaTime;
            }
        }
    }

    void OnDestroy()
    {
        // when black hole dies, spawn stars
        if (StarManager.instance != null && StarManager.instance.starPrefab != null)
        {
            for (int i = 0; i < Random.Range(5,starsOnDeath); i++)
            {
                Vector2 position = (Vector2)transform.position + Random.insideUnitCircle * 2f;
                GameObject newStar = Instantiate(StarManager.instance.starPrefab, position, Quaternion.identity);
                // optionally tweak its freeMovementDir so they burst outward
                newStar.GetComponent<Star>().freeMovementDir = Random.insideUnitCircle.normalized;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, pullRadius);
    }
}
