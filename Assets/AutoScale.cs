using UnityEngine;

public class AutoScale : MonoBehaviour
{
    [SerializeField] float scale;
    [SerializeField] float alpha;
    [SerializeField] float time = 0.2f;
 
    void Start()
    {
        transform.LeanScale(new(scale, scale, scale), time);
        gameObject.LeanColor(new(1, 1, 1, alpha), time).setOnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}
