using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("UI settings")]
    public GameObject pauseMenu;
    public GameObject pauseButton;
    public GameObject eogMenu;
    public List<Image> stars;
    public Text scoreText;
    public Text eogScoreText;
    public Button eogNextLevelButton;

    [Header("Safe zone")]
    public Vector2 safeZone;

    [Header("Star conditions")]
    public int starOne;
    public int starTwo;
    public int starThree;

    [Header("Level settings")]
    public CameraController cameraController;
    public List<GameObject> bird;
    public string nextLevel;
    public int level;

    [HideInInspector]
    public bool forceQuit;
    [HideInInspector]
    public bool EOG;
    [HideInInspector]
    public bool pause;
    [HideInInspector]
    public bool cameraDragEnabled;

    public bool waitForRbsToStop = false;

    bool starEogCheck = false;
    int numOfStars;
    int score;
    int numOfPigs;

    bool writeScore;

    bool startCounting = false;
    float waitLimit = 5f;
    float timeLimit = 5f;

    Rigidbody2D[] listOfRbs;

    public static GameManager instance;

    // Use this for initialization
    void Awake()
    {
        if (!instance)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        Time.timeScale = 1;

        cameraController.target = bird[0];

        forceQuit = false;
        EOG = false;
        pause = false;
        cameraDragEnabled = true;

        numOfStars = 3;
        score = 0;
        numOfPigs = 0;

        scoreText.text = "Score: " + score;
    }

    void Start()
    {
        SoundManager.instance.Play("level start");
    }

    // Update is called once per frame
    void Update()
    {
        if (EOG || pause)
            return;

        if (startCounting)
            waitLimit -= Time.deltaTime;

        if (starEogCheck)
        {
            IsEog();
            return;
        }

        if (waitForRbsToStop && !IsSomethingMoving() || waitLimit <= 0)
        {
            GetBirdReadyForLaunch();
            waitForRbsToStop = false;
            waitLimit = timeLimit;
            startCounting = false;
        }
    }

    public void RemoveBird()
    {
        listOfRbs = null;
        bird.RemoveAt(0);

        if (bird.Count == 0)
        {
            starEogCheck = true;
            startCounting = true;
            return;
        }
        
        // if something is moving wait before setting camera, also start counting so there is no much waiting
        if (IsSomethingMoving())
        {
            startCounting = true;
            waitForRbsToStop = true;
            return;
        }

        GetBirdReadyForLaunch();
    }

    void GetBirdReadyForLaunch()
    {
        bird[0].GetComponent<BirdController>().PrepareForLaunch();
        cameraController.target = bird[0];
        cameraController.goToTarget = true;
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = "Score: " + score;
    }

    public void AddPig()
    {
        numOfPigs++;
    }

    public void RemovePig()
    {
        numOfPigs--;

        if (numOfPigs == 0)
            starEogCheck = true;
    }

    void IsEog()
    {
        if (startCounting && bird.Count==0 && waitLimit<=0)
        {
            EOG = true;
            starEogCheck = false;
            ShowEog();
        }

        if (IsSomethingMoving())
            return;

        EOG = true;
        starEogCheck = false;

        if (bird.Count > 0)
        {
            cameraController.target = bird[0];
            cameraController.EogGoToTarget = true;
            StartCoroutine(AddSurvivedBirdPoints());
        }
        else
            ShowEog();
    }

    bool IsSomethingMoving()
    {
        // find rbs
        if (listOfRbs == null)
            listOfRbs = FindObjectsOfType<Rigidbody2D>();

        // if some rb is awake then return
        foreach (Rigidbody2D rb in listOfRbs)
            if (rb != null && !rb.IsSleeping())
                return true;

        return false;
    }

    // add points
    IEnumerator AddSurvivedBirdPoints()
    {
        yield return new WaitForSeconds(.7f);

        if (forceQuit)
            yield break;

        for (int i = 0; i < bird.Count; i++)
        {
            bird[i].GetComponent<BirdController>().AddPoints();
            yield return new WaitForSeconds(.7f);
            if (forceQuit)
                yield break;
        }

        yield return new WaitForSeconds(1);
        if (forceQuit)
            yield break;
        ShowEog();
    }

    void ShowEog()
    {
        eogMenu.SetActive(true);
        pauseButton.SetActive(false);
        eogScoreText.text = "Score: " + score;

        if (score < starThree)
        {
            stars[2].color = Color.black;
            numOfStars = 2;
        }
        if (score < starTwo)
        {
            stars[1].color = Color.black;
            numOfStars = 1;
        }
        if (score < starOne)
        {
            stars[0].color = Color.black;
            numOfStars = 0;
        }

        if (PlayerPrefs.GetInt("stars_level_" + level) < numOfStars)
            PlayerPrefs.SetInt("stars_level_" + level, numOfStars);

        // if next level exists and player have one star unlock next level button
        if (numOfStars > 0 || level < PlayerPrefs.GetInt("unlocked_level"))
            eogNextLevelButton.interactable = true;
        if (nextLevel == "")
            eogNextLevelButton.interactable = false;

        // check if u have unlocked next level
        if (numOfStars > 0 && level == PlayerPrefs.GetInt("unlocked_level"))
            PlayerPrefs.SetInt("unlocked_level", level + 1);

        if (numOfStars > 0)
            SoundManager.instance.Play("level won");
        else
            SoundManager.instance.Play("level failed");
    }

    #region button functions
    public void ToMenuScene()
    {
        EOG = true;
        SoundManager.instance.Play("button back");
        SceneManager.LoadScene("Menu");
    }

    public void ToNextLevelScene()
    {
        EOG = true;
        SoundManager.instance.Play("button confirm");
        SceneManager.LoadScene(nextLevel);
    }

    public void ResetScene()
    {
        EOG = true;
        SoundManager.instance.Play("button confirm");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PauseToggle()
    {
        SoundManager.instance.Play("button confirm");

        pause = !pause;
        pauseMenu.SetActive(pause);
        pauseButton.SetActive(!pause);

        if (pause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    public void ToggleSound()
    {
        SoundManager.instance.ToogleSound();
    }
    #endregion

    void OnApplicationQuit()
    {
        forceQuit = true;
    }
}
