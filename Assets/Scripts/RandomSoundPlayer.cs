using UnityEngine;
using UnityEngine.Audio;

public class RandomSoundPlayer : MonoBehaviour
{
    [Header("Sound effects")]
    public AudioClip[] SoundEffect;
    public AudioSource AudioSource;

    [Header("Sound settings")]
    public int chanceOfPlaying;




    private void FixedUpdate()
    {
        if (Random.Range(0,100) < chanceOfPlaying && !AudioSource.isPlaying) 
        {
            int audioClip = Random.Range(0, SoundEffect.Length);
            AudioSource.generator = SoundEffect[audioClip];
            AudioSource.Play();
        }
    }
}
