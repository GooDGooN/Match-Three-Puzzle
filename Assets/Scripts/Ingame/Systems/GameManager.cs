using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public TMP_Text ScoreTMP;

    public const int PieceScore = 10;
    public int TotalScore = 0;
    public int Combo = 0;

    protected override void Awake()
    {
        base.Awake();
        
    }

    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        ScoreTMP.text = $"Score : {TotalScore}";
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
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
