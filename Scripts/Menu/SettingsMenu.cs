using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using Slider = UnityEngine.UI.Slider;
using Toggle = UnityEngine.UI.Toggle;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [Tooltip("0 is MasterVolume, 1 is Ambient, 2 is Effects")]
    [SerializeField] private Slider[] volumeSlider;
    
    private static readonly string[] VolumeName =
    {
        "MasterVolume", "AmbientVolume", "EffectsVolume"
    };

    struct FullscreenSetting
    {
        public string Label;
        public FullScreenMode Mode;
    }
    private static readonly FullscreenSetting[] FullScreenModes =
    {
        new FullscreenSetting()
        {
           Label = "Windowed", Mode = FullScreenMode.Windowed
        }, 
        new FullscreenSetting()
        {
            Label = "Borderless", Mode = FullScreenMode.FullScreenWindow
        },
        new FullscreenSetting()
        {
            Label = "Fullscreen", Mode = FullScreenMode.ExclusiveFullScreen
        },
    };

    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown fullScreenModeDropdown;
    [SerializeField] private Toggle vSyncToggle;
    private FullScreenMode _fullScreenMode;
    private bool _vSync;
    private Resolution _resolution;

    private void OnDisable()
    {
        SaveSettings();
    }

    private void OnEnable()
    {
        Setup();
    }

    private void OnValidate()
    {
        Setup();
    }

    public void Setup()
    {
        SetupVolume();
        SetupResolution();
        SetupVSync();
        SetupFullScreenMode();
    }

    void SetupVolume()
    {
        if (volumeSlider == null || volumeSlider.Length == 0) return;
        if (PlayerPrefs.HasKey(VolumeName[0]))
        {
            for (int i = 0; i < VolumeName.Length; i++)
            {
                float value = VolumeToLinear(PlayerPrefs.GetFloat(VolumeName[i]));

                volumeSlider[i].value = value;

                SetVolume(value, i);
            }
        }
        else
        {
            for (int i = 0; i < VolumeName.Length; i++)
            {
                mixer.GetFloat(VolumeName[i], out var value);

                volumeSlider[i].value = VolumeToLinear(value);
            }
        }
        
        volumeSlider[0].onValueChanged.AddListener(SetMasterVolume);
        volumeSlider[1].onValueChanged.AddListener(SetMusicVolume);
        volumeSlider[2].onValueChanged.AddListener(SetSoundVolume);
    }

    void SetupResolution()
    {
        _resolution = Screen.currentResolution;

        if (resolutionDropdown == null) return;
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        var res = Screen.resolutions;

        var currentIndex = 0;
        for (var i = 0; i < res.Length; i++)
        {
            var t = res[i];
            options.Add(new TMP_Dropdown.OptionData($"{t.width}x{t.height}"));

            if (t.width == _resolution.width && t.height == _resolution.height) currentIndex = i;
        }
        
        resolutionDropdown.options = options;
        resolutionDropdown.value = currentIndex;
        
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    void SetupVSync()
    {
        _vSync = QualitySettings.vSyncCount.IntToBool();
        
        if (vSyncToggle == null) return;

        vSyncToggle.isOn = _vSync;
        vSyncToggle.onValueChanged.AddListener(SetVSync);
    }

    void SetupFullScreenMode()
    {
        _fullScreenMode = Screen.fullScreenMode;
        if(fullScreenModeDropdown == null) return;
        
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        
        foreach (var setting in FullScreenModes)
        {
            options.Add(new TMP_Dropdown.OptionData(setting.Label));
        }

        fullScreenModeDropdown.options = options;
        fullScreenModeDropdown.value = Array.FindIndex(FullScreenModes, x => x.Mode == Screen.fullScreenMode);
        fullScreenModeDropdown.onValueChanged.AddListener(SetFullscreen);
    }
    
    private void SaveSettings()
    {
        if (volumeSlider != null)
        {
            foreach (var t in VolumeName)
            {
                mixer.GetFloat(t, out var value);
            
                PlayerPrefs.SetFloat(t, value);
            }
        }
        PlayerPrefs.Save();
    }

    private void SetMasterVolume(float value) => SetVolume(value, 0);
    private void SetMusicVolume(float value) => SetVolume(value, 1);
    private void SetSoundVolume(float value) => SetVolume(value, 2);

    private void SetVSync(bool value) => _vSync = value;
    private void SetFullscreen(int index) => _fullScreenMode = FullScreenModes[index].Mode;
    private void SetResolution(int index) => _resolution = Screen.resolutions[index];

    public void ApplyGraphicsSettings()
    {
        Screen.SetResolution(_resolution.width, _resolution.height, _fullScreenMode);
        QualitySettings.vSyncCount = _vSync.BoolToInt();
    }

    private void SetVolume(float value, int index)
    {
        mixer.SetFloat(VolumeName[index], LinearToVolume(value));
    }

    private static float LinearToVolume(float value)
    {
        return value == 0 ? -80 : Mathf.Log10(value) * 20;
    }
    
    private static float VolumeToLinear(float value)
    {
        return value == -80 ? 0 : Mathf.Pow(10, value / 20);
    }
}
