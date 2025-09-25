using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public float pullRadius = 3f;
    public float pullForce = 5f;
    public float lifeTime = 10f;

    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0) Destroy(gameObject);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pullRadius);
        foreach (var hit in hits)
        {
            Star star = hit.GetComponent<Star>();
            if (star != null)
            {
                // Pull star toward black hole
                Vector2 dir = (transform.position - star.transform.position).normalized;
                star.transform.position = Vector2.MoveTowards(star.transform.position, transform.position, pullForce * Time.deltaTime);

                // destroy star if too close
                if (Vector2.Distance(star.transform.position, transform.position) < 0.2f)
                {
                    Destroy(star.gameObject);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, pullRadius);
    }
}
