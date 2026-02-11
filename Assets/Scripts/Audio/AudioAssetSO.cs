using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "NewAudioAsset", menuName = "Audio/AudioAsset")]
public class AudioAssetSO : ScriptableObject
{
    public EventReference fmodEvent;
}
