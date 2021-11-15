using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class GameUI : MonoBehaviour
{
    public GameObject homeUI, inGameUI, GameoverUI, FinishUI;
    public GameObject allbtns;

    private bool btns;

    [Header("PreGame")]
    public Button soundBtn;
    public Sprite soundOnSpr, soundOffSpr;

    [Header("InGame")]
    public Image levelSlider;
    public Image currentLevelImg;
    public Image nextLevelImg;
    public Text currentLevelText;
    public Text nextInGameLevelText;

    [Header("GameOver")]
    public Text score;
    public Text bestScore;

    [Header("Finish")]
    public Text nextLevelText;

    private Material playerMat;
    private Player player;
    private ScoreManager scoreManager;

    void Awake()
    {
        playerMat = FindObjectOfType<Player>().transform.GetChild(0).GetComponent<MeshRenderer>().material;
        player = FindObjectOfType<Player>();

        levelSlider.transform.parent.GetComponent<Image>().color = playerMat.color + Color.gray;
        levelSlider.color = playerMat.color;
        currentLevelImg.color = playerMat.color;
        nextLevelImg.color = playerMat.color;

        soundBtn.onClick.AddListener(() => SoundManager.instance.SoundOnOff());

        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
    }

    void Update()
    {
        if(player.playerState == Player.PlayerState.Prepare)
        {
            if (SoundManager.instance.sound && soundBtn.GetComponent<Image>().sprite != soundOnSpr)
                soundBtn.GetComponent<Image>().sprite = soundOnSpr;
            else if (!SoundManager.instance.sound && soundBtn.GetComponent<Image>().sprite != soundOffSpr)
                soundBtn.GetComponent<Image>().sprite = soundOffSpr;

            UpdateInGameLevel();

        }

        if (player.playerState == Player.PlayerState.Prepare && Input.GetMouseButtonDown(0) && !IgnoreUI())
        {
            player.playerState = Player.PlayerState.Playing;
            homeUI.SetActive(false);
            inGameUI.SetActive(true);
        }

        if(player.playerState == Player.PlayerState.Finish)
        {
            FinishUI.SetActive(true);
            UpdateLevel();
        }

        if(player.playerState == Player.PlayerState.Died)
        {
            UpdateScore();
            GameoverUI.SetActive(true);
        }
    }

    private void UpdateScore()
    {
        score.text = PlayerPrefs.GetInt("Score").ToString();
        bestScore.text = PlayerPrefs.GetInt("HighScore").ToString();
    }

    private void UpdateInGameLevel()
    {
        currentLevelText.text = PlayerPrefs.GetInt("Level").ToString();
        nextInGameLevelText.text = (PlayerPrefs.GetInt("Level") + 1).ToString();
    }

    private void UpdateLevel()
    {
        nextLevelText.text = "Level " + PlayerPrefs.GetInt("Level").ToString();
    }

    private bool IgnoreUI()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);
        for (int i = 0; i < raycastResults.Count; i++)
        {
            if(raycastResults[i].gameObject.GetComponent<IgnoreUI>() != null)
            {
                raycastResults.RemoveAt(i);
                i--;
            }
        }

        return raycastResults.Count > 0;
    }

    public void LevelSliderFill(float fillAmount)
    {
        levelSlider.fillAmount = fillAmount;
    }

    public void Settings()
    {
        btns = !btns;
        allbtns.SetActive(btns);
    }
}
