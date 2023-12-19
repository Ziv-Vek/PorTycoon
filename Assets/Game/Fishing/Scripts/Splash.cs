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


    public void Initialize(FishingManager fishingManager)
    {
        this.fishingManager = fishingManager;
        Interacteble = true;
        Catched = false;
        opportunity = fishingManager.OpportunityTime;
        Invoke(nameof(SetInteractToFalse), opportunity);
        fishingManager = transform.parent.GetComponent<FishingManager>();
    }

    void SetInteractToFalse()
    {
        Interacteble = false;
    }

    private void Update()
    {
        if (!Interacteble)
        {
            if (fishingManager.Step < 7)
                fishingManager.Invoke(nameof(fishingManager.MakeSplash), fishingManager.Delay);
            else if (!Catched)
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