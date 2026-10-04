using UnityEngine;
using Unity.Netcode;

public class PlayerDetect : NetworkBehaviour
{
    [Header("Raycast Settings")]
    public Transform interactOrigin;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] float toDistance;
    
    [Header("Debug Info")]
    public static float distance;
    public float toTarget;
    public bool canInteract;
    public GameObject targetObject;

    private GameObject previousTarget;
    private MaterialPropertyBlock outlineBlock;
    private static readonly int ShowOutlineId = Shader.PropertyToID("_Show_Outline");

    void Start()
    {
        outlineBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (!IsOwner) return;

        Raycast();
        UpdateOutline();
    }

    void Raycast()
    {
        Ray ray = new Ray(interactOrigin.position, interactOrigin.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, toDistance, interactableLayer))
        {
            canInteract = true;
            toTarget = hit.distance;
            targetObject = hit.collider.gameObject;
        }
        else
        {
            canInteract = false;
            targetObject = null;
        }
    }

    void UpdateOutline()
    {
        if (targetObject == previousTarget) return;

        SetOutline(previousTarget, false);
        SetOutline(targetObject, true);
        previousTarget = targetObject;
    }

    // ฟังก์ชันนี้ให้ PlayerInteract เรียกใช้ตอนหยิบของ เพื่อล้างค่า target และปิด outline
    public void ClearTarget()
    {
        SetOutline(targetObject, false);
        previousTarget = null;
        targetObject = null;
        canInteract = false;
    }

    void SetOutline(GameObject target, bool show)
    {
        if (target == null) return;

        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer == null) return;

        int outlineSlot = renderer.sharedMaterials.Length - 1;
        renderer.GetPropertyBlock(outlineBlock, outlineSlot);
        outlineBlock.SetFloat(ShowOutlineId, show ? 1f : 0f);
        renderer.SetPropertyBlock(outlineBlock, outlineSlot);
    }
}