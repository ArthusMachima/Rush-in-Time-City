using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{


    public static PlayerControl Instance;
    private void OnEnable()
    {
        Instance = this;
    }




    public AudioManager aud;

    [Header("Movement")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float speed;
    [SerializeField] float jumpforce;
    [SerializeField] float dashForce;
    public bool isSliding;
    [SerializeField] SpriteTrailMaker spriteTrail;

    [Header("Ground Detection")]
    [SerializeField] float groundCheckDistance = 0.1f;
    [SerializeField] float groundCheckHalfWidth = 0.4f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] bool onGround;

    [Header("Jump Buffering & Coyote Time")]
    [SerializeField] float coyoteTime = 0.15f;
    [SerializeField] float jumpBufferTime = 0.1f;

    [Header("Stamina")]
    public float stamina;
    [SerializeField] float staminaMax;
    [SerializeField] Transform hpBar;
    [SerializeField] bool isStaminaUsed;
    public GameObject StaminaPanel;

    [Header("Parry")]
    [SerializeField] GameObject weaponPivot;
    [SerializeField] float parryingDur = 0.05f;
    public bool isParrying;
    public bool isGuarding;

    [Header("Shoot")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform bulletPos;

    float coyoteTimer;
    float jumpBufferTimer;
    bool left, right;
    Vector2 direction;
    Vector2 lastDirection = new(1,0);

    private void Start()
    {
        aud = AudioManager.Instance;
        spriteTrail = GetComponent<SpriteTrailMaker>();
        spriteTrail.image = GetComponent<SpriteRenderer>().sprite;
        spriteTrail.enabled = false;
        rb = GetComponent<Rigidbody2D>();
        stamina = staminaMax;
    }

    private void Update()
    {
        if (rb == null) return;

        left = Input.GetKey(KeyCode.A);
        right = Input.GetKey(KeyCode.D);
        isGuarding = Input.GetMouseButton(1);

        if (onGround)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferTimer = jumpBufferTime;
        else
            jumpBufferTimer -= Time.deltaTime;

        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            Jump();
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }

        if (Input.GetKeyUp(KeyCode.Space) && rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);

        if (Input.GetKeyDown(KeyCode.LeftControl) && onGround && !isSliding)
        {
            Slide();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
            Dash();

        if (Input.GetMouseButtonDown(1))
        {
            weaponPivot.SetActive(true);
            isParrying = true;
            weaponPivot.LeanRotateZ(80, 0).setOnComplete(()=>{
                weaponPivot.LeanRotateZ(-80, parryingDur);
            });
            LeanTween.delayedCall(parryingDur, () =>
            {
                isParrying =false;
                weaponPivot.SetActive(false);
            });
        } 

        if (hpBar != null)
        {
            hpBar.localScale = new(stamina / staminaMax, transform.localScale.y, transform.localScale.z);
            if (stamina>=staminaMax) StaminaPanel.SetActive(false);
            else StaminaPanel.SetActive(true);
        }
        if (!isStaminaUsed && stamina < staminaMax) stamina++; 
    }

    void Slide()
    {
        aud.PlaySFX(aud.s_dash);
        if (stamina < 20) return;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(2 * jumpforce * lastDirection, ForceMode2D.Impulse);
        isSliding = true;
        LeanTween.delayedCall(2, () =>
        {
            isSliding = false;
        });

        stamina -= 20;
        isStaminaUsed = true;
        spriteTrail.enabled = true;
        LeanTween.delayedCall(0.5f, () =>
        {
            isStaminaUsed = false;
            spriteTrail.enabled = false;
        });
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpforce, ForceMode2D.Impulse);

    }

    public void Dash()
    {
        aud.PlaySFX(aud.s_dash);
        if (stamina<60) return;

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
        Vector2 dashDir = (mouseWorldPos - (Vector2)transform.position).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dashDir * dashForce, ForceMode2D.Impulse);

        stamina -= 60;
        isStaminaUsed = true;
        spriteTrail.enabled = true;
        LeanTween.delayedCall(0.5f, () =>
        {
            isStaminaUsed=false;
            spriteTrail.enabled = false;
        });
    }

    bool CastGroundRay(float xOffset)
    {
        Vector2 origin = (Vector2)transform.position + Vector2.right * xOffset;
        return Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer).collider != null;
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        onGround = CastGroundRay(0f) || CastGroundRay(-groundCheckHalfWidth) || CastGroundRay(groundCheckHalfWidth);

        direction = new Vector2((right ? 1 : 0) - (left ? 1 : 0), 0);
        if (left || right)
        {
            rb.AddForce(10 * speed * direction.normalized);
            lastDirection= direction;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = onGround ? Color.green : Color.red;
        foreach (float x in new[] { 0f, -groundCheckHalfWidth, groundCheckHalfWidth })
        {
            Vector2 origin = (Vector2)transform.position + Vector2.right * x;
            Gizmos.DrawLine(origin, origin + Vector2.down * groundCheckDistance);
        }
    }
}