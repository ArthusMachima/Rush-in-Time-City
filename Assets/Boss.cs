using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] Transform target;
    [SerializeField] Transform spawnPosition;
    [SerializeField] float fireRate;
    [SerializeField] float time;
    [SerializeField] GameObject strikenParticle;

    [Header("Charging Particle")]
    [SerializeField] GameObject chargingParticle;
    [SerializeField] bool isChargeParticleSpawned;
    [SerializeField] float chargeSpawnTime = 0.3f;

    [Header("Detection Settings")]
    public float detectionRadius = 3f;
    public LayerMask targetLayers;
    public float checkInterval = 0.2f;
    private float detectionTimer;

    [Header("Boss States")]
    [SerializeField] Follow follow;
    [SerializeField] bool isFollowing;
    int state;

    private void Start()
    {
        follow = GetComponent<Follow>();
    }

    private void Update()
    {
        if (detectionTimer >= checkInterval)
        {
            detectionTimer = 0f;
            DetectNearby();
        }
        else detectionTimer += Time.deltaTime;

        if (state==0)
        {
            if (bullet != null && spawnPosition != null && target != null)
            {
                if (fireRate < time)
                {
                    GameObject pellet = Instantiate(bullet, spawnPosition.position, Quaternion.identity);
                    pellet.GetComponent<Bullet>().shooter = transform;
                    pellet.GetComponent<Bullet>().target = target;
                    time = 0;
                    isChargeParticleSpawned = false;
                }
                else
                {
                    time += Time.deltaTime;
                    if (fireRate - chargeSpawnTime < time && !isChargeParticleSpawned)
                    {
                        Instantiate(chargingParticle, spawnPosition.position, Quaternion.identity);
                        isChargeParticleSpawned = true;
                    }
                }
            }
        }
    }

    void DetectNearby()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject) continue; // skip self

            if (hit.gameObject.CompareTag("Player"))
            {
                target = hit.transform;
                if (!isFollowing)
                {
                    GameManager.Instance.ClearPathway = true;
                    follow.SetPos(target);
                    isFollowing = true;
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius); // still works in 2D
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            if (bullet != null && bullet.isDeflected)
            {
                Instantiate(strikenParticle, spawnPosition.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerControl player = collision.gameObject.GetComponent<PlayerControl>();
            if (player != null && player.isSliding)
            {
                Instantiate(strikenParticle, spawnPosition.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.ClearPathway = false;
    }
}
