using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Vector3 targetOffset = new Vector3(0, 1f, 0);

    [Header("Distance")]
    public float defaultDistance = 6f;
    public float minDistance = 2f;
    public float maxDistance = 12f;
    public float zoomSpeed = 5f;

    [Header("Rotation")]
    public float rotationSpeed = 3f;
    public float minYAngle = 10f;
    public float maxYAngle = 80f;

    [Header("Smoothing")]
    public float smoothTime = 0.15f;

    private float currentDistance;
    private float currentX;
    private float currentY;
    private Vector3 velocity;

    void Start()
    {
        currentDistance = defaultDistance;
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Rotation
        if (Input.GetMouseButton(1))
        {
            currentX += Input.GetAxis("Mouse X") * rotationSpeed;
            currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;
            currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);
        }

        // Zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            currentDistance -= scroll * zoomSpeed;
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
        }

        // Calculate position
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 direction = rotation * Vector3.back;
        Vector3 targetPos = target.position + targetOffset + direction * currentDistance;

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
        transform.LookAt(target.position + targetOffset);
    }
}
