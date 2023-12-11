using UnityEngine;

public class FootSteps : MonoBehaviour
{
    [SerializeField] AudioClip[] footsteps;
    AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlaySoundRandom()
    {
        source.clip = footsteps[new System.Random().Next(footsteps.Length)];
        source.Play();
    }
}