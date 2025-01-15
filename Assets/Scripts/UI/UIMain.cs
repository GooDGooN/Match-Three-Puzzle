using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoBehaviour
{
    public GameObject CurrentFocus;
    public GameObject OptionDetail;
    public GameObject QuitDetail;

    public Slider MusicSlider;
    public Slider SoundSlider;
    private void Start()
    {
        MusicSlider.value = GameSystem.Instance.GetPlayerPref(PlayerPrefType.Music);
        SoundSlider.value = GameSystem.Instance.GetPlayerPref(PlayerPrefType.Sound);
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
