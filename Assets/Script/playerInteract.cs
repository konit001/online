using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class playerInteract : NetworkBehaviour
{
    [Header("pickUp Setting")]
    public Transform pickUpPoint;

    [Header("Throw Setting")]
    public float minThrowForce = 5f;
    public float maxThrowForce = 20f;
    public float chargeTime = 1.2f;
    public float throwUpwardArc = 0.3f;

    [Header("Delivery Popup Setting")]
    [SerializeField] GameObject deliveryPointPopupPrefab;

    private userInput input;
    private PlayerDetect scanner; // อ้างอิงไปยังสคริปต์แกน
    private NetworkObject heldItem;
    private ItemDropZone currentDropZone;
    
    private bool isCharging;
    private float chargeTimer;

    void Start()
    {
        input = GetComponent<userInput>();
        scanner = GetComponent<PlayerDetect>();
    }

    void Update()
    {
        if (!IsOwner) return;

        TryInteract();
        HandleThrow();
    }

    void TryInteract()
    {
        if (!input._interactInput) return;

        if (heldItem != null)
        {
            if (currentDropZone != null)
                DeliverItemServerRpc(heldItem.NetworkObjectId);
            else
                DropItemServerRpc(heldItem.NetworkObjectId);

            heldItem = null;
            return;
        }

        // ดึงข้อมูล target จาก PlayerScanner
        if (!scanner.canInteract || scanner.targetObject == null) return;

        if (scanner.targetObject.CompareTag("Item"))
        {
            NetworkObject itemNetworkObject = scanner.targetObject.GetComponent<NetworkObject>();
            if (itemNetworkObject == null) return;

            // สั่งล้างเป้าหมายใน Scanner เพื่อปิด Outline
            scanner.ClearTarget();

            heldItem = itemNetworkObject;
            PickUpItemServerRpc(itemNetworkObject.NetworkObjectId);
        }
    }

    void HandleThrow()
    {
        if (heldItem == null)
        {
            isCharging = false;
            chargeTimer = 0f;
            return;
        }

        if (input._attackInput)
        {
            isCharging = true;
            chargeTimer = Mathf.Min(chargeTimer + Time.deltaTime, chargeTime);
        }

        if (input._attackReleased && isCharging)
        {
            float chargeRatio = chargeTimer / chargeTime;
            float force = Mathf.Lerp(minThrowForce, maxThrowForce, chargeRatio);
            
            // ใช้ทิศทางจาก interactOrigin ของ Scanner
            Vector3 throwDirection = (scanner.interactOrigin.forward + Vector3.up * throwUpwardArc).normalized;

            ThrowItemServerRpc(heldItem.NetworkObjectId, throwDirection, force);
            heldItem = null;
            isCharging = false;
            chargeTimer = 0f;
        }
    }

    public void EnterDropZone(ItemDropZone zone) => currentDropZone = zone;

    public void ExitDropZone(ItemDropZone zone)
    {
        if (currentDropZone == zone) currentDropZone = null;
    }

    [ServerRpc]
    void DeliverItemServerRpc(ulong itemNetworkObjectId)
    {
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(itemNetworkObjectId, out NetworkObject itemNetworkObject)) return;

        Item item = itemNetworkObject.GetComponent<Item>();
        if (item != null)
        {
            PlayerScore playerScore = GetComponent<PlayerScore>();
            if (playerScore != null) playerScore.AddScore(item.Point);

            ShowDeliveryPopupClientRpc(itemNetworkObject.transform.position, item.Point);
        }

        itemNetworkObject.Despawn(true);
    }

    [ClientRpc]
    void ShowDeliveryPopupClientRpc(Vector3 position, int point)
    {
        if (deliveryPointPopupPrefab == null) return;

        GameObject popup = Instantiate(deliveryPointPopupPrefab, position, Quaternion.identity);
        popup.GetComponent<FloatingPopup>().Init($"+{point}");
    }

    [ServerRpc]
    void PickUpItemServerRpc(ulong itemNetworkObjectId)
    {
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(itemNetworkObjectId, out NetworkObject itemNetworkObject)) return;

        itemNetworkObject.ChangeOwnership(OwnerClientId);
        itemNetworkObject.TrySetParent(NetworkObject, false);

        SetHeldStateClientRpc(itemNetworkObjectId, true);

        SnapHeldItemClientRpc(itemNetworkObjectId, new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new[] { OwnerClientId } }
        });
    }

    [ClientRpc]
    void SetHeldStateClientRpc(ulong itemNetworkObjectId, bool isHeld)
    {
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(itemNetworkObjectId, out NetworkObject itemNetworkObject)) return;

        NetworkRigidbody itemNetworkRigidbody = itemNetworkObject.GetComponent<NetworkRigidbody>();
        if (itemNetworkRigidbody != null)
        {
            itemNetworkRigidbody.SetIsKinematic(isHeld);
        }
        else
        {
            Rigidbody itemRigidbody = itemNetworkObject.GetComponent<Rigidbody>();
            if (itemRigidbody != null) itemRigidbody.isKinematic = isHeld;
        }

        Collider itemCollider = itemNetworkObject.GetComponent<Collider>();
        if (itemCollider != null) itemCollider.enabled = !isHeld;
    }

    [ClientRpc]
    void SnapHeldItemClientRpc(ulong itemNetworkObjectId, ClientRpcParams clientRpcParams = default)
    {
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(itemNetworkObjectId, out NetworkObject itemNetworkObject)) return;

        Transform itemTransform = itemNetworkObject.transform;
        itemTransform.position = pickUpPoint.position;
        itemTransform.rotation = pickUpPoint.rotation;
    }

    [ServerRpc]
    void DropItemServerRpc(ulong itemNetworkObjectId)
    {
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(itemNetworkObjectId, out NetworkObject itemNetworkObject)) return;

        DetachHeldItem(itemNetworkObject);
        itemNetworkObject.RemoveOwnership();
    }

    [ServerRpc]
    void ThrowItemServerRpc(ulong itemNetworkObjectId, Vector3 direction, float force)
    {
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(itemNetworkObjectId, out NetworkObject itemNetworkObject)) return;

        Rigidbody itemRigidbody = DetachHeldItem(itemNetworkObject);

        ThrowableItem throwable = itemNetworkObject.GetComponent<ThrowableItem>();
        if (throwable != null) throwable.SetThrower(OwnerClientId);

        Vector3 throwDirection = direction.sqrMagnitude > 0.0001f ? direction.normalized : transform.forward;
        float clampedForce = Mathf.Clamp(force, minThrowForce, maxThrowForce);
        if (itemRigidbody != null) itemRigidbody.linearVelocity = throwDirection * clampedForce;

        itemNetworkObject.RemoveOwnership();
    }

    Rigidbody DetachHeldItem(NetworkObject itemNetworkObject)
    {
        itemNetworkObject.TrySetParent((Transform)null, true);
        SetHeldStateClientRpc(itemNetworkObject.NetworkObjectId, false);
        return itemNetworkObject.GetComponent<Rigidbody>();
    }
}