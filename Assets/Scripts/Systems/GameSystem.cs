using UnityEngine;

public enum PlayerPrefType
{
    HighScore,
    Music,
    Sound,
    Coin,
}

public class GameSystem : Singleton<GameSystem>
{
    public float MusicVolume;
    public float SoundVolume;
    protected override void Awake()
    {
        MusicVolume = GetPlayerPref(PlayerPrefType.Music);
        SoundVolume = GetPlayerPref(PlayerPrefType.Sound);
        Application.targetFrameRate = 60;
        base.Awake();
    }

    public void SetPlayerPref(PlayerPrefType playerPref, float value)
    {
        switch (playerPref)
        {
            case PlayerPrefType.HighScore:
                PlayerPrefs.SetFloat("ThreeMatchPuzzleHighScore", value);
                break;
            case PlayerPrefType.Music:
                PlayerPrefs.SetFloat("ThreeMatchPuzzleMusicVolume", value);
                break;
            case PlayerPrefType.Sound:
                PlayerPrefs.SetFloat("ThreeMatchPuzzleSoundVolume", value);
                break;
            case PlayerPrefType.Coin:
                PlayerPrefs.SetFloat("ThreeMatchPuzzleCoin", value);
                break;
        }
    }
    public void SetPlayerPref(PlayerPrefType playerPref, int value)
    {
        SetPlayerPref(playerPref, (float)value);
    }

    public float GetPlayerPref(PlayerPrefType playerPref)
    {
        var result = 0.0f;
        switch (playerPref)
        {
            case PlayerPrefType.HighScore:
                result = PlayerPrefs.GetFloat("ThreeMatchPuzzleHighScore");
                break;
            case PlayerPrefType.Music:
                result = PlayerPrefs.GetFloat("ThreeMatchPuzzleMusicVolume");
                if(!PlayerPrefs.HasKey("ThreeMatchPuzzleMusicVolume"))
                {
                    result = 0.5f;
                }
                break;
            case PlayerPrefType.Sound:
                result = PlayerPrefs.GetFloat("ThreeMatchPuzzleSoundVolume");
                if (!PlayerPrefs.HasKey("ThreeMatchPuzzleSoundVolume"))
                {
                    result = 0.5f;
                }
                break;
            case PlayerPrefType.Coin:
                result = PlayerPrefs.GetFloat("ThreeMatchPuzzleCoin");
                if (!PlayerPrefs.HasKey("ThreeMatchPuzzleCoin"))
                {
                    result = 0;
                }
                break;
        }
        return result;
    }

    public void SaveOptions()
    {
        SetPlayerPref(PlayerPrefType.Music, MusicVolume);
        SetPlayerPref(PlayerPrefType.Sound, SoundVolume);
    }
}
