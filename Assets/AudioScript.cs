using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AudioSource SFXSource , MusicSource;

    public AudioClip music, buttonSFX;

    [SerializeField] Slider VolumeSlider;
    [SerializeField] AudioMixer audioMixer;
 

    public static AudioScript instance;
    private void Awake()
    {
        if (instance != null)
        { Debug.LogWarning("careful more than one instance of AudioScript is present"); return; }
        instance = this;
    }

    void Start()
    {
       // launch music at the start of the game
        LaunchMusic(music);


        if (PlayerPrefs.HasKey("masterVolume"))
        {
            LoadVolume(); // if volume as been already change, we keep the change 
        }
        else
        {
            SetMasterVolume();
        }

       // SetMasterVolume(); // set up song on slider

    }
   
    public void SetMasterVolume() // link scrolling bar of menu with audio Mixer 
    {
        float volume = VolumeSlider.value;
        audioMixer.SetFloat("MasterVolume",Mathf.Log10(  volume)*20); // la music est gérer de manier exponentiel, 

        PlayerPrefs.SetFloat("masterVolume", volume); // keep the float value of volume, for use it if the game is restart 
    }

    private void LoadVolume()
    {
        VolumeSlider.value = PlayerPrefs.GetFloat("masterVolume");
        SetMasterVolume();
    }
    
    // call this methode with a SFX for play it 
    public void LaunchSoundSFX(AudioClip audio)
    {
        SFXSource.clip = audio;
        SFXSource.Play();
    }

    public void LaunchMusic(AudioClip audio)
    {
        MusicSource.clip = audio;
        MusicSource.Play();
    }

}
