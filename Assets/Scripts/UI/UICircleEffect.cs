using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UICircleEffect : MonoBehaviour
{
    [SerializeField] private bool working;
    private void Start()
    {
        Time.timeScale = 1.0f;
        FadeIn();
    }
    public void FadeIn()
    {
        if (!working)
        {
            working = true;
            transform.localScale = Vector3.one;
            transform.DOScale(Vector3.zero, 0.75f).SetEase(Ease.OutExpo).onComplete = () => { working = false; };
        }
    }

    public void FadeOut(int sceneNum)
    {
        if (!working)
        {
            working = true;
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 0.75f).SetEase(Ease.OutExpo).SetUpdate(true).onComplete = () => ChangeScene(sceneNum);
        }
    }

    public void ChangeScene(int sceneNum)
    {
        working = false;
        SceneManager.LoadScene(sceneNum);
    }
}
