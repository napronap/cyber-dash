using UnityEngine;
using UnityEngine.UI;

public class MusicSliderBinder : MonoBehaviour
{
    public Slider slider;
    public AudioSettingsManager audioSettings;

    void Start()
    {
        // 1) Decide what the initial volume should be
        float v = PlayerPrefs.HasKey("MasterVolume")
            ? PlayerPrefs.GetFloat("MasterVolume")
            : 1f; // default to MAX

        // 2) Set slider FIRST (this prevents volume jump)
        slider.SetValueWithoutNotify(v);

        // 3) Apply volume once so audio matches what you hear
        audioSettings.SetMasterVolume(v);

        // 4) Now hook the listener
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener(audioSettings.SetMasterVolume);
    }
}

