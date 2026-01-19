using UnityEngine;
using System;
using System.Collections.Generic;

public class BoardView : MonoBehaviour
{
    public GameObject cardPrefab;
    public RectTransform area;

    public Sprite[] availableImages;

    List<CardView> views = new();

    public event Action<int> CardClicked;

    public int Count => views.Count;

    public void Build(int rows, int cols)
    {
        Clear();

        int total = rows * cols;
        int pairs = total / 2;

        // ---- CREATE RANDOM ID LIST ----
        List<int> ids = new List<int>();

        for (int i = 0; i < pairs; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }

        // shuffle
        for (int i = 0; i < ids.Count; i++)
        {
            int r = UnityEngine.Random.Range(i, ids.Count);
            (ids[i], ids[r]) = (ids[r], ids[i]);
        }

        // ===== POSITION CALC =====
        float width = area.rect.width;
        float height = area.rect.height;

        float cellW = width / cols;
        float cellH = height / rows;

        float size = Mathf.Min(cellW, cellH);

        float gridW = size * cols;
        float gridH = size * rows;

        float offsetX = (width - gridW) / 2f;
        float offsetY = (height - gridH) / 2f;
        // =========================

        // ---- CREATE CARDS ----
        for (int i = 0; i < total; i++)
        {
            var go = Instantiate(cardPrefab, area);

            var view = go.GetComponent<CardView>();

            view.Index = i;
            int imageIndex = ids[i] % availableImages.Length;

            view.SetImage(availableImages[imageIndex], ids[i]);


            int currentIndex = i;

            view.Clicked += _ => CardClicked?.Invoke(currentIndex);

            views.Add(view);

            // ===== POSITIONING =====
            RectTransform rt = go.GetComponent<RectTransform>();

            int x = i % cols;
            int y = i / cols;

            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0.5f, 0.5f);

            rt.sizeDelta = new Vector2(size, size);

            rt.anchoredPosition = new Vector2(
                offsetX + (x + 0.5f) * size,
               -(offsetY + (y + 0.5f) * size)
            );
            // ========================
        }
    }

    // ----------- SAFE METHODS -----------

    private void SafeHide(int index)
    {
        if (index < 0 || index >= views.Count)
            return;

        var v = views[index];
        if (v == null)
            return;

        v.SetMatched();
        v.gameObject.SetActive(false);
    }

    public void SetMatched(CardModel a, CardModel b)
    {
        SafeHide(a.Index);
        SafeHide(b.Index);
    }

    public void HideCard(int index)
    {
        if (index < 0 || index >= views.Count)
            return;

        views[index].gameObject.SetActive(false);
    }

    public void ShowFlip(int index, bool show, bool playSound = true)
    {
        if (index < 0 || index >= views.Count) return;

        views[index].Flip(show, playSound);
    }


    public CardView GetView(int index)
    {
        if (index < 0 || index >= views.Count)
            return null;

        return views[index];
    }

    public bool IsValid(int index)
    {
        return index >= 0 &&
               index < views.Count &&
               views[index] != null &&
               views[index].gameObject.activeSelf;
    }

    void Clear()
    {
        foreach (Transform t in area)
            Destroy(t.gameObject);

        views.Clear();
    }

    public int[] GetCardIds()
{
    int[] ids = new int[views.Count];

    for (int i = 0; i < views.Count; i++)
        ids[i] = views[i].CardId;

    return ids;
}

public int[] GetMatchedIndexes()
{
    var list = new List<int>();

    for (int i = 0; i < views.Count; i++)
        if (!views[i].gameObject.activeSelf)
            list.Add(i);

    return list.ToArray();
}

}
