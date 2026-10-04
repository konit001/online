using Unity.Netcode;

public class PlayerScore : NetworkBehaviour
{
    private NetworkVariable<int> score = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public int Score => score.Value;

    public void AddScore(int amount)
    {
        if (!IsServer) return;
        score.Value += amount;
    }

    public void ResetScore()
    {
        if (!IsServer) return;
        score.Value = 0;
    }
}
