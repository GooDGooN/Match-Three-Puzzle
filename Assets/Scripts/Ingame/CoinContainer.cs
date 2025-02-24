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

    public void SetCoinValue(int saved, int gained)
    {
        gainedCoin = gained;
        savedCoin = localCoin = saved;
        CoinValue.text = saved.ToString();
    }

    public IEnumerator ScoreToCoin(int score)
    {
        count = Mathf.CeilToInt(score / 100);
        while(count-- > 0)
        {
            var coin = PickCoin();
            coin.transform.position = ScoreObj.transform.position;
            coin.transform.localScale = Vector3.zero;
            coin.transform
                .DOScale(new Vector3(-1.0f, 1.0f, 1.0f), 0.25f);
            coin.transform
                .DOMove(IconObj.transform.position, 0.5f)
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
