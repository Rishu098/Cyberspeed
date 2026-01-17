using UnityEngine;
using System;
using System.Collections.Generic;

public class BoardView : MonoBehaviour
{
    public GameObject cardPrefab;
    public RectTransform area;

    List<CardView> views = new();

    public event Action<int> CardClicked;

    public void Build(int rows, int cols)
    {
        Clear();

        // get real panel size
        float width = area.rect.width;
        float height = area.rect.height;

        float cellW = width / cols;
        float cellH = height / rows;

        float size = Mathf.Min(cellW, cellH);

        // --- CENTER GRID ---
        float gridW = size * cols;
        float gridH = size * rows;

        float offsetX = (width - gridW) / 2f;
        float offsetY = (height - gridH) / 2f;

        for (int i = 0; i < rows * cols; i++)
        {
            var go = Instantiate(cardPrefab, area);
            RectTransform rt = go.GetComponent<RectTransform>();

            int x = i % cols;
            int y = i / cols;

            // FORCE LOCAL UI MODE
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0.5f, 0.5f);

            rt.sizeDelta = new Vector2(size, size);

            rt.anchoredPosition = new Vector2(
                offsetX + (x + 0.5f) * size,
               -(offsetY + (y + 0.5f) * size)
            );

            var v = go.GetComponent<CardView>();
            v.Index = i;
            v.Clicked += id => CardClicked?.Invoke(id);

            views.Add(v);
        }
    }



    public void ShowFlip(int index, bool show)
        => views[index].Flip(show);

    public void SetMatched(CardModel a, CardModel b)
    {
        views[a.Id].SetMatched();
        views[b.Id].SetMatched();
    }

    void Clear()
    {
        foreach (Transform t in area)
            Destroy(t.gameObject);

        views.Clear();
    }
}
