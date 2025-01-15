using UnityEngine;
using UnityEngine.UI;

public class UIPause : MonoBehaviour
{
    public GameObject CurrentFocus;
    public GameObject OptionDetail;
    public GameObject QuitDetail;

    public Slider MusicSlider;
    public Slider SoundSlider;

    public AudioPlayer SoundPlayer;

    private void Start()
    {
        MusicSlider.value = GameSystem.Instance.GetPlayerPref(PlayerPrefType.Music);
        SoundSlider.value = GameSystem.Instance.GetPlayerPref(PlayerPrefType.Sound);
    }
    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
        {
            if(CurrentFocus == gameObject)
            {
                Resume();
            }
            else
            {
                OptionBack();
            }
            
        }
    }
    public void Resume()
    {
        CurrentFocus = null;
        GameManager.Instance.ResumeGame();
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
        CurrentFocus = gameObject;
        GameSystem.Instance.SaveOptions();
        OptionDetail.SetActive(false);
        QuitDetail.SetActive(false);
    }

    public void AudioVolumeChange()
    {
        GameSystem.Instance.SoundVolume = SoundSlider.value;
        GameSystem.Instance.MusicVolume = MusicSlider.value;
    }
}
