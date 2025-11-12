using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("== Movimento ==")]
    public float speed = 5f;
    public float jumpForce = 8f;
    private bool canJump = false;
    private bool onJumpZone = false;
    private float moveInput;
    private Rigidbody2D rb;
    private Animator anim;

    [Header("== Ataque ==")]
    public float attackRange = 0.5f;
    public int attackDamage = 10;
    public LayerMask enemyLayer;

    [Header("== Energia ==")]
    public float maxEnergy = 100f;
    public float currentEnergy;
    public float energyUseRate = 5f;

    [Header("== Sistema de Sono ==")]
    public float dayLength = 60f; // tempo em segundos para o "dia"
    private float timePassed = 0f;
    private bool isAsleep = false;

    [Header("== Interação ==")]
    public Transform interactionPoint;
    public float interactionRange = 1.5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentEnergy = maxEnergy;
    }

    private void Update()
    {
        if (isAsleep) return;

        HandleMovement();
        HandleAttack();
        HandleEnergy();
        HandleInteraction();
        HandleSleepTimer();
    }

    // ========================================
    // MOVIMENTO E PULO
    // ========================================
    void HandleMovement()
    {
        moveInput = 0;

        if (Keyboard.current.aKey.isPressed) moveInput = -1;
        if (Keyboard.current.dKey.isPressed) moveInput = 1;

        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        anim.SetFloat("Speed", Mathf.Abs(moveInput));

        if (Keyboard.current.spaceKey.wasPressedThisFrame && canJump && onJumpZone)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            anim.SetTrigger("Jump");
            canJump = false;
            UseEnergy(5f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
            canJump = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        canJump = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PularPermitido"))
            onJumpZone = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PularPermitido"))
            onJumpZone = false;
    }

    // ========================================
    // ATAQUE
    // ========================================
    void HandleAttack()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            anim.SetTrigger("Attack");
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<Enemy>()?.TakeDamage(attackDamage);
            }

            UseEnergy(10f);
        }
    }

    // ========================================
    // ENERGIA
    // ========================================
    void HandleEnergy()
    {
        // Usa energia quando corre (segurando Shift)
        if (Keyboard.current.leftShiftKey.isPressed)
        {
            UseEnergy(energyUseRate * Time.deltaTime);
        }

        // Recupera energia ao comer (C)
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            GainEnergy(20f);
            Debug.Log("Você comeu algo. Energia restaurada!");
        }

        // Dormir manualmente (Z)
        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            Sleep();
        }
    }

    void UseEnergy(float amount)
    {
        currentEnergy = Mathf.Clamp(currentEnergy - amount, 0, maxEnergy);
        if (currentEnergy <= 0)
        {
            Debug.Log("Você ficou sem energia!");
            // aqui você pode adicionar penalidades ou fazer o personagem desmaiar
        }
    }

    void GainEnergy(float amount)
    {
        currentEnergy = Mathf.Clamp(currentEnergy + amount, 0, maxEnergy);
    }

    // ========================================
    // SISTEMA DE SONO
    // ========================================
    void HandleSleepTimer()
    {
        timePassed += Time.deltaTime;

        if (timePassed >= dayLength)
        {
            Debug.Log("Você desmaiou de sono...");
            isAsleep = true;
            anim.SetTrigger("Sleep");
            UseEnergy(30f);
            Invoke(nameof(WakeUpNextDay), 5f);
        }
    }

    void Sleep()
    {
        isAsleep = true;
        anim.SetTrigger("Sleep");
        GainEnergy(50f);
        Invoke(nameof(WakeUpNextDay), 3f);
    }

    void WakeUpNextDay()
    {
        timePassed = 0;
        isAsleep = false;
        Debug.Log("Novo dia começou!");
    }

    // ========================================
    // INTERAÇÃO ESPECIAL (ex: lixeira)
    // ========================================
    void HandleInteraction()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(interactionPoint.position, interactionRange);

            foreach (var hit in hits)
            {
                if (hit.CompareTag("Lixeira"))
                {
                    anim.SetTrigger("EnterTrash");
                    Debug.Log("Entrou na lixeira!");
                }
            }
        }
    }

    // ========================================
    // DEBUG GIZMOS
    // ========================================
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (interactionPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(interactionPoint.position, interactionRange);
        }
    }
}
