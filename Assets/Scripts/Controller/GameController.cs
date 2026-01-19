using TMPro;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine.SocialPlatforms.Impl;

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
        LoadGame();
    }

    public void NewGame(int r, int c)
    {
        model = new GameModel();
        Unsubscribe();
        model.CreateBoard((r * c) / 2);

        board.Build(r, c);

        totalPairs = (r * c) / 2;
        matchedPairs = 0;

        maxAllowedClicks = totalPairs * 2;

        totalClicks = 0;
        UpdateClickUI();

        UpdatePairUI();

        winPanel.SetActive(false);

        scoreText.text = "Score: 0";

        if (losePanel != null)
            losePanel.SetActive(false);

        StartCoroutine(PreviewRoutine());
        Subscribe();
    }



    void OnCardClicked(int index)
    {
        if (isComparing) return;

        var view = board.GetView(index);
        if (view == null) return;

        // Flip first so player sees card
        board.ShowFlip(index, true);

        // Then process logic
        totalClicks++;
        UpdateClickUI();

        model.Select(index, view.CardId);

        // After reveal → check lose
        if (totalClicks >= maxAllowedClicks)
        {
            LoseGame();
        }
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

            SoundManager.I?.PlayMatch();

            UpdatePairUI();

            CheckWin();             

            isComparing = false;
            SaveGame();
            return;
        }


        // ---- MISMATCH CASE ----
        isComparing = true;

        await System.Threading.Tasks.Task.Delay(600);

        SoundManager.I?.PlayMismatch();

        board.ShowFlip(indexA, false, false);   // silent
        board.ShowFlip(indexB, false, false);   // silent


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

        SoundManager.I?.PlayGameOver();

        if (winPanel != null)
            winPanel.SetActive(true);
    }

    public void Restart()
    {
        NewGame(4, 4);   // or your chosen size
    }

    void Subscribe()
    {
        board.CardClicked += OnCardClicked;
        model.OnCompared += OnCompared;
        model.OnScoreChanged += UpdateScoreUI;

    }

    void UpdateClickUI()
    {
        if (clickText != null)
            clickText.text = $"Clicks: {totalClicks} / {maxAllowedClicks}";
    }

    void UpdateScoreUI(int s)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + s;
    }


    void LoseGame()
    {
        Debug.Log("YOU LOSE – max clicks reached");
        SoundManager.I?.PlayGameOver();

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
            board.ShowFlip(i, true, false);   // no sound

        // 2. Wait preview time
        yield return new WaitForSeconds(previewTime);

        // 3. Flip all back
        for (int i = 0; i < board.Count; i++)
            board.ShowFlip(i, false, false);


        isPreviewing = false;
        isComparing = false;
    }

    public void SaveGame()
    {
        SaveData d = new SaveData();

        d.score = model.Score;      
        d.clicks = totalClicks;
        d.matchedPairs = matchedPairs;

        d.rows = 4;
        d.cols = 4;

        d.cardIds = board.GetCardIds();
        d.matchedIndexes = board.GetMatchedIndexes();

        SaveManager.Save(d);
    }


    public void LoadGame()
    {
        var d = SaveManager.Load();

        // --- No save → normal start ---
        if (d == null)
        {
            NewGame(4, 4);
            return;
        }

        // --- Rebuild board first ---
        NewGame(d.rows, d.cols);

        // --- RESTORE VALUES ---
        totalClicks = d.clicks;
        matchedPairs = d.matchedPairs;

        //restore score into model
        model.RestoreScore(d.score);

        UpdateClickUI();
        UpdatePairUI();

        // restore solved cards
        foreach (int i in d.matchedIndexes)
            board.HideCard(i);

        Debug.Log("Game Loaded with score: " + d.score);
    }


    void OnApplicationQuit()
    {
        SaveGame();
    }

    public void RestartNewGame()
    {
        // Optional: remove saved progress so load doesn’t restore old state
        SaveManager.Clear();

        // Start a completely fresh game
        NewGame(4, 4);

        // Re-enable input in case game ended in win/lose
        isComparing = false;

        Debug.Log("New game started");
    }

    void Unsubscribe()
    {
        if (board != null)
            board.CardClicked -= OnCardClicked;

        if (model != null)
            model.OnCompared -= OnCompared;
    }

}
