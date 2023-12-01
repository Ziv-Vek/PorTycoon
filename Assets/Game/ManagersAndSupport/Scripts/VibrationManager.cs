using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CandyCoded.HapticFeedback;

public class VibrationManager : MonoBehaviour
{
    public static VibrationManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void LightVibrate()
    {
        if(GameManager.Instance.Vibration)
             HapticFeedback.MediumFeedback();
    }
    public void MediumeVivrate()
    {
        if (GameManager.Instance.Vibration)
            HapticFeedback.LightFeedback();
    }
    public void HeavyVibrate()
    {
        if (GameManager.Instance.Vibration)
            HapticFeedback.HeavyFeedback();
    }
    public void DefaultVibrate()
    {
        if (GameManager.Instance.Vibration)
            Handheld.Vibrate();
    }
}
