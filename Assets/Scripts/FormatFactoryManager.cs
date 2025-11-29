using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
public class FormatFactoryManager : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public TextMeshProUGUI promptText;
    public GridBuilder grid;
    public SelectionMovement selectionMovement;
    public float delaySeconds = 15f;
    public bool autoStart = true;

    class C { public string Raw; public string Type; }
    readonly List<C> challenges = new List<C>();
    float t;
    bool running;

    void Start()
    {
        if (grid == null) grid = FindAnyObjectByType<GridBuilder>();
        if (autoStart) { BuildAndPopulate(); running = true; t = 0f; UpdatePrompt(); }
    }

    void Update()
    {
        if (!running) return;

        t += Time.deltaTime;
        UpdatePrompt();

        if (t >= delaySeconds)
        {
            t = delaySeconds;
            UpdatePrompt();

            bool success = Evaluate();
            ClearImage(success);
            running = false;
        }
    }

    void UpdatePrompt()
    {
        if (promptText == null) return;
        int total = Mathf.Max(1, Mathf.RoundToInt(delaySeconds));
        int elapsedDisplay = Mathf.Clamp(Mathf.FloorToInt(t) + 1, 1, total);
        promptText.text = $"{elapsedDisplay:00} / {total:00}";
    }

    CellData Cell(int r, int c) => grid.cell_datas[r, c];

    public void BuildAndPopulate()
    {
        if (grid == null) grid = FindAnyObjectByType<GridBuilder>();
        if (grid == null || grid.cols < 2) return;

        challenges.Clear();
        var pool = new List<C>
        {
            new C{Raw="4/21/24",Type="Date"}, new C{Raw="12/31/99",Type="Date"}, new C{Raw="1/1/2022",Type="Date"}, new C{Raw="7/4",Type="Date"}, new C{Raw="4/21/2025",Type="Date"},
            new C{Raw="13.5",Type="Time"}, new C{Raw="0.25",Type="Time"}, new C{Raw="1:05:03",Type="Time"}, new C{Raw="00:15:00",Type="Time"}, new C{Raw="8:05",Type="Time"},
            new C{Raw="100",Type="Currency"}, new C{Raw="9.5",Type="Currency"}, new C{Raw="0",Type="Currency"}, new C{Raw="1234.56",Type="Currency"}, new C{Raw="5.25",Type="Currency"},
            new C{Raw="0.5",Type="Percentage"}, new C{Raw="1",Type="Percentage"}, new C{Raw="0.1234",Type="Percentage"}, new C{Raw="0",Type="Percentage"}, new C{Raw="0.9876",Type="Percentage"},
            new C{Raw="12",Type="Decimal"}, new C{Raw="13",Type="Decimal"}, new C{Raw="9.9",Type="Decimal"}, new C{Raw="0",Type="Decimal"}, new C{Raw="3.1415",Type="Decimal"}
        };

        var rng = new System.Random();
        int max = 14;
        for (int i = 0; i < max; i++)
        {
            int idx = rng.Next(pool.Count);
            challenges.Add(pool[idx]);
            pool.RemoveAt(idx);
        }

        for (int r = 0; r < grid.rows; r++)
        {
            Cell(r, 0).update_text("");
            if (grid.cols > 1) Cell(r, 1).update_text("");
        }

        for (int r = 0; r < challenges.Count; r++)
        {
            Cell(r, 0).update_text(challenges[r].Raw);
            Cell(r, 1).update_text(challenges[r].Type);
        }

        t = 0f;
        running = true;
        UpdatePrompt();
    }

    public bool Evaluate()
    {
        if (grid == null) grid = FindAnyObjectByType<GridBuilder>();
        if (grid == null || selectionMovement == null) return false;

        var saved = selectionMovement.selectedCells == null ? new List<CellData>() : new List<CellData>(selectionMovement.selectedCells);

        int correct = 0;
        for (int r = 0; r < challenges.Count; r++)
        {
            var cell = Cell(r, 0);
            string player = cell.get_text() ?? "";
            string old = player;

            cell.update_text(challenges[r].Raw);
            selectionMovement.selectedCells = new List<CellData> { cell };

            var type = challenges[r].Type;
            if (type == "Decimal") selectionMovement.FormatAsDecimal();
            else if (type == "Time") selectionMovement.FormatAsTime();
            else if (type == "Date") selectionMovement.FormatAsDate();
            else if (type == "Currency") selectionMovement.FormatAsCurrency();
            else if (type == "Percentage") selectionMovement.FormatAsPercentage();

            string expected = cell.get_text() ?? "";
            cell.update_text(old);

            if (Equalish(player, expected)) correct++;
        }

        selectionMovement.selectedCells = saved;

        Debug.Log($"{correct}/{challenges.Count}");
        return correct == challenges.Count;
    }

    static bool Equalish(string a, string b)
    {
        a = (a ?? "").Trim();
        b = (b ?? "").Trim();
        if (string.Equals(a, b, StringComparison.OrdinalIgnoreCase)) return true;
        return Norm(a) == Norm(b);
    }

    void ClearImage(bool success)
    {
        if (success)
        {
            Sprite loaded = Resources.Load<Sprite>("checkmark");
            spriteRenderer.sprite = loaded;
        }
        else
        {
            Sprite loaded = Resources.Load<Sprite>("fail");
            spriteRenderer.sprite = loaded;
        }
    }

    static string Norm(string s) => (s ?? "").Replace(" ", "").Replace(",", "").Replace("$", "").ToLowerInvariant();
}