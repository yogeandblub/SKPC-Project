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
    public void OnMouseDown()
    {
        // Only triggers on touch/click if you tap Santa
        santaCharController.SetTrigger("fist_fight");
        Debug.Log("Fight triggered!");
    }
}
