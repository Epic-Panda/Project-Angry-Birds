using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonController : MonoBehaviour {

    public void BackToMenu()
    {
        SoundManager.instance.Play("button confirm");
        SceneManager.LoadScene("Menu");
    }
}
