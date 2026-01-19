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

    public void NewGame()
    {
        SaveManager.Clear();       // remove old progress
        SceneManager.LoadScene("GameScene");
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
