using TMPro;
using UnityEngine;
using System.Threading.Tasks;

public class GameController : MonoBehaviour
{
    public BoardView board;
    public TextMeshProUGUI scoreText;
    int totalPairs;
    int matchedPairs = 0;

    public GameObject winPanel;   
    public TextMeshProUGUI pairText;

    int totalClicks = 0;
    public TextMeshProUGUI clickText;


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

        totalPairs = (r * c) / 2;
        matchedPairs = 0;

        totalClicks = 0;
        UpdateClickUI();

        UpdatePairUI();

        winPanel.SetActive(false);

        Subscribe();
    }


    void OnCardClicked(int index)
    {
        if (isComparing) return;

        var view = board.GetView(index);
        if (view == null) return;

        // 👉 COUNT EVERY CARD CLICK
        totalClicks++;
        UpdateClickUI();

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

            await Task.Delay(500);

            board.SetMatched(a, b);

            matchedPairs++;          // 👈 COUNT MATCH

            UpdatePairUI();

            CheckWin();              // 👈 CHECK GAME OVER

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

    void UpdatePairUI()
    {
        if (pairText != null)
            pairText.text = $"{matchedPairs} / {totalPairs}";
    }

    void CheckWin()
    {
        if (matchedPairs >= totalPairs)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("YOU WIN!");

        if (winPanel != null)
            winPanel.SetActive(true);

        // optional sound
        // audio.PlayGameOver();
    }

    public void Restart()
    {
        NewGame(4, 4);   // or your chosen size
    }

    void Subscribe()
    {
        board.CardClicked += OnCardClicked;
        model.OnCompared += OnCompared;
    }

    void UpdateClickUI()
    {
        if (clickText != null)
            clickText.text = $"Clicks: {totalClicks}";
    }


}
