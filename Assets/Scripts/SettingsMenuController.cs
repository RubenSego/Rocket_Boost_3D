using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Audio Mixer & Exposed Parameters")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string masterParam = "MasterVolume";
    [SerializeField] private string musicParam = "MusicVolume";
    [SerializeField] private string sfxParam = "SFXVolume";

    [Header("Volume Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Default Values (0 to 1)")]
    [Range(0.0001f, 1f)] [SerializeField] private float defaultMaster = 0.8f;
    [Range(0.0001f, 1f)] [SerializeField] private float defaultMusic = 0.8f;
    [Range(0.0001f, 1f)] [SerializeField] private float defaultSfx = 0.8f;

    private const string MasterKey = "Vol_Master";
    private const string MusicKey = "Vol_Music";
    private const string SfxKey = "Vol_SFX";

    private void Awake()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        LoadAndApplyVolumes();
        RegisterListeners();
    }

    private void LoadAndApplyVolumes()
    {
        float masterVal = Mathf.Clamp(PlayerPrefs.GetFloat(MasterKey, defaultMaster), 0.0001f, 1f);
        float musicVal = Mathf.Clamp(PlayerPrefs.GetFloat(MusicKey, defaultMusic), 0.0001f, 1f);
        float sfxVal = Mathf.Clamp(PlayerPrefs.GetFloat(SfxKey, defaultSfx), 0.0001f, 1f);

        if (masterSlider != null) masterSlider.SetValueWithoutNotify(masterVal);
        if (musicSlider != null) musicSlider.SetValueWithoutNotify(musicVal);
        if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(sfxVal);

        ApplyVolumeToMixer(masterParam, masterVal);
        ApplyVolumeToMixer(musicParam, musicVal);
        ApplyVolumeToMixer(sfxParam, sfxVal);
    }

    private void RegisterListeners()
    {
        if (masterSlider != null)
            masterSlider.onValueChanged.AddListener(SetMasterVolume);

        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(SetMusicVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void SetMasterVolume(float value)
    {
        float clamped = Mathf.Clamp(value, 0.0001f, 1f);
        PlayerPrefs.SetFloat(MasterKey, clamped);
        PlayerPrefs.Save();
        ApplyVolumeToMixer(masterParam, clamped);
    }

    public void SetMusicVolume(float value)
    {
        float clamped = Mathf.Clamp(value, 0.0001f, 1f);
        PlayerPrefs.SetFloat(MusicKey, clamped);
        PlayerPrefs.Save();
        ApplyVolumeToMixer(musicParam, clamped);
    }

    public void SetSFXVolume(float value)
    {
        float clamped = Mathf.Clamp(value, 0.0001f, 1f);
        PlayerPrefs.SetFloat(SfxKey, clamped);
        PlayerPrefs.Save();
        ApplyVolumeToMixer(sfxParam, clamped);
    }

    public void ResetVolumesToDefault()
    {
        SetMasterVolume(defaultMaster);
        SetMusicVolume(defaultMusic);
        SetSFXVolume(defaultSfx);

        if (masterSlider != null) masterSlider.SetValueWithoutNotify(defaultMaster);
        if (musicSlider != null) musicSlider.SetValueWithoutNotify(defaultMusic);
        if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(defaultSfx);
    }

    private void ApplyVolumeToMixer(string exposedParameterName, float linearValue)
    {
        if (audioMixer == null || string.IsNullOrEmpty(exposedParameterName))
            return;

        float dB = linearValue > 0.0001f ? Mathf.Log10(linearValue) * 20f : -80f;
        audioMixer.SetFloat(exposedParameterName, dB);
    }
}