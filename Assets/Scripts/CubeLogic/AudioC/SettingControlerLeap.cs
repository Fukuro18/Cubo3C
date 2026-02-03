using Leap.PhysicalHands;

using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingControlerLeap : MonoBehaviour

{
    public PhysicalHandsSlider volumeSlider;
    public PhysicalHandsSlider brightnessSlider;

    private bool isInitializing = true;

    void Start()
    {
        ApplyVolume(PlayerPrefs.GetFloat("volume", 1f));
        ApplyBrightness(PlayerPrefs.GetFloat("brightness", 1f));
        isInitializing = false;
    }

    public void OnVolumeChange(float value)
    {
        if (isInitializing) return;
        ApplyVolume(value);
        PlayerPrefs.SetFloat("volume", value);
        PlayerPrefs.Save();
    }

    void ApplyVolume(float value)
    {
        AudioListener.volume = value;
    }

    public void OnBrightnessChange(float value)
    {
        if (isInitializing) return;
        ApplyBrightness(value);
        PlayerPrefs.SetFloat("brightness", value);
        PlayerPrefs.Save();
    }

    void ApplyBrightness(float value)
    {
        BrightnessManager.SetBrightness(value);
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
