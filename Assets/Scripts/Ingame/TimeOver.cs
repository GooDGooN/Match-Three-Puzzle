using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TimeOver : MonoBehaviour
{
    public GameObject[] Childrens;
    public TMP_Text CurrentScore;
    public TMP_Text HighScore;
    public GameObject NewRecord;
    public AudioPlayer SoundPlayer;
    public CoinContainer CoinController;
    public GainedCoin GainedCoin;
    public GameObject CoinParent;

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
        Childrens[1].transform.DOLocalMove(Vector3.up * 190.0f, 2.0f).SetEase(Ease.OutBounce).onComplete = ShowChildrens;

        var highScore = GameSystem.Instance.GetPlayerPref(PlayerPrefType.HighScore);
        var currentScore = (int)GameManager.Instance.TargetScore;
        CurrentScore.text = GameManager.Instance.TotalScore.ToString();
        HighScore.text = highScore.ToString();

        if (highScore < currentScore)
        {
            NewRecord.SetActive(true);
            GameSystem.Instance.SetPlayerPref(PlayerPrefType.HighScore, currentScore);
            HighScore.text = currentScore.ToString();
        }

        var gainedCoin = (int)GameManager.Instance.TotalScore / 100;
        var savedCoin = (int)GameSystem.Instance.GetPlayerPref(PlayerPrefType.Coin);
        GainedCoin.Gained = gainedCoin;
        CoinParent.transform.parent = transform;
        CoinController.GetComponentInParent<Transform>().parent = transform;
        CoinController.SetCoinValue(savedCoin, gainedCoin);
        GameSystem.Instance.SetPlayerPref(PlayerPrefType.Coin, savedCoin + gainedCoin);
    }

    private void Update()
    {
        backgroundColorLerpValue += backgroundColorLerpValue < 1.0f ? Time.deltaTime : 0f;
        Childrens[0].GetComponent<Image>().color = Color.Lerp(Color.clear, new Color32(0, 0, 0, 240), backgroundColorLerpValue);

        if(skipable)
        {
            if(Input.GetMouseButtonDown(0))
            {
                CoinController.Skip();
                for (int i = 2; i < Childrens.Length; i++)
                {
                    Childrens[i].transform.DOKill();
                    Childrens[i].transform.localScale = Vector3.one;
                }
            }
        }
    }

    private void ShowChildrens()
    {
        Childrens[index++].transform.DOScale(Vector3.one, time).SetEase(Ease.InOutElastic).onComplete = PopDone;
        skipable = true;    
    }

    public void PopDone() 
    {
        SoundPlayer.PlayAudio(5);
        if (index == Childrens.Length)
        {
            StartCoroutine(CoinController.CoinTransfer((int)GameManager.Instance.TargetScore));
        }
        if (index < Childrens.Length - 1)
        {
            Childrens[index++].transform
                .DOScale(Vector3.one, time)
                .SetEase(Ease.InOutElastic)
                .onComplete = PopDone;
        }
        if(index == Childrens.Length - 1)
        {
            Childrens[index++].transform
                .DOScale(Vector3.one, time)
                .SetEase(Ease.InOutElastic);
        }
        
    }

    public void GotoMain()
    {

    }
}
