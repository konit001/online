using UnityEngine;
using TMPro;

public class FloatingPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float floatSpeed = 1.5f;
    [SerializeField] private float lifetime = 1.2f;

    private float timer;
    private Color startColor;
    private Camera targetCamera;

    void Awake()
    {
        if (text != null) startColor = text.color;
        targetCamera = Camera.main;
    }

    public void Init(string message)
    {
        if (text != null) text.text = message;
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        timer += Time.deltaTime;
        if (text != null)
        {
            float alpha = Mathf.Clamp01(1f - timer / lifetime);
            text.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
        }

        if (timer >= lifetime) Destroy(gameObject);
    }

    void LateUpdate()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        if (targetCamera == null) return;

        transform.rotation = Quaternion.LookRotation(transform.position - targetCamera.transform.position);
    }
}
