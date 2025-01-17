
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTutorial : MonoBehaviour
{
    private int pageNumber;
    public Sprite[] Sprites;
    public Image TutoImage1;
    public Image TutoImage2;

    public TMP_Text Title;
    public TMP_Text PageNumber;

    private void OnEnable()
    {
        pageNumber = 0;
    }

    private void Update()
    {
        Title.text = $"Tutorial {pageNumber + 1}";
        PageNumber.text = $"{pageNumber + 1}/4";
        var index = pageNumber * 2;
        TutoImage1.sprite = Sprites[index];
        TutoImage2.sprite = Sprites[index + 1];
    }

    public void NextPage()
    {
        if (pageNumber < (Sprites.Length / 2) - 1)
        {
            pageNumber++;
        }
    }
    public void PrevPage()
    {
        if (pageNumber > 0)
        {
            pageNumber--;
        }
    }
}

