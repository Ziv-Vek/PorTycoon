using UnityEngine;

public class EnterFishing : MonoBehaviour
{
    [SerializeField] FishingManager FishingGame;

    private void OnTriggerEnter(Collider other)
    {
        FishingGame.gameObject.SetActive(true);
        FishingGame.StartInvoke();
        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        playerMover.ToggleMovement(false);
        playerMover.HideJoystick();
        AudioManager.Instance.ChangeSounds("General Music", "Fishing Music");
    }
}