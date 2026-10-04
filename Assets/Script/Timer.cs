using UnityEngine;
using TMPro;
using Unity.Netcode;

public class Timer : NetworkBehaviour
{
    public GameObject _Timer;
    public bool isPlay = true;

    [Header("Time")]
    public TextMeshProUGUI time;
    public bool FreezeTime;

    [Header("Round")]
    public float RoundDurationSeconds = 300f; // ความยาวของแต่ละรอบ (วินาที) ปรับได้ใน Inspector

    private float secondCount;

    public int MinuteCount { get; private set; } = 0;
    public int SecondCount => Mathf.FloorToInt(secondCount);
    public bool RoundEnded { get; private set; }

    // เวลาที่เหลือของรอบปัจจุบัน (server เป็นคนคุม)
    private NetworkVariable<float> networkTimeRemaining = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private NetworkVariable<bool> networkRoundEnded = new NetworkVariable<bool>(
        true,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            StartNewRound();
    }
    void FixedUpdate()
    {
        if (IsServer)
            Tick();

        UpdateLocalFromNetwork();
        RefreshTimeUI();
    }

    void Tick()
    {
        if (FreezeTime || !isPlay || networkRoundEnded.Value) return;

        networkTimeRemaining.Value -= Time.fixedDeltaTime;
        if (networkTimeRemaining.Value <= 0f)
        {
            EndRound();
        }
    }

    void UpdateLocalFromNetwork()
    {
        float total = Mathf.Max(networkTimeRemaining.Value, 0f);
        MinuteCount = Mathf.FloorToInt(total / 60f);
        secondCount = total % 60f;
        RoundEnded = networkRoundEnded.Value;

        if (_Timer != null)
            _Timer.SetActive(!networkRoundEnded.Value);
    }

    void RefreshTimeUI()
    {
        if (time != null)
            time.text = MinuteCount.ToString("00") + ":" + SecondCount.ToString("00");
    }

    // เรียกตอนต้องการเริ่มรอบใหม่ (เช่นจากปุ่ม Ready หรือ GameManager)
    public void StartNewRound()
    {
        if (!IsServer) return;
        networkTimeRemaining.Value = RoundDurationSeconds;
        networkRoundEnded.Value = false;
        RoundManager.Instance?.ResetAllScores();
    }

    public void EndRound()
    {
        if (!IsServer) return;
        networkTimeRemaining.Value = 0f;
        networkRoundEnded.Value = true; // จบรอบ แล้วหยุดรอ ไม่นับต่อเอง
        RoundManager.Instance?.TallyAndBroadcastResults();
    }
}