using UnityEngine;
using UnityEngine.Audio;

public class AudioSettingsManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    void Start()
    {
        // If player has saved settings, use them
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            float v = PlayerPrefs.GetFloat("MasterVolume");
            SetMasterVolume(v);
        }
        else
        {
            // No saved value → default to max
            SetMasterVolume(1f);
        }
    }

    public void SetMasterVolume(float value)
    {
        float dB = Mathf.Lerp(-80f, 0f, value);
        audioMixer.SetFloat("MasterVolume", dB);
    }
}
