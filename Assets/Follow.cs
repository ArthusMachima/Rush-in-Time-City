
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class Follow : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 Offset;
    [SerializeField] private float FollowSpeed = 10;
    [SerializeField] private float FollowDistance = 0;
    public bool doFollow;
    [SerializeField] private bool DisableZ;
    [SerializeField] private bool DisableY;
    [SerializeField] bool MoveOnlyRight;
    [SerializeField] private bool ControlDistanceWithMouse;
    private float ogspeed;

    private void Start()
    {
        ogspeed = FollowSpeed;
    }

    public void SetPos(Transform pos)
    {
        target = pos;
    }

    public void SetSpeed(float speed)
    {
        FollowSpeed = speed;
    }

    public void ResetSpeed()
    {
        FollowSpeed = ogspeed;
    }

    void Update()
    {
        if (doFollow && target != null)
        {
            Vector3 direction = (transform.position - target.position).normalized;
            Vector3 targetPosition = target.position + direction * FollowDistance;

            if (DisableZ) targetPosition.z = transform.position.z;
            if (DisableY) targetPosition.y = transform.position.y;

            // --- ADD THIS LINE ---
            // Ensure the target X is never smaller than current X
            if (MoveOnlyRight) targetPosition.x = Mathf.Max(transform.position.x, targetPosition.x + Offset.x);
            // ---------------------

            // Note: I moved Offset.x into the Max check above to ensure the 
            // final destination (with offset) respects the "right-only" rule.
            if (MoveOnlyRight) transform.position = Vector3.Lerp(transform.position, targetPosition + new Vector3(0, Offset.y, Offset.z), Time.deltaTime * FollowSpeed);
            else transform.position = Vector3.Lerp(transform.position, targetPosition + new Vector3(Offset.x, Offset.y, Offset.z), Time.deltaTime * FollowSpeed);

            if (ControlDistanceWithMouse) FollowDistance -= Input.GetAxis("Mouse ScrollWheel") * 5;
        }
    }


    // Drag Initialization
    public void OnBeginDrag(PointerEventData eventData)
    {
        doFollow = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        doFollow = true;
    }
}