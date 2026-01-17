using System;
using System.Collections.Generic;

public class GameModel
{
    public int Score { get; private set; }

    public event Action<CardModel, CardModel, bool> OnCompared;
    public event Action<int> OnScoreChanged;

    List<CardModel> cards = new();
    List<CardModel> open = new();

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
    }

    public void Select(CardModel card)
    {
        if (card.IsMatched || card.IsFlipped)
            return;

        card.IsFlipped = true;
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
            a.IsMatched = b.IsMatched = true;
            Score += 10;
            OnScoreChanged?.Invoke(Score);
        }

        OnCompared?.Invoke(a, b, match);

        open.Clear();
    }

    void Shuffle()
    {
        var rng = new Random();

        for (int i = 0; i < cards.Count; i++)
        {
            int r = rng.Next(i, cards.Count);
            (cards[i], cards[r]) =
            (cards[r], cards[i]);
        }
    }
}
