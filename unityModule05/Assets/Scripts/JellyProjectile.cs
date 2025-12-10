using UnityEngine;

public class JellyProjectile : MonoBehaviour
{
    public int damage = 1;
    public float lifeTime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si on touche le joueur
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeHit(damage);
            }

            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            // Si on touche un mur, le sol, etc. (un collider solide)
            Destroy(gameObject);
        }
    }
}
