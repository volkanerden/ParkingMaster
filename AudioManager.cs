using HolagoGames;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource crashSFX;
    [SerializeField] AudioSource engineSFX;
    [SerializeField] AudioSource successSFX;
    [SerializeField] AudioSource coinsSFX;

    private void OnEnable()
    {
        Holago.SystemContainer.EventSystem.PlayCrashSFX.Register(PlayCrashSFX);
        Holago.SystemContainer.EventSystem.PlayEngineSFX.Register(PlayEngineSFX);
        Holago.SystemContainer.EventSystem.PlaySuccessSFX.Register(PlaySuccessSFX);
        Holago.SystemContainer.EventSystem.PlayCoinsSFX.Register(PlayCoinsSFX);
    }
    private void OnDisable()
    {
        Holago.SystemContainer.EventSystem.PlayCrashSFX.UnRegister(PlayCrashSFX);
        Holago.SystemContainer.EventSystem.PlayEngineSFX.UnRegister(PlayEngineSFX);
        Holago.SystemContainer.EventSystem.PlaySuccessSFX.UnRegister(PlaySuccessSFX);
        Holago.SystemContainer.EventSystem.PlayCoinsSFX.UnRegister(PlayCoinsSFX);
    }
    private void PlayCrashSFX()
    {
        if (Holago.SystemContainer.UISystem.IsSoundOn)
        {
            crashSFX.Play();
        }
    }
    private void PlayEngineSFX()
    {
        if (Holago.SystemContainer.UISystem.IsSoundOn)
        {
            engineSFX.Play();
        }
    }
    private void PlaySuccessSFX()
    {
        if (Holago.SystemContainer.UISystem.IsSoundOn)
        {
            successSFX.Play();
        }
    }

    private void PlayCoinsSFX()
    {
        if (Holago.SystemContainer.UISystem.IsSoundOn)
        {
            coinsSFX.Play();
        }
    }
}
