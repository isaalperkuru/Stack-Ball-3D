using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    private Text scoreText;

    public int score = 0;
    void Awake()
    {
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        PlayerPrefs.GetInt("Score", 0);
        MakeSingleton();
    }

    private void Start()
    {
        AddScore(0);
    }

    private void Update()
    {
        if(scoreText == null)
        {
            scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
            AddScore(0);
        }
    }
    void MakeSingleton()
    {
        if (instance != null)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void AddScore(int amount)
    {
        PlayerPrefs.SetInt("Score", (PlayerPrefs.GetInt("Score") + amount));
        //score += amount;
        if (/*score*/ PlayerPrefs.GetInt("Score") > PlayerPrefs.GetInt("HighScore", 0))
            PlayerPrefs.SetInt("HighScore", PlayerPrefs.GetInt("Score"));

        scoreText.text = PlayerPrefs.GetInt("Score").ToString();
            //score.ToString();
    }

    public void ResetScore()
    {
        PlayerPrefs.SetInt("Score", 0);
        //score = 0;
    }
}
