using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager I;

    public AudioSource source;

    public AudioClip flip;
    public AudioClip match;
    public AudioClip mismatch;
    public AudioClip gameover;

    void Awake()
    {
        I = this;
    }

    public void PlayFlip() => source.PlayOneShot(flip);
    public void PlayMatch() => source.PlayOneShot(match);
    public void PlayMismatch() => source.PlayOneShot(mismatch);
    public void PlayGameOver() => source.PlayOneShot(gameover);
}
