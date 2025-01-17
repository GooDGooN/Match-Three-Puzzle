using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameMain : MonoBehaviour
{
    public GameObject CurrentFocus;
    public GameObject OptionDetail;
    public GameObject TutorialDetail;
    public GameObject QuitDetail;
    public TMP_Text HighScore;

    public Slider MusicSlider;
    public Slider SoundSlider;
    private void Start()
    {
        MusicSlider.value = GameSystem.Instance.GetPlayerPref(PlayerPrefType.Music);
        SoundSlider.value = GameSystem.Instance.GetPlayerPref(PlayerPrefType.Sound);
        HighScore.text = ((int)GameSystem.Instance.GetPlayerPref(PlayerPrefType.HighScore)).ToString();
    }

    public void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
        {
            if (CurrentFocus == gameObject)
            {
                ShowQuit();
            }
            else
            {
                OptionBack();
            }

        }
    }

    public void ShowOption()
    {
        CurrentFocus = OptionDetail;
        OptionDetail.SetActive(true);
    }

    public void ShowTutorial()
    {
        CurrentFocus = TutorialDetail;
        TutorialDetail.SetActive(true);
    }

    public void ShowQuit()
    {
        CurrentFocus = QuitDetail;
        QuitDetail.SetActive(true);
    }

    public void OptionBack()
    {
        GameSystem.Instance.SaveOptions();
        CurrentFocus = gameObject;
        OptionDetail.SetActive(false);
        TutorialDetail.SetActive(false);
        QuitDetail.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void AudioVolumeChange()
    {
        GameSystem.Instance.SoundVolume = SoundSlider.value;
        GameSystem.Instance.MusicVolume = MusicSlider.value;
    }

}
