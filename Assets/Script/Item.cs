using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Item Setting")]
    [SerializeField] private int point;

    public int Point => point;
}
