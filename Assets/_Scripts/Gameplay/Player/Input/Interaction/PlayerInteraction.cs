using UnityEngine;

public class PlayerInteraction : MonoBehaviour, IInteractor
{
    private IInteractable currentInteractable;

    public void TryInteract()
    {
        currentInteractable?.Interaction(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        currentInteractable = other.GetComponent<IInteractable>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<IInteractable>() == currentInteractable)
            currentInteractable = null;
    }
}