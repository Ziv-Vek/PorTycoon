using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splash : MonoBehaviour
{
    public bool Interacteble;
    public ParticleSystem SplashParticals;
    public float opportunity;
    FishingManager fishingManager;
    bool Catched; 


    void Start()
    {
        Interacteble = true;
        Catched = false;
        Invoke("SetInteractToFalse", opportunity);
        fishingManager = transform.parent.GetComponent<FishingManager>();
    }
    void SetInteractToFalse()
    {
        Interacteble = false;
    }
    private void Update()
    {
        if (!SplashParticals.IsAlive())
        {
            if (fishingManager.Step < 7)
                fishingManager.Invoke(nameof(fishingManager.MakeSplash), fishingManager.Delay);
            else if(!Catched)
                fishingManager.Invoke(nameof(fishingManager.BackToPort), fishingManager.Delay);
            Destroy(gameObject);
        }
    }
    public void OnMouseDown()
    {
        if (Interacteble)
        {
            transform.parent.GetComponent<FishingManager>().ObjectFished(transform.position);
            Interacteble = false;
            Catched = true;
        }
    }
}
