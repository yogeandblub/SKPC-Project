using UnityEngine;

public class AnimationTriggers : MonoBehaviour
{
    public Animator santaCharController;

    public void TwerkDance()
    {
        santaCharController.SetTrigger("Twerk");
        santaCharController.ResetTrigger("Idle");
    }
}
