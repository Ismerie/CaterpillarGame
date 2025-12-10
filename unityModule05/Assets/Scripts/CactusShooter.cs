using System.Collections;
using UnityEngine;

public class CactusShooter : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject jellyPrefab;   // Prefab de la jelly
    public Transform shootPoint;     // Point de spawn
    public float shootForce = 5f;
    public float fireRate = 1.5f;    // Temps entre deux tirs
    public Vector2 shootDirection = new Vector2(-1, -0.5f);

    [Header("Player detection")]
    public string playerTag = "Player";

    [Header("Animation")]
    public string awakeBoolName = "isAwake";  // Trigger dans l'Animator du cactus (optionnel)

    private bool playerInRange = false;
    private Coroutine shootRoutine;
    private Animator anim;

    public AudioSource audioSource;
    public AudioClip shootSound;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        playerInRange = true;

        if (shootRoutine == null)
            shootRoutine = StartCoroutine(ShootLoop());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        playerInRange = false;

        if (shootRoutine != null)
        {
            StopCoroutine(shootRoutine);
            shootRoutine = null;
        }

        if (anim != null && !string.IsNullOrEmpty(awakeBoolName))
        {
            anim.SetBool(awakeBoolName, false);
        }

    }

    private IEnumerator ShootLoop()
    {
        // On normalise une fois pour toutes
        Vector2 dir = shootDirection.normalized;

        while (playerInRange)
        {
            if (audioSource && shootSound)
                audioSource.PlayOneShot(shootSound);
            // Animation du cactus (optionnel)
            if (anim != null && !string.IsNullOrEmpty(awakeBoolName))
            {
                anim.SetBool(awakeBoolName, true);
            }

            // Instancier la jelly
            GameObject jelly = Instantiate(jellyPrefab, shootPoint.position, Quaternion.identity);

            Rigidbody2D rb = jelly.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = dir * shootForce;
            }

            yield return new WaitForSeconds(fireRate);
        }
    }
}
