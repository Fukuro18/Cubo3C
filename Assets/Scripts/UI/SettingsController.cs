using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingsController : MonoBehaviour
{
    public Slider volumeSlider;
    public Slider brightnessSlider;
    public AudioSource musicSource;

    private bool isInitializing = true;

    void Start()
    {
        // LIMPIEZA TOTAL: Borrar cualquier conexión "fantasma" del Inspector (causa probable del bug)
        volumeSlider.onValueChanged.RemoveAllListeners();
        brightnessSlider.onValueChanged.RemoveAllListeners();

        // Cargar valores sin disparar eventos de guardado
        volumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("volume", 1f));
        brightnessSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("brightness", 1f));

        ApplyVolume();
        ApplyBrightness();

        // Conectar SOLO nuestras funciones
        volumeSlider.onValueChanged.AddListener(OnVolumeChange);
        brightnessSlider.onValueChanged.AddListener(OnBrightnessChange);

        isInitializing = false;
    }

    public void OnVolumeChange(float value)
    {
        if (isInitializing) return;

        // VALIDACIÓN ESTRICTA
        if (!IsValidSender(volumeSlider.gameObject)) return;

        Debug.Log($"[Settings] Volume Changed to: {value}");
        ApplyVolume();
        PlayerPrefs.SetFloat("volume", value);
        PlayerPrefs.Save();
    }

    void ApplyVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }

    public void OnBrightnessChange(float value)
    {
        if (isInitializing) return;

        // VALIDACIÓN ESTRICTA
        if (!IsValidSender(brightnessSlider.gameObject)) return;

        Debug.Log($"[Settings] Brightness Changed to: {value}");
        ApplyBrightness();
        PlayerPrefs.SetFloat("brightness", value);
        PlayerPrefs.Save();
    }

    void ApplyBrightness()
    {
        // Delegar al Manager centralizado
        BrightnessManager.SetBrightness(brightnessSlider.value);
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Verifica si el objeto seleccionado actualmente es el que esperamos
    private bool IsValidSender(GameObject expectedSender)
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) return true;

        if (selected != expectedSender && (selected == volumeSlider.gameObject || selected == brightnessSlider.gameObject))
        {
            Debug.LogError($"[Settings] BLOCKED CROSS-TALK: Event from {expectedSender.name} triggered while selecting {selected.name}");
            return false;
        }

        return true;
    }
}
