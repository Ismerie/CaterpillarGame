using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;


public class PlayerController : MonoBehaviour
{
    [Header("Stat Player")]
    public int maxHP = 3;
    private int currentHP;
    public int CurrentHP => currentHP;
    public float invincibilityTime = 1f;

    [Header("UI")]
    public TextMeshProUGUI hpText;

    [Header("Respawn")]
    public Transform respawnPoint;   // à assigner dans l'Inspector
    public ScreenFader fader;        // à assigner dans l'Inspector

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Jump")]
    public float jumpForce = 7f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip damageSound;
    public AudioClip defeatSound;
    public AudioClip respawnSound;





    private Rigidbody2D rb;
    private Animator anim;

    private PlayerControls controls;
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool isGrounded;
    private bool facingRight = true;

    private bool isInvincible = false;
    private bool canControl = true;           // désactivé pendant la mort/respawn
    private Coroutine dieRoutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        controls = new PlayerControls();

        // READ MOVEMENT
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // READ JUMP
        controls.Player.Jump.performed += ctx => jumpPressed = true;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // On cherche le StartPoint pour les respawns
        if (respawnPoint == null)
        {
            StartPoint start = FindFirstObjectByType<StartPoint>();
            if (start != null)
            {
                respawnPoint = start.transform;
            }
        }

        // 🔹 CAS RESUME : on charge la position sauvegardée
        if (GameManager.Instance != null && GameManager.Instance.IsLoadingFromSave())
        {
            currentHP = GameManager.Instance.GetSavedHP();
            Vector2 savedPos = GameManager.Instance.GetSavedPosition();

            // On met le joueur à la dernière position connue
            transform.position = savedPos;

            // ❗ On NE TOUCHE PLUS au respawnPoint ici
            // respawnPoint reste le StartPoint, donc la mort renvoie au StartPoint.

            GameManager.Instance.OnPlayerLoadedFromSave();
        }
        else
        {
            // 🔹 CAS NORMAL / NEW GAME / RESTART : on spawn au StartPoint
            currentHP = maxHP;

            if (respawnPoint != null)
                transform.position = respawnPoint.position;
        }

        UpdateHPUI();
    }


    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        // Check ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Flip
        if (moveInput.x > 0 && !facingRight)
            Flip();
        else if (moveInput.x < 0 && facingRight)
            Flip();

        // Animation parameters
        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(moveInput.x));
            anim.SetBool("IsGrounded", isGrounded);
        }
    }

    private void FixedUpdate()
    {
        if (!canControl)
        {
            // pas de movement pendant la mort / respawn
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            jumpPressed = false;
            return;
        }

        // Horizontal movement
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // Jump (only once)
        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            
            if (audioSource && jumpSound)
                audioSource.PlayOneShot(jumpSound);
            // 🔹 Lancer l'animation de saut UNIQUEMENT quand on appuie sur jump
            if (anim != null)
                anim.SetTrigger("Jump");
        }

        jumpPressed = false;
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 s = transform.localScale;
        s.x *= -1f;
        transform.localScale = s;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    // ---- GESTION DES DÉGÂTS / MORT / RESPAWN ----

    public void TakeHit(int amount)
    {
        if (isInvincible) return;
        if (dieRoutine != null) return; // évite de prendre des dégâts pendant la séquence de mort

        currentHP -= amount;
        UpdateHPUI();

        if (audioSource && damageSound)
            audioSource.PlayOneShot(damageSound);

        if (anim != null)
            anim.SetTrigger("TakeDamage");

        if (currentHP > 0)
        {
            StartCoroutine(Invincibility());
        }
        else
        {
            // HP <= 0 -> mort

            if (dieRoutine == null)
                dieRoutine = StartCoroutine(DieAndRespawn());
        }
    }

    private IEnumerator Invincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }

    private IEnumerator DieAndRespawn()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RegisterDeath();

        canControl = false;
        rb.linearVelocity = Vector2.zero;

        if (audioSource && defeatSound)
            audioSource.PlayOneShot(defeatSound);
        
        if (anim != null)
            anim.SetTrigger("Die");   // animation Defeated

        // fade vers noir
        if (fader != null)
            yield return fader.FadeOut();

        // replacer au point de respawn
        if (respawnPoint != null)
            transform.position = respawnPoint.position;

        currentHP = maxHP;
        UpdateHPUI();

        if (audioSource && respawnSound)
            audioSource.PlayOneShot(respawnSound);

        if (anim != null)
            anim.SetTrigger("Respawn");   // animation wake-up

        // fade du noir vers transparent
        if (fader != null)
            yield return fader.FadeIn();

        canControl = true;
        dieRoutine = null;
    }

    // méthode si tu veux respawn manuellement depuis un autre script (optionnel)
    public void Respawn(Vector3 respawnPosition)
    {
        transform.position = respawnPosition;
        currentHP = maxHP;

        if (anim != null)
            anim.SetTrigger("Respawn");
    }

    private void UpdateHPUI()
    {
        if (hpText != null)
            hpText.text = "HP: " + currentHP + "/" + maxHP;
    }

}
