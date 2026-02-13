using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {get; private set;}

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        Instance = this;
    }

    public void PlayOneShot(AudioAssetSO audioAsset, Vector3 position = default)
    {
        if(audioAsset == null)
        {
            Debug.LogWarning("Audio asset null");
            return;
        }

        RuntimeManager.PlayOneShot(audioAsset.fmodEvent);
    }
}
