using System;
using DG.Tweening;
using UnityEngine;

public class Pier : Carrier
{
    private ITransferBoxes currentTransferPartner = null;

    public event Action<CarriersTypes> onBoxDrop;
    public Transform actionRectZone;
    private float actionZoneScaleMultiplier = 1.1f;
    private Vector3 actionRectScaleUp;
    private Vector3 actionRectOriginalScale;
    [SerializeField] Animator ActionRectHalo;

    [SerializeField] MoneyPile moneyPile;

    private void Start()
    {
        actionRectOriginalScale = actionRectZone.localScale;
        actionRectScaleUp = actionRectOriginalScale * actionZoneScaleMultiplier;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentTransferPartner != null) return;

        if (other.CompareTag("Player")) ScaleUpActionZone();

        if (other.TryGetComponent(out currentTransferPartner))
        {
            IsAttemptingToGiveCargo = true;

            StartCoroutine(BoxesTransferHandler.Instance.CheckTransfer(this, currentTransferPartner));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) ScaleDownActionZone();

        if (!other.TryGetComponent(out ITransferBoxes otherTransferer)
            && (otherTransferer != currentTransferPartner)) return;

        IsAttemptingToGiveCargo = false;
        currentTransferPartner = null;
    }

    private void ScaleUpActionZone()
    {
        actionRectZone.DOScale(actionRectScaleUp, 0.3f).SetEase(Ease.OutQuart);
    }

    private void ScaleDownActionZone()
    {
        actionRectZone.DOScale(actionRectOriginalScale, 0.3f).SetEase(Ease.OutQuart);
    }

    public override void ReceiveBox(PortBox cargo)
    {
        int index = Array.FindIndex(boxes, i => i == null);
        boxes[index] = cargo;
        cargo.transform.SetParent(boxesPlaces[index]);
        cargo.transform.localPosition = Vector3.zero;
        cargo.transform.localRotation = gameObject.transform.rotation;
        Bank.Instance.AddMoneyToPile(moneyPile, "Cargo");
        onBoxDrop?.Invoke(CarriersTypes.Pier);
        ActionRectHalo.Play("ActionRectBoxIn", 0);
    }
    public override PortBox GiveBox()
    {
        int index = Array.FindLastIndex(boxes, box => box != null);

        PortBox box = boxes[index];
        boxes[index] = null;
        if(!CheckCanGiveBoxes())
        {
            ActionRectHalo.Play("Default", 0);
        }
        return box;
    }
}