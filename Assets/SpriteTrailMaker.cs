using UnityEngine;

public class SpriteTrailMaker : MonoBehaviour
{
    float time;
    [SerializeField] float spriteRate = 0.05f;
    [SerializeField] float ghostLifetime = 0.5f;
    [SerializeField] int sortingOrder = -1;
    [SerializeField] Color ghostColor = new(1, 1, 1, 0.5f);

    SpriteRenderer _sr; // cache instead of GetComponent every spawn

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        time += Time.deltaTime;

        if (time < spriteRate) return;
        time = 0;

        GameObject ghost = new("Ghost");
        ghost.transform.SetPositionAndRotation(transform.position, transform.rotation);
        ghost.transform.localScale = transform.localScale;

        SpriteRenderer sr = ghost.AddComponent<SpriteRenderer>();
        sr.sprite = _sr != null ? _sr.sprite : image; // always in sync with current sprite
        sr.color = ghostColor;
        sr.sortingOrder = sortingOrder;

        Destroy(ghost, ghostLifetime);
    }

    public Sprite image; // fallback if no SpriteRenderer on this object
}