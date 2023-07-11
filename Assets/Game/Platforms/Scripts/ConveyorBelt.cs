using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] private float beltSpeed = 0.01f;
    [SerializeField] private Material beltMaterial;
    [SerializeField] Renderer conveyorRenderer;

    public Reader reader;
    public ConveyorEnd conveyorEnd;

    private ICallTransferEvents platform;

    private bool isBeltMoving = false;
    float uvOffset = 0.0f;

    private void Awake()
    {
        platform = GetComponentInChildren<ICallTransferEvents>();
    }

    private void OnEnable()
    {
        platform.OnSingleTransferComplete += MoveBelt;
        reader.OnReaderFull += CancelMoveBelt;
        reader.OnReaderEmpty += MoveBelt;
        conveyorEnd.OnConveyorEndEmpty += CancelMoveBelt;
    }

    private void OnDisable()
    {
        platform.OnSingleTransferComplete -= MoveBelt;
        reader.OnReaderFull -= CancelMoveBelt;
        reader.OnReaderEmpty -= MoveBelt;
        conveyorEnd.OnConveyorEndEmpty -= CancelMoveBelt;
    }

    private void MoveBelt(GameObject box)
    {
        isBeltMoving = true;

        StartCoroutine(Move(box));
    }

    IEnumerator Move(GameObject box)
    {
        Material[] materials = new Material[2];

        while (isBeltMoving)
        {
            box.transform.Translate((conveyorEnd.transform.position - box.transform.position) * Time.deltaTime * beltSpeed);

            uvOffset += (beltSpeed * Time.deltaTime);

            materials = conveyorRenderer.materials;
            materials[0].SetTextureOffset("_BaseMap", new Vector2(0, uvOffset));

            conveyorRenderer.materials = materials;

            yield return null;
        }
    }

    private void CancelMoveBelt()
    {
        Debug.Log("called cancel movement");
        isBeltMoving = false;
    }
}
