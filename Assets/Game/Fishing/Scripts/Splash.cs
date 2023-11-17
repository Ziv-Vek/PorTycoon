using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splash : MonoBehaviour
{
    public bool Interacteble;
    public ParticleSystem SplashParticals;
    
    void Start()
    {
        Interacteble = true;
        Invoke("SetInteractToFalse", 0.5f);
    }
    void SetInteractToFalse()
    {
        Interacteble = false;
    }
    private void Update()
    {
        if (!SplashParticals.IsAlive())
            Destroy(gameObject);
    }
    public void OnMouseDown()
    {
        if (Interacteble)
        {
            transform.parent.GetComponent<FishingManager>().ObjectFished(transform.position);
            Interacteble = false;
        }
    }
}
