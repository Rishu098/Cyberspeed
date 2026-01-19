using System;
using System.Collections.Generic;
using UnityEngine;

public class GameModel
{
    public int Score { get; private set; }

    public event Action<CardModel, CardModel, bool> OnCompared;
    public event Action<int> OnScoreChanged;

    List<CardModel> cards = new();
    List<CardModel> open = new();

    int combo = 0;   // streak system

    public CardModel GetCard(int index)
        => cards[index];

    public void CreateBoard(int pairs)
    {
        cards.Clear();

        for (int i = 0; i < pairs; i++)
        {
            cards.Add(new CardModel { Id = i });
            cards.Add(new CardModel { Id = i });
        }

        Shuffle();

        // reset score
        Score = 0;
        combo = 0;
        OnScoreChanged?.Invoke(Score);
    }

    public void Select(int index, int cardId)
    {
        var card = new CardModel
        {
            Index = index,
            Id = cardId
        };

        open.Add(card);

        if (open.Count >= 2)
            Compare();
    }

    void Compare()
    {
        var a = open[0];
        var b = open[1];

        bool match = a.Id == b.Id;

        if (match)
        {
            combo++;

            // base + combo bonus
            AddScore(10 + combo * 2);
        }
        else
        {
            combo = 0;

            // small penalty but never below 0
            AddScore(-2);
        }

        OnCompared?.Invoke(a, b, match);

        open.Clear();
    }

    void AddScore(int amount)
    {
        Score = Mathf.Max(0, Score + amount);
        OnScoreChanged?.Invoke(Score);
    }

    void Shuffle()
    {
        var rng = new System.Random();

        for (int i = 0; i < cards.Count; i++)
        {
            int r = rng.Next(i, cards.Count);
            (cards[i], cards[r]) =
            (cards[r], cards[i]);
        }
    }

    // Optional bonus from controller
    public void AddBonus(int amount)
    {
        AddScore(amount);
    }

    public void RestoreScore(int s)
    {
        Score = s;
        OnScoreChanged?.Invoke(Score);
    }


}
