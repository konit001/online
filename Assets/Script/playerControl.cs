using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class playerControl : NetworkBehaviour
{
    private Rigidbody rb;
    private Transform camTransform;

    [Header("Move Setting")]
    public float Speed;
    public bool canMove = true;
    [Header("Rotate Setting")]
    public float rotateSpeed = 10f;
    [Header("Jump Setting")]
    public float jumpForce = 7f;
    public int maxJumps = 1;
    private int jumpsRemaining;
    [Header("Sprint Setting")]
    public float sprintMultiplier = 1.6f;
    [Header("Knockback Setting")]
    public float knockbackDuration = 0.35f;
    [Header("Cam")]
    public GameObject Comlock;

    private userInput input;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        input = GetComponent<userInput>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            jumpsRemaining = maxJumps;
            SetupCamera();
        }
    }

    private void SetupCamera()
    {
        var cam = GameObject.FindFirstObjectByType<CinemachineCamera>();
        if (cam != null)
        {
            cam.Target.TrackingTarget = Comlock.transform;
            cam.Target.LookAtTarget = Comlock.transform;
            camTransform = cam.transform;
        }
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;
        move();

        if (input._jumpInput && jumpsRemaining >= 0)
        {
            Jump();
        }
    }

    void move()
    {
        if (!IsOwner) return;
        if (camTransform == null) return;

        Vector2 MoveInput = input._moveInput;
        if (canMove)
        {
            // เอาทิศทางกล้อง แต่ตัดแกน Y ทิ้ง (ไม่ให้ตัวละครเอียงขึ้นลงตามกล้อง)
            Vector3 camForward = camTransform.forward;
            Vector3 camRight = camTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            // คำนวณทิศทางเดินโดยอิงจากกล้อง
            float currentSpeed = input._sprintInput ? Speed * sprintMultiplier : Speed;
            Vector3 moveDirection = (camForward * MoveInput.y + camRight * MoveInput.x);
            Vector3 movement = moveDirection * currentSpeed;
            rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);

            // หมุนตัวละครไปทางทิศที่กำลังเดิน (อ้างอิงจากกล้องแล้ว)
            if (moveDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime));
            }
        }
    }

    void Jump()
    {
        jumpsRemaining--;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            jumpsRemaining = maxJumps;
            input._jumpInput = false;
        }
    }

    // Server-side entry point: routes the impulse to the owning client, since movement is owner-authoritative.
    public void ApplyKnockback(Vector3 impulse)
    {
        ClientRpcParams rpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new[] { OwnerClientId } }
        };
        ApplyKnockbackClientRpc(impulse, rpcParams);
    }

    [ClientRpc]
    void ApplyKnockbackClientRpc(Vector3 impulse, ClientRpcParams rpcParams = default)
    {
        if (!IsOwner) return;

        canMove = false;
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(impulse, ForceMode.Impulse);

        StopCoroutine(nameof(ResumeMovementAfterKnockback));
        StartCoroutine(nameof(ResumeMovementAfterKnockback));
    }

    System.Collections.IEnumerator ResumeMovementAfterKnockback()
    {
        yield return new WaitForSeconds(knockbackDuration);
        canMove = true;
    }
}