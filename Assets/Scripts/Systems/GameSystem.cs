using UnityEngine;

public enum PlayerPrefType
{
    HighScore,
    Music,
    Sound,
}

public class GameSystem : Singleton<GameSystem>
{
    public float MusicVolume;
    public float SoundVolume;
    protected override void Awake()
    {
        MusicVolume = GetPlayerPref(PlayerPrefType.Music);
        SoundVolume = GetPlayerPref(PlayerPrefType.Sound);
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
                break;
            case PlayerPrefType.Sound:
                result = PlayerPrefs.GetFloat("ThreeMatchPuzzleSoundVolume");
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
