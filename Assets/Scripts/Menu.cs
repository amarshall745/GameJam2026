using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject howToPlay;

    public void play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void quit()
    {
        Application.Quit();
    }

    public void howToPlayy()
    {
        howToPlay.SetActive(true);
    }

    public void clsoehowToPlayy()
    {
        howToPlay.SetActive(false);
    }
}
