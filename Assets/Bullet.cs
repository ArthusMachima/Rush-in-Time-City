
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody2D rb;
    public Transform shooter;
    public Transform target;
    [SerializeField] float speed;
    public bool isDeflected;
    [SerializeField] GameObject explosion;
    [SerializeField] GameObject strikenParticle;



    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (target!=null) rb.AddForce((target.transform.position - transform.position).normalized * speed, ForceMode2D.Impulse);
        Destroy(gameObject, 5);
    }

    public void OnPlayerParried()
    {
        isDeflected = true;
        rb.linearVelocity = new Vector2(0, 0);
        if (shooter != null) rb.AddForce((shooter.position - transform.position).normalized * speed, ForceMode2D.Impulse);
        GameManager.Instance.DoParryEffect();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySFX(AudioManager.Instance.s_explode);

        collision.gameObject.TryGetComponent<PlayerControl>(out PlayerControl player);
        if (player!=null)
        {
            if (player.isParrying) OnPlayerParried();
            else if (player.isGuarding)
            {
                player.aud.PlaySFX(player.aud.s_nullify);
                Instantiate(strikenParticle, transform.position, Quaternion.identity);
                player.stamina -= 10;
                Destroy(gameObject);
            }
            else
            {
                collision.rigidbody.linearVelocity = new Vector2(0, 0); 
                Destroy(gameObject);
            }
            

        }
        else
        {
            Destroy(gameObject);
        }


    }
}