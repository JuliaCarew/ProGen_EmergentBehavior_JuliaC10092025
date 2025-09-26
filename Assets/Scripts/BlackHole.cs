using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public float pullRadius = 3f;
    public float pullForce = 5f;
    public float lifeTime = 10f;

    void Update()
    {
        //lifeTime -= Time.deltaTime;
        //if (lifeTime <= 0) Destroy(gameObject);

        // find all colliders in range
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pullRadius);

        foreach (var hit in hits)
        {
            Star star = hit.GetComponent<Star>();
            if (star == null || !star.isAlive)
                continue;

            // push away stars
            Vector2 dir = (star.transform.position - transform.position).normalized;

            // apply movement/force to star’s direction
            star.freeMovementDir += dir * pullForce * Time.deltaTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, pullRadius);
    }
}
