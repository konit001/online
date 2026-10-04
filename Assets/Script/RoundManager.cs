using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

public struct RoundResultEntry : INetworkSerializable
{
    public int Rank;
    public int PlayerNumber;
    public int Score;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Rank);
        serializer.SerializeValue(ref PlayerNumber);
        serializer.SerializeValue(ref Score);
    }
}

public class RoundManager : NetworkBehaviour
{
    public static RoundManager Instance { get; private set; }

    private readonly Dictionary<ulong, int> playerNumbers = new Dictionary<ulong, int>();
    private int nextPlayerNumber = 1;

    void Awake() => Instance = this;

    // อ่านผู้เล่นที่ต่ออยู่จริง ณ ขณะนี้จาก NetworkManager ตรงๆ แทนการเก็บ registry เอง
    // เพื่อไม่ให้ผลลัพธ์ขาดผู้เล่นคนไหนไปเพราะจังหวะ spawn/OnNetworkSpawn มาไม่ทัน
    private IEnumerable<PlayerScore> GetAllPlayerScores()
    {
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject == null) continue;

            PlayerScore playerScore = client.PlayerObject.GetComponent<PlayerScore>();
            if (playerScore != null) yield return playerScore;
        }
    }

    private int GetPlayerNumber(ulong clientId)
    {
        if (!playerNumbers.TryGetValue(clientId, out int number))
        {
            number = nextPlayerNumber++;
            playerNumbers[clientId] = number;
        }
        return number;
    }

    // เรียกตอนจบรอบ (จาก Timer.EndRound) เพื่อรวบรวม+จัดอันดับคะแนน แล้วส่งผลไปให้ทุก client แสดง
    public void TallyAndBroadcastResults()
    {
        if (!IsServer) return;

        RoundResultEntry[] results = GetAllPlayerScores()
            .OrderByDescending(p => p.Score)
            .Select((p, index) => new RoundResultEntry
            {
                Rank = index + 1,
                PlayerNumber = GetPlayerNumber(p.OwnerClientId),
                Score = p.Score
            })
            .ToArray();

        ShowRoundResultsClientRpc(results);
    }

    // เรียกตอนเริ่มรอบใหม่ (จาก Timer.StartNewRound) เพื่อรีเซ็ตคะแนนทุกคนและซ่อนผลของรอบก่อนหน้า
    public void ResetAllScores()
    {
        if (!IsServer) return;

        foreach (PlayerScore player in GetAllPlayerScores())
            player.ResetScore();

        HideRoundResultsClientRpc();
    }

    [ClientRpc]
    void ShowRoundResultsClientRpc(RoundResultEntry[] results)
    {
        if (RoundResultsUI.Instance != null)
            RoundResultsUI.Instance.ShowResults(results);
    }

    [ClientRpc]
    void HideRoundResultsClientRpc()
    {
        if (RoundResultsUI.Instance != null)
            RoundResultsUI.Instance.HideResults();
    }
}
