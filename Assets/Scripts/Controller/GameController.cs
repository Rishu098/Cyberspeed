using UnityEngine;

public class GameController : MonoBehaviour
{
    public BoardView board;
    public TMPro.TextMeshProUGUI scoreText;

    GameModel model;

    bool isComparing = false;


    void Start()
    {
        NewGame(4,4);
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
        if (isComparing) return;   // 👈 NEW

        var view = board.GetView(index);

        model.Select(index, view.CardId);

        board.ShowFlip(index, true);
    }

    async void OnCompared(CardModel a, CardModel b, bool match)
    {
        int indexA = a.Index;
        int indexB = b.Index;

        // ---- MATCH CASE ----
        if (match)
        {
            isComparing = true;

            // 👉 WAIT so player can see second card
            await System.Threading.Tasks.Task.Delay(500);

            board.SetMatched(a, b);

            isComparing = false;
            return;
        }

        // ---- MISMATCH CASE ----
        isComparing = true;

        await System.Threading.Tasks.Task.Delay(600);

        board.ShowFlip(indexA, false);
        board.ShowFlip(indexB, false);

        isComparing = false;
    }



}
