using UnityEngine;

public class ItemDropZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        playerInteract player = other.GetComponentInParent<playerInteract>();
        if (player == null || !player.IsOwner) return;
        player.EnterDropZone(this);
    }

    void OnTriggerExit(Collider other)
    {
        playerInteract player = other.GetComponentInParent<playerInteract>();
        if (player == null || !player.IsOwner) return;
        player.ExitDropZone(this);
    }
}
