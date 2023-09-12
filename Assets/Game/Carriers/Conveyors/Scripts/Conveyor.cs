using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Conveyor : Carrier
{
    // Config: 
    [SerializeField] public float beltSpeed = 0.01f;

    [SerializeField] private Animator myAnimator;
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

    private void Start()
    {
        actionRectOriginalScale = actionRectZone.localScale;
        actionRectScaleUp = actionRectOriginalScale * actionZoneScaleMultiplier;
    }
    
    private void OnEnable()
    {
        scanner.OnScannerActivated += CancelBeltMovement;
        scanner.OnScannerDeactivated += MoveBelt;
    }

    private void OnDisable()
    {
        scanner.OnScannerActivated -= CancelBeltMovement;
        scanner.OnScannerDeactivated -= MoveBelt;
    }

    private void Update()
    {
        if (isScanning || isConveyorEndFull || !CheckCanGiveBoxes()) return;
        
        MoveBelt();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) ScaleUpActionZone();
        
        if (currentTransferPartner != null) return;
        
        if (other.TryGetComponent<ITransferBoxes>(out ITransferBoxes giver))
        {
            IsAttemptingToGiveCargo = true;
        
            StartCoroutine(BoxesTransferHandler.Instance.CheckTransfer(this, giver));
        }
        
        if (!isBeltMoving && CheckCanGiveBoxes())
        {
            isBeltMoving = true;
            Move();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) ScaleDownActionZone();
        
        IsAttemptingToGiveCargo = false;
    }

    private void MoveBelt()
    {
        isScanning = false;
        isBeltMoving = true;

        Move();
    }

    public bool HasBoxInstance(PortBox targetBox)
    {
        return Array.Exists(boxes, box => box == targetBox);
    }

    public void ProcessTransferToTable(PortBox box)
    {
        isBeltMoving = false;
        
        StartCoroutine(AttemptToTransferToTable(box));
    }

    private void Move()
    {
        Transform box = boxes[0].transform;
        
        Material[] materials = new Material[2];

        if (isBeltMoving)
        {
            myAnimator.SetTrigger(Belt);
            box.transform.Translate((conveyorEnd.transform.position - box.position) * Time.deltaTime * beltSpeed, Space.World);
            
            uvOffset += (beltSpeed * Time.deltaTime);

            materials = beltRenderer.materials;
            materials[0].SetTextureOffset("_BaseMap", new Vector2(0, -uvOffset));

            beltRenderer.materials = materials;
        }
    }

    private void CancelBeltMovement()
    {
        isScanning = true;
        isBeltMoving = false;
    }
    
    IEnumerator AttemptToTransferToTable(PortBox box)
    {
        isConveyorEndFull = true;
        
        while (!tableCarrier.CheckCanReceiveBoxes())
        {
            yield return new WaitForSeconds(delayBeforeTransferAttemptsToTable);
        }

        GiveBox(box);
        isConveyorEndFull = false;
        
        yield return null;
    }

    private void GiveBox(PortBox targetBox)
    {
        int index = Array.FindIndex(boxes, box => targetBox == box);
        tableCarrier.ReceiveBox(targetBox);
        boxes[index] = null;

        if (CheckCanGiveBoxes())
        {
            MoveBelt();
        }
    }
    
    private void ScaleUpActionZone()
    {
        actionRectZone.DOScale(actionRectScaleUp, 0.3f).SetEase(Ease.OutQuart);

    }
    
    private void ScaleDownActionZone()
    {
        actionRectZone.DOScale(actionRectOriginalScale, 0.3f).SetEase(Ease.OutQuart);

    }
}
