using System.Collections;
using UnityEngine;

public class VineHazard : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 1;
    public float damageDelay = 0.5f;  // délai avant d'infliger les dégâts
    public string playerTag = "Player";

    [Header("Animation")]
    public string attackTriggerName = "Attack";

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip attackSound;
    public float soundDelay = 1f;       // délai avant de jouer le son
    public float soundCooldown = 0.5f;  // temps avant de relancer une attaque

    private Animator anim;
    private bool busy = false;  // empêche d’enchaîner les attaques trop vite

    private void Awake()
    {
        anim = GetComponentInParent<Animator>();
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>() ?? GetComponentInParent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        PlayerController player = other.GetComponent<PlayerController>();

        // 🔹 Animation d'attaque immédiate
        if (anim != null && !string.IsNullOrEmpty(attackTriggerName))
            anim.SetTrigger(attackTriggerName);

        // 🔹 Lancer attaque complète (son + dégâts + cooldown)
        if (!busy)
            StartCoroutine(AttackSequence(player));
    }

    private IEnumerator AttackSequence(PlayerController player)
    {
        busy = true;

        // ⏱ DÉLAI avant le son
        yield return new WaitForSeconds(soundDelay);

        // 🔊 Jouer le son
        if (audioSource != null && attackSound != null)
            audioSource.PlayOneShot(attackSound);

        // ⏱ DÉLAI avant les dégâts
        yield return new WaitForSeconds(damageDelay);

        // 💥 Infliger dégâts
        if (player != null)
            player.TakeHit(damage);

        // ⏱ Attendre fin du son + cooldown
        float wait = (attackSound != null ? attackSound.length : 0f) + soundCooldown;
        if (wait > 0)
            yield return new WaitForSeconds(wait);

        busy = false;
    }
}
