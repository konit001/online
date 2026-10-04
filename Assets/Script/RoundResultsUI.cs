using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundResultsUI : MonoBehaviour
{
    public static RoundResultsUI Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private Transform rowContainer;
    [SerializeField] private GameObject rowPrefab; // ต้องมี TextMeshProUGUI อยู่ในตัวมันเองหรือลูก

    private readonly List<GameObject> spawnedRows = new List<GameObject>();

    void Awake()
    {
        Instance = this;
        if (panel != null) panel.SetActive(false);
    }

    public void ShowResults(RoundResultEntry[] results)
    {
        ClearRows();

        foreach (RoundResultEntry entry in results)
        {
            if (rowPrefab == null || rowContainer == null) break;

            GameObject row = Instantiate(rowPrefab, rowContainer);
            TextMeshProUGUI text = row.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.text = $"#{entry.Rank}  Player {entry.PlayerNumber}  -  {entry.Score} pts";

            spawnedRows.Add(row);
        }

        if (panel != null) panel.SetActive(true);
    }

    public void HideResults()
    {
        if (panel != null) panel.SetActive(false);
        ClearRows();
    }

    void ClearRows()
    {
        foreach (GameObject row in spawnedRows)
            Destroy(row);
        spawnedRows.Clear();
    }
}
