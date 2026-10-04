using UnityEngine;

public class GameSession : MonoBehaviour
{
    public enum GameMode { FreeForAll, Team }

    public static GameSession Instance { get; private set; }

    [SerializeField] private GameMode mode = GameMode.FreeForAll;
    public GameMode CurrentMode => mode;

    void Awake() => Instance = this;
}
