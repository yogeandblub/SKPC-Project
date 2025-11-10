using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Targets (characters)")]
    public Transform santaTarget;
    public Transform orcTarget;

    [Header("Camera settings")]
    public float distance = 5f;          // distance from character
    public float height = 2f;            // height above character
    public float positionSmooth = 5f;    // move smoothing
    public float rotationSmooth = 5f;    // rotation smoothing
    public float orbitSpeed = 30f;       // degrees per second

    private Transform currentTarget;
    private Camera cam;

    private bool isOrbiting = false;
    private float orbitAngle = 0f;
    private Vector3 desiredPosition;

    void Awake()
    {
        // Use the camera this script is on, or Camera.main
        cam = GetComponent<Camera>();
        if (cam == null)
            cam = Camera.main;
    }

    void LateUpdate()
    {
        if (currentTarget == null || cam == null) return;

        if (isOrbiting)
        {
            // orbit angle over time
            orbitAngle += orbitSpeed * Time.deltaTime;

            float rad = orbitAngle * Mathf.Deg2Rad;

            // circle around the character on XZ plane
            Vector3 orbitOffset = new Vector3(
                Mathf.Sin(rad),
                0f,
                Mathf.Cos(rad)
            ) * distance;

            desiredPosition = currentTarget.position
                              + orbitOffset
                              + Vector3.up * height;
        }

        // Smooth movement
        cam.transform.position = Vector3.Lerp(
            cam.transform.position,
            desiredPosition,
            positionSmooth * Time.deltaTime
        );

        // Always look at the target
        Quaternion lookRot = Quaternion.LookRotation(
            currentTarget.position - cam.transform.position,
            Vector3.up
        );

        cam.transform.rotation = Quaternion.Slerp(
            cam.transform.rotation,
            lookRot,
            rotationSmooth * Time.deltaTime
        );
    }

    // ----------------------------
    // BUTTON METHODS
    // ----------------------------

    // Call this from the SANTA button (in OnClick)
    public void OnSantaButton()
    {
        HandleTargetButton(santaTarget);
    }

    // Call this from the ORC button (in OnClick)
    public void OnOrcButton()
    {
        HandleTargetButton(orcTarget);
    }

    // Core logic: switch target / toggle orbit / show front
    private void HandleTargetButton(Transform newTarget)
    {
        if (newTarget == null) return;

        // If we are switching to a different character
        if (currentTarget != newTarget)
        {
            currentTarget = newTarget;
            AlignBehindCurrentTarget();
            isOrbiting = true;  // start orbit automatically
            Debug.Log("Camera: new target, start orbit");
        }
        else
        {
            // Same character again → toggle orbit / front
            if (isOrbiting)
            {
                // stop orbit and move to front
                isOrbiting = false;
                MoveToFrontOfCurrentTarget();
                Debug.Log("Camera: stop orbit, show front");
            }
            else
            {
                // go back behind and start orbit again
                AlignBehindCurrentTarget();
                isOrbiting = true;
                Debug.Log("Camera: back behind, orbit again");
            }
        }
    }

    // ----------------------------
    // HELPER METHODS
    // ----------------------------

    // Put camera behind current target
    private void AlignBehindCurrentTarget()
    {
        if (currentTarget == null) return;

        Vector3 dir = -currentTarget.forward;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f)
            dir = Vector3.back;

        dir.Normalize();

        orbitAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        desiredPosition = currentTarget.position
                          + dir * distance
                          + Vector3.up * height;

        // snap once so Lerp has good start
        cam.transform.position = desiredPosition;
        cam.transform.LookAt(currentTarget.position, Vector3.up);
    }

    // Put camera in front of current target
    private void MoveToFrontOfCurrentTarget()
    {
        if (currentTarget == null) return;

        Vector3 dir = currentTarget.forward;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f)
            dir = Vector3.forward;

        dir.Normalize();

        orbitAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        desiredPosition = currentTarget.position
                          + dir * distance
                          + Vector3.up * height;
    }
}
