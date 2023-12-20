using UnityEngine;

public class EnterFishing : MonoBehaviour
{
    [SerializeField] FishingManager FishingGame;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartFishing(other.GetComponent<PlayerMover>());
        }
    }

    private void StartFishing(PlayerMover playerMover)
    {
        FishingGame.gameObject.SetActive(true);
        FishingGame.StartInvoke();
        playerMover.ToggleMovement(false);
        playerMover.HideJoystick();
        AudioManager.Instance.ChangeSounds("General Music", "Fishing Music");
    }
}