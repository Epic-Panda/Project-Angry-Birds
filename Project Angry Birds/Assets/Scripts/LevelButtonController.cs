using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelButtonController : MonoBehaviour
{
    public List<Image> stars;
    public Text levelText;

    int level;

    // Use this for initialization
    void Start()
    {
        level = int.Parse(levelText.text);

        // if level is not unlocked yet than lock button
        if (PlayerPrefs.GetInt("unlocked_level") < level)
        {
            gameObject.GetComponent<Button>().interactable = false;
            return;
        }

        int numOfStars = PlayerPrefs.GetInt("stars_level_" + level);

        if (numOfStars > 0)
            stars[0].color = Color.white;
        if (numOfStars > 1)
            stars[1].color = Color.white;
        if (numOfStars > 2)
            stars[2].color = Color.white;
    }

    public void GoToLevel()
    {
        SoundManager.instance.Play("button confirm");
        SceneManager.LoadScene("Level " + level);
    }
}
