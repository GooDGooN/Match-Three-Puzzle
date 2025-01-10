using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITimeOver : MonoBehaviour
{
    public GameObject[] Childrens;
    public TMP_Text CurrentScore;
    public TMP_Text HighScore;
    public GameObject NewRecord;

    private float backgroundColorLerpValue;
    private int index;
    private bool skipable;
    private readonly float time = 0.75f;

    private void OnEnable()
    {
        index = 2;
        backgroundColorLerpValue = 0f;
        skipable = false;
        Childrens[0].GetComponent<Image>().color = Color.clear;
        Childrens[1].transform.localPosition = Vector3.up * 600.0f;
        Childrens[1].transform.DOLocalMove(Vector3.up * 220.0f, 2.0f).SetEase(Ease.OutBounce).onComplete = ShowScores;

        var highScore = PlayerPrefs.GetInt("ThreeMatchPuzzleHighScore");
        var currentScore = GameManager.Instance.TotalScore;
        CurrentScore.text = GameManager.Instance.TotalScore.ToString();
        HighScore.text = highScore.ToString();

        if (highScore < currentScore)
        {
            NewRecord.SetActive(true);
            PlayerPrefs.SetInt("ThreeMatchPuzzleHighScore", (int)currentScore);
            HighScore.text = currentScore.ToString();
        }
    }

    private void Update()
    {
        backgroundColorLerpValue += backgroundColorLerpValue < 1.0f ? Time.deltaTime : 0f;
        Childrens[0].GetComponent<Image>().color = Color.Lerp(Color.clear, new Color32(0, 0, 0, 240), backgroundColorLerpValue);

        if(skipable)
        {
            if(Input.GetMouseButtonDown(0))
            {
                for(int i = 2; i < Childrens.Length; i++)
                {
                    Childrens[i].transform.DOKill();
                    Childrens[i].transform.localScale = Vector3.one;
                }
            }
        }
    }

    private void ShowScores()
    {
        Childrens[index++].transform.DOScale(Vector3.one, time).SetEase(Ease.InOutElastic).onComplete = PopDone;
        skipable = true;    
    }

    public void PopDone() 
    {
        if(index < 6)
        {
            Childrens[index++].transform.DOScale(Vector3.one, time).SetEase(Ease.InOutElastic).onComplete = PopDone;
        }
        if(index == 5)
        {
            Childrens[index++].transform.DOScale(Vector3.one, time).SetEase(Ease.InOutElastic).onComplete = PopDone;
        }
    }

    public void Restart()
    {

    }

    public void GotoMain()
    {

    }
}
