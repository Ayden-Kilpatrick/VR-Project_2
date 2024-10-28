using UnityEngine;

public class Teleportation : MonoBehaviour
{
    public Transform player; // The player object (headset position)
    public OVRInput.Button teleportButton = OVRInput.Button.PrimaryIndexTrigger; // Teleport button

    public LayerMask teleportLayer; // Layer mask for valid teleport zones
    public float maxTeleportDistance = 10.0f; // Max teleport distance
    private LineRenderer lineRenderer; // Visual cue for the teleport beam
    private bool isAiming = false;

    void Start()
    {
        // Initialize the line renderer for teleport aim visuals
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }

    void Update()
    {
        HandleTeleportation();
    }

    void HandleTeleportation()
    {
        // Check if teleport button is pressed
        if (OVRInput.GetDown(teleportButton))
        {
            isAiming = true;
            lineRenderer.enabled = true;
        }
        else if (OVRInput.GetUp(teleportButton) && isAiming)
        {
            // Perform the teleport when the button is released
            PerformTeleport();
            isAiming = false;
            lineRenderer.enabled = false;
        }

        // Visualize the teleport aim line
        if (isAiming)
        {
            VisualizeTeleport();
        }
    }

    void VisualizeTeleport()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Cast a ray to determine the teleport destination
        if (Physics.Raycast(ray, out hit, maxTeleportDistance, teleportLayer))
        {
            lineRenderer.SetPosition(0, transform.position); // Start of the teleport line (controller position)
            lineRenderer.SetPosition(1, hit.point); // End of the teleport line (where the ray hits)

            lineRenderer.material.color = Color.green; // Valid teleport location
        }
        else
        {
            // If no valid location is hit, extend the line
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + transform.forward * maxTeleportDistance);

            lineRenderer.material.color = Color.red; // Invalid location
        }
    }

    void PerformTeleport()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Perform the raycast again to find the valid teleport location
        if (Physics.Raycast(ray, out hit, maxTeleportDistance, teleportLayer))
        {
            Vector3 teleportDestination = hit.point;

            // Check if the hit point is within a valid teleport area (teleportLayer)
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("TeleportArea"))
            {
                // Move the player to the teleport destination
                player.position = teleportDestination;
            }
        }
    }
}
