using TMPro;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections;

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

    int maxAllowedClicks; 

    public GameObject losePanel;

    GameModel model;

    bool isComparing = false;

    [SerializeField] float previewTime = 0.5f;
    bool isPreviewing = false;


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

        maxAllowedClicks = totalPairs * 2;

        totalClicks = 0;
        UpdateClickUI();

        UpdatePairUI();

        winPanel.SetActive(false);

        if (losePanel != null)
            losePanel.SetActive(false);

        StartCoroutine(PreviewRoutine());
        Subscribe();
    }



    void OnCardClicked(int index)
    {
        if (isComparing || isPreviewing) return;

        var view = board.GetView(index);
        if (view == null) return;

        totalClicks++;
        UpdateClickUI();

        if (totalClicks >= maxAllowedClicks)
        {
            LoseGame();
            return;
        }

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

            matchedPairs++;         

            UpdatePairUI();

            CheckWin();             

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
            pairText.text = $"Pairs: {matchedPairs} / {totalPairs}";

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
            clickText.text = $"Clicks: {totalClicks} / {maxAllowedClicks}";
    }

    void LoseGame()
    {
        Debug.Log("YOU LOSE – max clicks reached");

        if (losePanel != null)
            losePanel.SetActive(true);

        isComparing = true;   // stop further play
    }

    public IEnumerator PreviewRoutine()
    {
        isPreviewing = true;
        isComparing = true;   // block input

        // 1. Flip all cards open
        for (int i = 0; i < board.Count; i++)
            board.ShowFlip(i, true);

        // 2. Wait preview time
        yield return new WaitForSeconds(previewTime);

        // 3. Flip all back
        for (int i = 0; i < board.Count; i++)
            board.ShowFlip(i, false);

        isPreviewing = false;
        isComparing = false;
    }



}
