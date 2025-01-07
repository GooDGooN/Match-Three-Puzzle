using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public TMP_Text ScoreTMP;

    public const int PieceScore = 10;
    public int TotalScore = 0;
    public int Combo = 0;

    public float TimeLimitValue = 0;
    public GameObject TimeLimitBarImageObject;

    protected override void Awake()
    {
        base.Awake();
        
    }

    private void Start()
    {
        TimeLimitValue = 1.0f;
    }

    // Update is called once per frame
    private void Update()
    {
        ScoreTMP.text = $"Score : {TotalScore}";
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }

        if (TimeLimitValue > 0)
        {
            TimeLimitValue -= Time.deltaTime / 60.0f;
            var rectTransform = TimeLimitBarImageObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(320.0f * TimeLimitValue, 32.0f);

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
        TotalScore += (basicScore) + (int)(comboMultiply * basicScore);
    }
}
