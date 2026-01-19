using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject continueButton;

    void Start()
    {
        // show continue only if save exists
        continueButton.SetActive(
            SaveManager.Load() != null
        );
    }

    public void StartGrid(int size)
    {
        GameSettings.Rows = size;
        GameSettings.Cols = size;

        SaveManager.Clear();          // new game
        SceneManager.LoadScene("GameScene");
    }

    public void StartGrid5x6()
    {
        GameSettings.Rows = 5;
        GameSettings.Cols = 6;

        SaveManager.Clear();
        SceneManager.LoadScene("GameScene");
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
