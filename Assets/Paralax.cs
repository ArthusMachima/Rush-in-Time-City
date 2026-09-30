using UnityEngine;

public class SimpleParallax : MonoBehaviour
{
    [Header("Parallax Settings")]
    [Tooltip("Lower values (e.g., 0.1) move slower/further away. Higher values (e.g., 0.9) move faster/closer.")]
    [Range(0f, 1f)]
    public float parallaxSpeed = 0.5f;

    private Transform _cameraTransform;
    private Vector3 _lastCameraPosition;

    void Start()
    {
        _cameraTransform = Camera.main.transform;
        _lastCameraPosition = _cameraTransform.position;
    }

    // LateUpdate is preferred for camera-following scripts
    void LateUpdate()
    {
        // Calculate how much the camera moved this frame
        Vector3 deltaMovement = _cameraTransform.position - _lastCameraPosition;

        // Move this object by a fraction of that movement
        transform.position += deltaMovement * parallaxSpeed;

        // Update the last position for the next frame
        _lastCameraPosition = _cameraTransform.position;
    }
}
