using UnityEngine;
using UnityEngine.UI;

public class UIPause : MonoBehaviour
{
    public GameObject OptionDetail;
    public GameObject QuitDetail;

    public Slider MusicSlider;
    public Slider SoundSlider;


    public void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
        {
            if(GameManager.Instance.CurrentFocus != gameObject)
            {
                OptionBack();
            }
        }
    }
    public void Resume()
    {
        GameManager.Instance.ResumeGame();
    }

    public void ShowOption()
    {
        OptionDetail.SetActive(true);
    }

    public void ShowQuit()
    {
        QuitDetail.SetActive(true);
    }

    public void OptionBack()
    {
        GameManager.Instance.CurrentFocus = gameObject;
        OptionDetail.SetActive(false);
        QuitDetail.SetActive(false);
    }


    public void SaveOption()
    {
        GameManager.Instance.SetPlayerPref(PlayerPrefType.Music, MusicSlider.value);
        GameManager.Instance.SetPlayerPref(PlayerPrefType.Sound, SoundSlider.value);
    }


    
}
