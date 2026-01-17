using UnityEngine;

public class GameController : MonoBehaviour
{
    public BoardView board;
    public TMPro.TextMeshProUGUI scoreText;

    GameModel model;

    void Start()
    {
        NewGame(2, 2);
    }

    public void NewGame(int r, int c)
    {
        model = new GameModel();

        model.CreateBoard((r * c) / 2);

        board.Build(r, c);

        board.CardClicked += OnCardClicked;

        model.OnCompared += OnCompared;
        model.OnScoreChanged +=
            s => scoreText.text = s.ToString();
    }

    void OnCardClicked(int index)
    {
        var card = model.GetCard(index);

        model.Select(card);

        board.ShowFlip(index, true);
    }

    async void OnCompared(CardModel a,
                          CardModel b,
                          bool match)
    {
        if (match)
        {
            board.SetMatched(a, b);
        }
        else
        {
            await System.Threading.Tasks
                .Task.Delay(600);

            board.ShowFlip(a.Id, false);
            board.ShowFlip(b.Id, false);
        }
    }
}
