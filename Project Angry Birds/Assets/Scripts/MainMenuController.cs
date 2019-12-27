using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject credits;

    public void LevelSelection()
    {
        if (PlayerPrefs.GetInt("unlocked_level") == 0)
            PlayerPrefs.SetInt("unlocked_level", 1);

        SoundManager.instance.Play("button confirm");

        SceneManager.LoadScene("LevelSelection");
    }

    public void MainMenuCredits()
    {
        SoundManager.instance.Play("button confirm");
        mainMenu.SetActive(!mainMenu.active);
        credits.SetActive(!credits.active);
    }

    public void ToggleSound()
    {
        SoundManager.instance.ToogleSound();
    }

    public void Quit()
    {
        SoundManager.instance.Play("button confirm");

        Application.Quit();
    }
}
