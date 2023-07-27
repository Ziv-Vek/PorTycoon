using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCarrier : Carrier
{
    public Carrier targetCarrier;
    public bool enabled = true;
    public MoneyPile moneyPile;
    [SerializeField] Animator myAnimator;
    public int waitTime = 2; // Might be taken from global state
    public Item CurrentItem { get; set; }

    public override void Awake()
    {
        base.Awake();
        if (targetCarrier)
            StartCoroutine(ProcessBoxesRoutine());
    }

    private IEnumerator ProcessBoxesRoutine()
    {
        while (true)
        {
            if (!targetCarrier.CheckCanGiveBoxes() || !CheckCanReceiveBoxes() || !enabled)
            {
                yield return new WaitForSeconds(1);
                continue;
            }

            // Take the box from the targetCarrier
            myAnimator.SetFloat("forwardSpeed", 100f);
            ReceiveBox(targetCarrier.GiveBox());
            Debug.Log("Box Received from Target Carrier");

            // Wait for the specified amount of seconds
            yield return new WaitForSeconds(waitTime);
            myAnimator.SetFloat("forwardSpeed", 0);

            GiveBox();
            Debug.Log("Box Removed from NPC");
            CurrentItem = GameManager.Instance.CurrentLevel.GetRandomItemForLevel();
            GameManager.Instance.UnlockItem(CurrentItem);
            Debug.Log("Item Unlocked: " + CurrentItem.name);
            Bank.Instance.AddMoneyToPile(moneyPile);
        }
    }
}