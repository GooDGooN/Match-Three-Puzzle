using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameIntro : MonoBehaviour
{
    public GameObject ReadyObject;
    public GameObject StartObject;
    public AudioPlayer SoundPlayer;
    public AudioPlayer MusicPlayer;
    private TextMeshProUGUI[] texts = new TextMeshProUGUI[2];
    private int count;


    private void Awake()
    {
        texts[0] = ReadyObject.GetComponentInChildren<TextMeshProUGUI>();
        texts[1] = StartObject.GetComponentInChildren<TextMeshProUGUI>();
        count = 3;

        for(int i = 0; i < texts.Length; i++)
        {
            texts[i].fontMaterial = new Material(texts[i].fontMaterial);
            texts[i].fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, 1.0f);
            texts[i].fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor, Color.white);
        }

        texts[0].text = $"{count}";
        texts[0].transform
            .DOMove(Vector3.up * 64, 0.5f)
            .SetEase(Ease.OutExpo)
            .onComplete = CountDown;
    }

    private void Start()
    {
        GameManager.Instance.isPause = true;
        SoundPlayer.PlayAudio(1);
    }

    private void CountDown()
    {
        count--;
        if(count > 0)
        {
            texts[0].text = $"{count}";
            texts[0].transform.position = new Vector3(-300.0f, 64.0f, 0);
            texts[0].transform
            .DOMove(Vector3.up * 64, 0.5f)
            .SetEase(Ease.OutExpo)
            .onComplete = CountDown;
        } 
        else
        {
            ActiveStart();
        }
    }

    private void ActiveStart()
    {
        ReadyObject.SetActive(false);
        StartObject.SetActive(true);
        StartObject.transform
            .DOScale(Vector3.one, 1.5f)
            .SetEase(Ease.OutElastic)
            .onComplete = GameStart;
        SoundPlayer.PlayAudio(2);
        MusicPlayer.PlayAudio();
    }

    private void GameStart()
    {
        GameManager.Instance.isPause = false;
        gameObject.SetActive(false);
    }

}
