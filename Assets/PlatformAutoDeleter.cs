using UnityEngine;

public class PlatformAutoDeleter : MonoBehaviour
{
    [SerializeField] GameObject ObjectToDelete;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(ObjectToDelete);
        }
    }
}
