using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class Conveyor : Carrier
{
    [SerializeField] private float beltSpeed = 0.01f;
    [SerializeField] Renderer conveyorRenderer;

    [SerializeField] Scanner scanner;
    [SerializeField] ConveyorEnd conveyorEnd;
    
    [SerializeField] private float delayBeforeTransferAttemptsToTable = 1f;
    [SerializeField] private TableCarrier tableCarrier;
    
    private ITransferBoxes currentTransferPartner = null;
    private bool isBeltMoving = false;
    private bool isConveyorEndFull = false;
    private bool isScanning = false;
    float uvOffset = 0.0f;

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
        IsAttemptingToGiveCargo = false;
    }

    private void MoveBelt()
    {
        isScanning = false;
        isBeltMoving = true;

        Move();
    }

    public bool HasBoxInstance(GameObject targetBox)
    {
        return Array.Exists(boxes, box => box == targetBox);
    }

    public void ProcessTransferToTable(GameObject box)
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
            box.transform.Translate((conveyorEnd.transform.position - box.position) * Time.deltaTime * beltSpeed, Space.World);
            
            uvOffset += (beltSpeed * Time.deltaTime);

            materials = conveyorRenderer.materials;
            materials[0].SetTextureOffset("_BaseMap", new Vector2(0, uvOffset));

            conveyorRenderer.materials = materials;
        }
    }

    private void CancelBeltMovement()
    {
        isScanning = true;
        isBeltMoving = false;
    }
    
    IEnumerator AttemptToTransferToTable(GameObject box)
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

    private void GiveBox(GameObject targetBox)
    {
        int index = Array.FindIndex(boxes, box => targetBox == box);
        tableCarrier.ReceiveBox(targetBox);
        boxes[index] = null;

        if (CheckCanGiveBoxes())
        {
            MoveBelt();
        }
    }
}
