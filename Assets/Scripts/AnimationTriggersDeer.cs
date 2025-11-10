using UnityEngine;

public class OrcAnimationController : MonoBehaviour
{
    // Arrastra aquí el Animator del Orc (el que tiene DeerController asignado)
    public Animator orcAnimator;

    // Guarda el último estado reproducido (por defecto "Idle")
    private string currentState = "Idle";

    // --- MÉTODOS PARA ANIMACIONES ---

    public void PlayIdle()
    {
        ChangeState("Idle");          // Trigger "Idle"
    }

    public void PlayJazz()
    {
        ChangeState("Jazz");          // Trigger "Jazz"
    }

    public void PlayBreakdance()
    {
        ChangeState("Breakdance");    // Trigger "Breakdance"
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

    // --- DETECCIÓN DE TOQUES / CLICS EN EL PERSONAJE (OPCIONAL) ---
    void Update()
    {
        // Clic con mouse (para PC)
        if (Input.GetMouseButtonDown(0))
        {
            DetectTap();
        }

        // Toque en pantalla (para móvil)
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            DetectTap();
        }
    }

    private void DetectTap()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Solo reacciona si se hace clic en el modelo del orco
            if (hit.transform == transform)
            {
                // Ejemplo: alterna entre Jazz y Breakdance al tocar el orco
                if (currentState == "Jazz")
                {
                    PlayBreakdance();
                }
                else
                {
                    PlayJazz();
                }
            }
        }
    }
}
