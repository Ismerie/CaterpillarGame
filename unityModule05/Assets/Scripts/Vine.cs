using UnityEngine;

public class Vine : MonoBehaviour
{
    public float activationDistance = 2f;
    public int damage = 1;

    private Animator anim;
    private Transform player;
    private bool isActive = false;

    private void Start()
    {
        anim = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        if (player == null)
            return;

        float dist = Vector2.Distance(transform.position, player.position);

        // La liane commence à bouger quand la chenille est proche
        if (!isActive && dist <= activationDistance)
        {
            isActive = true;

            if (anim != null)
                anim.SetTrigger("Move");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si la liane touche la chenille → dégâts
        if (isActive && other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeHit(damage);
            }
        }
    }
}
