using UnityEngine;

public class AnimationTriggers : MonoBehaviour
{
    public Animator santaCharController;
    private bool isTwerking = false; // keeps track of current state


    public void ToggleDance()
    {
        if (isTwerking)
        {
            //go back to idle
            santaCharController.ResetTrigger("Twerk");
            santaCharController.SetTrigger("Idle");
            Debug.Log("Back to Idle!");
        }
        else
        {
            //start twerking
            santaCharController.ResetTrigger("Idle");
            santaCharController.SetTrigger("Twerk");
            Debug.Log("Twerk is triggered!");
        }
        // Flip the state
        isTwerking = !isTwerking;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DetectTap();
        }

        // Also handle mobile touch
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            DetectTap();
        }
    }

    void DetectTap()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform == transform)
            {
                santaCharController.SetTrigger("Tap");
            }
        }
    }
}
