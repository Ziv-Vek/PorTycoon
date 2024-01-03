using UnityEngine;

public class EnterFishing : MonoBehaviour
{
    public FishingMenu menu;
    public GameObject ArrowPrefab;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && GameManager.Instance.GoneThroughTutorial)
        {
            PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
            playerMover.ToggleMovement(false);
            playerMover.HideJoystick();
            menu.gameObject.SetActive(true);
            menu.gameObject.transform.Find("UI Holder").GetComponent<Animator>().Play("Open UI", 0);
            GameManager.Instance.ThereUIActive = true;
        }
    }
    public void GoToFish()
    {
        Instantiate(ArrowPrefab).GetComponent<ArrowNavigation>().Target = gameObject.transform;
    }
}