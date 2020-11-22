using UnityEngine;

[System.Serializable]
public struct SoundFXDefinition
{
    public SoundEffect effect;
    public AudioClip clip;
}

[System.Serializable]
public enum SoundEffect
{
    CooldownFinish,
    GrappleCollision,
    GrappleFire,
    GrappleReturn,
    Spooling
}
