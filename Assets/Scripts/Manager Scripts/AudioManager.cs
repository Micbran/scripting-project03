using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : Manager<AudioManager>
{
    public List<SoundFXDefinition> SoundFX;
    private AudioSource audioSource;

    public override void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySoundEffect(SoundEffect soundEffect)
    {
        AudioClip effect = SoundFX.Find(sfx => sfx.effect == soundEffect).clip;
        audioSource.PlayOneShot(effect);
    }

    public void StartLoopedSoundEffect(SoundEffect soundEffect)
    {
        StartCoroutine(PlayLoopedEffect(SoundFX.Find(sfx => sfx.effect == soundEffect).clip));
    }

    public void StopAllLoopedSoundEffects()
    {
        StopAllCoroutines();
    }

    private IEnumerator PlayLoopedEffect(AudioClip clipToLoop)
    {
        while(true)
        {
            audioSource.PlayOneShot(clipToLoop);
            yield return new WaitForSeconds(clipToLoop.length);
        }
    }
}
