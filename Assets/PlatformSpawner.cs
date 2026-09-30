using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] PlatformVariants;
    //[SerializeField] GameObject PreviousPlatform;
    [SerializeField] Vector3 PlatformSpawnOffset;
    [SerializeField] bool spawned;
    [SerializeField] bool isPlatform;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player") || spawned) return;
        if (GameManager.Instance.ClearPathway)
            Instantiate(PlatformVariants[^1], transform.position + PlatformSpawnOffset, Quaternion.identity);
        else
            Instantiate(PlatformVariants[Random.Range(0, PlatformVariants.Length - 1)], transform.position + PlatformSpawnOffset, Quaternion.identity);
        if (isPlatform) GameManager.Instance.sectorCurrent++;
        spawned = true;
    }
}
