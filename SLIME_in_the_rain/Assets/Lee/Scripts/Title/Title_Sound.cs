using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class Title_Sound : MonoBehaviour
{
    public AudioMixer masterMixer;
    public Slider masterAudioSlider;
    public Slider bgmAudioSlider;
    public Slider sfxAudioSlider;

    public Toggle mastertoggle;
    
    private bool state = true;

    public void ToggleAudioVolume()     //사운드 토글
    {
        AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
    }
    public void MasterAudioControl()
    {
        float masterSound = masterAudioSlider.value;
        if (masterSound == -40f) masterMixer.SetFloat("Master", -80);
        else masterMixer.SetFloat("Master", masterSound);
        getValumInt(masterAudioSlider ,masterSound);
    }

    public void getValumInt(Slider slider,float sound)
    {
        slider.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text
            = ((int)((sound + 40) / 40 * 100 )).ToString();
    }

    public void soundOnOff()
    {
        if (state == true) state = false;
        else state = true;
        mastertoggle.transform.GetChild(0).gameObject.SetActive(state);
        mastertoggle.transform.GetChild(1).gameObject.SetActive(!state);

        if (state == true)
        {
            this.GetComponent<AudioSource>().mute = false;

        }
        else
        {
            this.GetComponent<AudioSource>().mute = true;
        }

    }

}
