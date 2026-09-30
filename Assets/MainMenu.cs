using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject tutorialToast;

    public void StartGame()
    {
        tutorialToast.SetActive(true);
    }

    public void YesTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void NoTutorial()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
#if UNITY_EDITOR
        // This stops play mode in the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // This closes the built application
            Application.Quit();
#endif
    }
}
