using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Targets (characters)")]
    public Transform santaTarget;
    public Transform orcTarget;

    [Header("Camera settings")]
    public float distance = 5f;
    public float height = 2f;
    public float smoothSpeed = 5f;

    private Transform currentTarget;
    private Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void Start()
    {
        // Start focusing Santa by default
        currentTarget = santaTarget;
    }

    void LateUpdate()
    {
        if (currentTarget == null || cam == null) return;

        Vector3 desiredPosition =
            currentTarget.position
            - currentTarget.forward * distance
            + Vector3.up * height;

        cam.transform.position = Vector3.Lerp(
            cam.transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        Quaternion desiredRotation =
            Quaternion.LookRotation(currentTarget.position - cam.transform.position, Vector3.up);

        cam.transform.rotation = Quaternion.Slerp(
            cam.transform.rotation,
            desiredRotation,
            smoothSpeed * Time.deltaTime
        );
    }

    // 🎯 Button 1: Focus Santa
    public void FocusSanta()
    {
        currentTarget = santaTarget;
        Debug.Log("Camera now focusing on Santa!");
    }

    // 🎯 Button 2: Focus Orc
    public void FocusOrc()
    {
        currentTarget = orcTarget;
        Debug.Log("Camera now focusing on Orc!");
    }
}
