using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Sprite MusicOn, MusicOff, SoundOn, SoundOff, VibrationOn, VibrationOff, Terms;
    [SerializeField] private GameObject MusicButton, SoundButton, VibrationButton, TermsButton;
    public AudioMixer audioMixer;
    private void OnEnable()
    {
        if(GameManager.Instance.Music)
            MusicButton.GetComponent<Image>().sprite = MusicOn;
        else
            MusicButton.GetComponent<Image>().sprite = MusicOff;  

        if(GameManager.Instance.Sound)
            SoundButton.GetComponent<Image>().sprite = SoundOn;
        else
            SoundButton.GetComponent<Image>().sprite = SoundOff;

        if(GameManager.Instance.Vibration)
            VibrationButton.GetComponent<Image>().sprite = VibrationOn;
        else
            VibrationButton.GetComponent<Image>().sprite = VibrationOff;

    }

    public void setMusic()
    {
        GameManager.Instance.Music = !GameManager.Instance.Music;
        if(GameManager.Instance.Music)
        {
            audioMixer.SetFloat("MusicVolume", 0);
            MusicButton.GetComponent<Image>().sprite = MusicOn;
        }
        else
        {
            audioMixer.SetFloat("MusicVolume", -80);
            MusicButton.GetComponent<Image>().sprite = MusicOff;
        }
    }   
    public void setSfx()
    {
        GameManager.Instance.Sound = !GameManager.Instance.Sound;
        if(GameManager.Instance.Sound)
        {
            audioMixer.SetFloat("SfxVolume", 0);
            SoundButton.GetComponent<Image>().sprite = SoundOn;
        }
        else
        {
            audioMixer.SetFloat("SfxVolume", -80);
            SoundButton.GetComponent<Image>().sprite = SoundOff;
        }
    }
    public void setVibration()
    {
        GameManager.Instance.Vibration = !GameManager.Instance.Vibration;
        if(GameManager.Instance.Vibration)
        {
            VibrationButton.GetComponent<Image>().sprite = VibrationOn;
            VibrationManager.Instance.DefaultVibrate();
        }
        else
        {
            VibrationButton.GetComponent<Image>().sprite = VibrationOff;
        }
    }
    public void SeeTerms()
    {
        //see terms & conditions
    }
    public void RunCloseAnimation()
    {
        transform.Find("UI Holder").GetComponent<Animator>().Play("Close UI", 0);
        VibrationManager.Instance.LightVibrate();
        AudioManager.Instance.Play("Close UI Window");
    }
    public void Exit()
    {
        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        try
        {
            playerMover = GameObject.Find("Player_New").GetComponent<PlayerMover>();
            VibrationManager.Instance.LightVibrate();
        }
        catch
        {
        }
        playerMover.ToggleMovement(true);
        playerMover.ShowJoystick();
        playerMover.joystick.DeactivateJoystick();
        GameManager.Instance.ThereUIActive = false;

        gameObject.SetActive(false);
    }
}
