using UnityEngine;
using AK;
using Unity.VisualScripting;
using JetBrains.Annotations;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one AudioManager in the scene.");
        }

        instance = this;
    }

    public static AudioManager GetInstance()
    {
        return instance;
    }

    public void PlayOneShotAudio(string audioName)
    {
        AkUnitySoundEngine.PostEvent($"{audioName}", gameObject);
    }

    public void SetSwitch(string switchName, string switchValue)
    {
        AkUnitySoundEngine.SetSwitch($"{switchName}", $"{switchValue}", gameObject);
    }

    public void StopAllAudio()
    {
        AkUnitySoundEngine.StopAll(gameObject);
    }
}
