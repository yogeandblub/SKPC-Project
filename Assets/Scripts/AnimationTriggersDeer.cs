using UnityEngine;

public class OrcAnimationController : MonoBehaviour
{
    // Arrastra aquí el Animator del Orc (el que tiene DeerController asignado)
    public Animator orcAnimator;

    // Guarda el último estado reproducido (por defecto "Idle")
    private string currentState = "Idle";

    // ¿Estamos bailando o en Idle?
    private bool isDancing = false;

    // --- MÉTODO PARA EL BOTÓN ---

    // Llama a esto desde el botón del Orc
    public void ToggleDance()
    {
        if (isDancing)
        {
            // Volver a Idle
            ChangeState("Idle");
        }
        else
        {
            // Empezar a bailar (puedes usar "Jazz" o "Breakdance")
            ChangeState("Jazz");
        }

        isDancing = !isDancing;
    }

    // --- MÉTODOS OPCIONALES POR SI QUIERES LLAMARLOS DIRECTO ---

    public void PlayIdle()
    {
        ChangeState("Idle");
    }

    public void PlayJazz()
    {
        ChangeState("Jazz");
    }

    public void PlayBreakdance()
    {
        ChangeState("Breakdance");
    }

    // --- CAMBIO DE ESTADO GENERAL ---
    private void ChangeState(string newTrigger)
    {
        // Evita repetir la misma animación
        if (currentState == newTrigger) return;

        // Limpia el trigger anterior y activa el nuevo
        orcAnimator.ResetTrigger(currentState);
        orcAnimator.SetTrigger(newTrigger);

        currentState = newTrigger;
        Debug.Log("Orc animation changed to: " + newTrigger);
    }
}
