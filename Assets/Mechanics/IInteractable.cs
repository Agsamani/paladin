using UnityEngine;
public struct InteractionInfo 
{
    public GameObject Interactor;
}

public interface IInteractable {
    void Interact(InteractionInfo intrc);
    void OnRayHit(RaycastHit HitInfo);
    void OnRayExit();

    GameObject ReturnSelf();
}
