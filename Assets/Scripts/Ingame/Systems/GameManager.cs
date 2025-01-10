using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum PlayerPrefType
{
    HighScore,
    Music,
    Sound,
}
public class GameManager : Singleton<GameManager>
{
    public TMP_Text ScoreTMP;

    private float targetScore = 0;
    public const float PieceScore = 10;
    public float TotalScore = 0;
    public int Combo = 0;
    public bool isPause;

    public float TimeLimitValue = 0;
    public GameObject TimeLimitBarImageObject;
    private float timeLimitBarWidth;

    public GameObject CurrentFocus;
    public GameObject TimeOverObject;
    public GameObject PauseObject;

    protected override void Awake()
    {
        base.Awake();
        
    }

    private void Start()
    {
        TimeLimitValue = 1.0f;
        timeLimitBarWidth = TimeLimitBarImageObject.GetComponent<RectTransform>().rect.width;
    }

    // Update is called once per frame
    private void Update()
    {
        GamePause();
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SceneManager.LoadScene(0);
        }

        if (TotalScore < targetScore)
        {
            TotalScore += (targetScore - TotalScore) * 0.1f;
        }
        if(TotalScore > targetScore || targetScore - TotalScore < 1)
        {
            TotalScore = targetScore;
        }
        ScoreTMP.text = $"Score : {(int)TotalScore}";

        if (TimeLimitValue > 0)
        {
            if(!isPause)
            {
                TimeLimitValue -= Time.deltaTime / 60.0f;
            }
            var rectTransform = TimeLimitBarImageObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(timeLimitBarWidth * TimeLimitValue, 32.0f);

            var targetColor = new Color32();
            targetColor.a = 255;
            targetColor.b = 0;
            if (TimeLimitValue > 0.7f)
            {
                targetColor.r = 27;
                targetColor.g = 205;
            } 
            else if (TimeLimitValue > 0.3f)
            {
                targetColor.r = 180;
                targetColor.g = 180;
            } 
            else
            {
                targetColor.r = 205;
                targetColor.g = 27;
            }

            TimeLimitBarImageObject.GetComponent<Image>().color = targetColor;
        }       
    }

    private void GamePause()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
        {
            if (PauseObject.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                if (!isPause)
                {
                    isPause = true;
                    CurrentFocus = PauseObject;
                    PauseObject.SetActive(true);
                    Time.timeScale = 0;
                }
            }
        }
    }

    public void ResumeGame()
    {
        isPause = false;
        Time.timeScale = 1.0f;
        PauseObject.SetActive(false);
        CurrentFocus = null;
    }

    public void AddScore(int amount, int bombMultiply = 1)
    {
        Debug.Log($"{amount} {bombMultiply}");
        var basicScore = amount * PieceScore * bombMultiply;
        var comboMultiply = Mathf.Clamp(Combo, 0, 5) * 0.1f;
        if (bombMultiply == 1)
        {
            if (amount == 4)
            {
                basicScore *= 2;
            }
            if (amount > 4)
            {
                basicScore *= 3;
            }
        }
        targetScore += (basicScore) + (comboMultiply * basicScore);
    }

    public void TimeOver()
    {
        if (!TimeOverObject.activeSelf)
        {
            TimeOverObject.SetActive(true);
            isPause = true;
        }
    }

    public void SetPlayerPref(PlayerPrefType playerPref, float value)
    {
        var result = value.ToString();

        switch(playerPref)
        {
            case PlayerPrefType.HighScore:
                PlayerPrefs.SetString("ThreeMatchPuzzleHighScore", result);
                break;
            case PlayerPrefType.Music:
                PlayerPrefs.SetString("ThreeMatchPuzzleMusicVolume", result);
                break;
            case PlayerPrefType.Sound:
                PlayerPrefs.SetString("ThreeMatchPuzzleSoundVolume", result);
                break;
        }
    }
    public void SetPlayerPref(PlayerPrefType playerPref, int value)
    {
        SetPlayerPref(playerPref, (float)value);
    }

    public float GetPlayerPref(PlayerPrefType playerPref)
    {
        var result = 0.0f;
        switch (playerPref)
        {
            case PlayerPrefType.HighScore:
                result = float.Parse(PlayerPrefs.GetString("ThreeMatchPuzzleHighScore"));
                break;
            case PlayerPrefType.Music:
                result = float.Parse(PlayerPrefs.GetString("ThreeMatchPuzzleMusicVolume"));
                break;
            case PlayerPrefType.Sound:
                result = float.Parse(PlayerPrefs.GetString("ThreeMatchPuzzleSoundVolume"));
                break;
        }
        return result;
    }
}
