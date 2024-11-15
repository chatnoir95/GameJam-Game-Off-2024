using UnityEngine;

public class AudioScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AudioSource SFXSource , MusicSource;

    public AudioClip music, buttonSFX;

    
 

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
