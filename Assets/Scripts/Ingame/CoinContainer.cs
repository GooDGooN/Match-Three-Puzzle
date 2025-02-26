using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class CoinContainer : MonoBehaviour
{
    public GameObject CoinPrefab;
    public GameObject ScoreObj;
    public GameObject IconObj;
    public TMP_Text CoinValue;
    public GainedCoin GainedValue;
    public AudioPlayer MyAudioPlayer;
    private List<GameObject> CoinList = new();
    private int count;
    private int gainedCoin;
    private int savedCoin;
    private int localCoin;

    private void Awake()
    {
        for (int i = 0; i < 30; i++)
        {
            var obj = Instantiate(CoinPrefab, transform);
            obj.SetActive(false);
            CoinList.Add(obj);
        }
    }

    private void Start()
    {
        CoinValue.text = ((int)GameSystem.Instance.GetPlayerPref(PlayerPrefType.Coin)).ToString();
    }

    public void SetCoinValue(int saved, int gained)
    {
        gainedCoin = gained;
        savedCoin = localCoin = saved;
        CoinValue.text = saved.ToString();
    }

    public IEnumerator CoinTransfer(int score)
    {
        count = GainedValue.Gained;
        while (count-- > 0)
        {
            var coin = PickCoin();
            coin.transform.position = GainedValue.Icon.transform.position;
            coin.transform.localScale = Vector3.zero;

            GainedValue.ReduceCoin(); 

            coin.transform
                .DOScale(new Vector3(-1.0f, 1.0f, 1.0f), 0.25f);
            coin.transform
                .DOMove(IconObj.transform.position, 1.0f)
                .SetEase(Ease.OutCirc)
                .onComplete = count > 0 ? () => DisableCoin(coin) : () => DisableCoin(coin, true);

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void DisableCoin(GameObject target, bool last = false)
    {
        IconObj.transform.DOKill();
        IconObj.transform.localScale = new Vector3(-2.0f, 2.0f, 1.0f);
        IconObj.transform.DOScale(new Vector3(-1.0f, 1.0f, 1.0f), 0.5f);
        MyAudioPlayer.PlayAudio(7);

        CoinValue.text = (localCoin++).ToString();
        if (last)
        {
            CoinValue.text = (gainedCoin + savedCoin).ToString();
        }
        target.SetActive(false);       
    }

    private GameObject PickCoin()
    {
        foreach(var coin in CoinList)
        {
            if(!coin.activeSelf)
            {
                coin.SetActive(true);
                return coin;
            }
        }
        return null;
    }

    public void Skip()
    {
        count = 0;
        CoinValue.text = (savedCoin + gainedCoin).ToString();
        GainedValue.Gained = 0;
        foreach (var coin in CoinList)
        {
            if (coin.activeSelf)
            {
                coin.transform.DOKill();
                coin.SetActive(false);
            }
        }
    }
}
