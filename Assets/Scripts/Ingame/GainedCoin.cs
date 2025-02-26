using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GainedCoin : MonoBehaviour
{
    public GameObject Icon;
    public int Gained = 0;

    private void Update()
    {
        GetComponent<TMP_Text>().text = $"x {Gained}";
    }

    public void ReduceCoin()
    {
        Gained--;
        Gained = Gained > 0 ? Gained : 0;
        Icon.transform.DOKill();
        Icon.transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
        Icon.transform.DOScale(new Vector3(-1.0f, 1.0f, 1.0f), 0.5f);
    }
}
