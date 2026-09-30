using UnityEngine;

public class JumperAnimation : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            transform.LeanScale(new(1.5f, 1.5f, 1.5f), 0.3f).setEaseOutQuint();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            transform.LeanScale(new(1f, 1f, 1f), 0.3f).setEaseOutQuint();
        }
    }
}
