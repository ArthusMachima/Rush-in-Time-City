using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject ObjectToSpawn;
    [SerializeField] float spawnrate;
    [SerializeField] bool randomizedTime;
    float time;
    float ogspawnrate;

    private void Start()
    {
        ogspawnrate = spawnrate;
        if (randomizedTime) spawnrate = Random.Range(2, ogspawnrate);
    }

    private void Update()
    {
        

        if (time > spawnrate)
        {
            Instantiate(ObjectToSpawn, transform.position, Quaternion.identity);
            time = 0;
            if (randomizedTime) spawnrate = Random.Range(2, ogspawnrate);
        }
        else time += Time.deltaTime;
    }
}
