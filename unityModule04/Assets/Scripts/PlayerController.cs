using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Stat Player")]
    public int maxHP = 3;
    private int currentHP;
    
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Jump")]
    public float jumpForce = 7f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;

    private PlayerControls controls;
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool isGrounded;
    private bool facingRight = true;

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
        currentHP = maxHP;
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
        // Horizontal movement
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // Jump (only once)
        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
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

    public void TakeHit(int amount)
    {
        currentHP -= amount;

        if (anim != null)
            anim.SetTrigger("TakeDamage");

        if (currentHP <= 0)
            Die();
    }

    private void Die()
    {
        if (anim != null)
            anim.SetTrigger("Die");

        // Optionnel : désactiver le mouvement
        // rb.velocity = Vector2.zero;
        // enabled = false; // désactive ce script
    }

    public void Respawn(Vector3 respawnPosition)
    {
        transform.position = respawnPosition;
        currentHP = maxHP;

        if (anim != null)
            anim.SetTrigger("Respawn");

        // Optionnel : réactiver le mouvement
        // enabled = true;
    }

}
