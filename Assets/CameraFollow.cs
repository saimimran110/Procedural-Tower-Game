using UnityEngine;

public class CameraFollowTower : MonoBehaviour
{
    public Transform stackTracker;
    public float followSmoothness = 3f;
    public float verticalOffset = 6.5f;
    public float minCameraY = 10.28f; // Ground (3.78) + offset (6.5)
    
    private Vector3 initialPosition;
    private float targetYPosition;

    private void Start()
    {
        initialPosition = new Vector3(0, minCameraY, -10); // X=0, Y=10.28, Z=-10
        transform.position = initialPosition;
        transform.rotation = Quaternion.Euler(30, 0, 0); // Angled perspective view
        Camera.main.fieldOfView = 60; // Set FOV
        
        targetYPosition = minCameraY;
    }

    void LateUpdate()
    {
        if (stackTracker == null) return;

        targetYPosition = Mathf.Max(stackTracker.position.y + verticalOffset, minCameraY);
        
        Vector3 desiredPosition = new Vector3(
            initialPosition.x, 
            targetYPosition, 
            initialPosition.z
        );

        transform.position = Vector3.Lerp(
            transform.position, 
            desiredPosition, 
            followSmoothness * Time.deltaTime
        );
    }
}