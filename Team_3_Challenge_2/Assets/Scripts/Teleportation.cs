using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.OpenXR.Input;

public class OpenXRTeleport : MonoBehaviour
{
    public Transform player; // Player or XR Rig
    public LayerMask teleportLayer; // Layer for valid teleport areas
    public float maxTeleportDistance = 10.0f; // Max distance for teleportation

    private InputAction teleportAction;
    private LineRenderer lineRenderer;
    private bool isAiming = false;

    void Awake()
    {
        // Initialize the line renderer for visual feedback
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;

        // Initialize the teleport action
        teleportAction = new InputAction("Teleport", binding: "<XRController>{RightHand}/trigger");
        teleportAction.Enable();
    }

    void Update()
    {
        // Check if the teleport button is pressed
        if (teleportAction.triggered)
        {
            isAiming = !isAiming; // Toggle aiming
            lineRenderer.enabled = isAiming;

            if (!isAiming) PerformTeleport(); // Teleport on release
        }

        // Show teleport line when aiming
        if (isAiming)
        {
            VisualizeTeleport();
        }
    }

    void VisualizeTeleport()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxTeleportDistance, teleportLayer))
        {
            lineRenderer.SetPosition(0, transform.position); // Controller position
            lineRenderer.SetPosition(1, hit.point); // Teleport point

            lineRenderer.material.color = Color.green; // Valid target indicator
        }
        else
        {
            // Draw the max distance if no valid point is hit
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + transform.forward * maxTeleportDistance);

            lineRenderer.material.color = Color.red; // Invalid target indicator
        }
    }

    void PerformTeleport()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxTeleportDistance, teleportLayer))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("TeleportArea"))
            {
                // Move player to teleport destination
                player.position = hit.point;
            }
        }
    }
}
