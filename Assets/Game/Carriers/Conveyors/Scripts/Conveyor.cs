using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Conveyor : Carrier
{
    // Config: 
    [SerializeField] public float beltSpeed = 0.01f;

    public Animator myAnimator;
    [SerializeField] Scanner scanner;
    [SerializeField] ConveyorEnd conveyorEnd;

    [SerializeField] private float delayBeforeTransferAttemptsToTable = 1f;
    [SerializeField] private TableCarrier tableCarrier;
    [SerializeField] private Renderer beltRenderer;

    private ITransferBoxes currentTransferPartner = null;
    private bool isBeltMoving = false;
    private bool isConveyorEndFull = false;
    private bool isScanning = false;
    float uvOffset = 0.0f;
    public Transform actionRectZone;
    private float actionZoneScaleMultiplier = 1.1f;
    private Vector3 actionRectScaleUp;
    private Vector3 actionRectOriginalScale;
    private readonly int Belt = Animator.StringToHash("moveBelt");
    private BoxesPile boxesPile;
    [SerializeField] private float delayBeforeTransferFromGiverToPile = 0.1f;
    private Dictionary<ITransferBoxes, Coroutine> transferProcesses = new Dictionary<ITransferBoxes, Coroutine>();
    private PortBox boxOnBelt = null;
    private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");

    private void Start()
    {
        boxesPile = GetComponent<BoxesPile>();
        actionRectOriginalScale = actionRectZone.localScale;
        actionRectScaleUp = actionRectOriginalScale * actionZoneScaleMultiplier;
    }

    private void OnEnable()
    {
        scanner.OnScannerActivated += CancelBeltMovement;
        scanner.OnScannerDeactivated += ActivateBeltMovement;
    }

    private void OnDisable()
    {
        scanner.OnScannerActivated -= CancelBeltMovement;
        scanner.OnScannerDeactivated -= ActivateBeltMovement;
    }

    private void Update()
    {
        LoadBoxToBeltHandler();
        MoveBelt();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) ScaleUpActionZone();

        if (other.TryGetComponent(out ITransferBoxes boxesGiver))
        {
            transferProcesses.Add(boxesGiver, StartCoroutine(TransferBoxesHandler(boxesGiver)));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) ScaleDownActionZone();

        if (other.TryGetComponent(out ITransferBoxes boxesGiver))
        {
            if (transferProcesses.TryGetValue(boxesGiver, out var activeTransferProcess))
            {
                StopCoroutine(activeTransferProcess);
                transferProcesses.Remove(boxesGiver);
            }
        }
    }

    private void ActivateBeltMovement()
    {
        isScanning = false;
        isBeltMoving = true;

        MoveBelt();
    }

    private void LoadBoxToBeltHandler()
    {
        if (isScanning || isBeltMoving || isConveyorEndFull) return;

        boxOnBelt = boxesPile.TakeBoxFromPile();
        boxes[0] = boxOnBelt;

        if (boxOnBelt)
        {
            boxOnBelt.transform.SetParent(CargoPlacesHolder);
            boxOnBelt.transform.localPosition = Vector3.zero;
            ActivateBeltMovement();
        }
    }

    public bool HasBoxInstance(PortBox targetBox)
    {
        return targetBox == boxOnBelt;
    }

    private void MoveBelt()
    {
        if (!boxOnBelt) return;

        Transform box = boxOnBelt.transform;

        Material[] materials = new Material[2];

        if (isBeltMoving)
        {
            if (myAnimator)
            {
                myAnimator.SetTrigger(Belt);
            }

            box.transform.Translate((conveyorEnd.transform.position - box.position) * Time.deltaTime * beltSpeed,
                Space.World);

            uvOffset += (beltSpeed * Time.deltaTime);

            materials = beltRenderer.materials;
            materials[0].SetTextureOffset(BaseMap, new Vector2(0, -uvOffset));

            beltRenderer.materials = materials;
        }
    }

    private void CancelBeltMovement()
    {
        isScanning = true;
        isBeltMoving = false;
    }

    public IEnumerator TransferBoxToTableHandler()
    {
        isBeltMoving = false;
        isConveyorEndFull = true;

        while (!tableCarrier.CheckCanReceiveBoxes())
        {
            yield return new WaitForSeconds(delayBeforeTransferAttemptsToTable);
        }

        tableCarrier.ReceiveBox(boxOnBelt);
        boxOnBelt = null;
        boxes[0] = null;
        isConveyorEndFull = false;

        yield return null;
    }

    private void ScaleUpActionZone()
    {
        actionRectZone.DOScale(actionRectScaleUp, 0.3f).SetEase(Ease.OutQuart);
    }

    private void ScaleDownActionZone()
    {
        actionRectZone.DOScale(actionRectOriginalScale, 0.3f).SetEase(Ease.OutQuart);
    }

    IEnumerator TransferBoxesHandler(ITransferBoxes boxesGiver)
    {
        while (boxesGiver.CheckCanGiveBoxes())
        {
            var portBox = boxesGiver.GiveBox();
            if (portBox)
            {
                boxesPile.AddBoxToPile(portBox);
            }

            yield return new WaitForSeconds(delayBeforeTransferFromGiverToPile);
        }

        yield return null;
    }
}