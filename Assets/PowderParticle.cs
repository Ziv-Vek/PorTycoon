using UnityEngine;
using UnityEngine.EventSystems;

public class PowderParticle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler,
    IPointerExitHandler, IDragHandler
{
    public ParticleSystem touchParticlesPrefab;

    private bool isDragging = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        SpawnParticles(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging)
            SpawnParticles(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isDragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        SpawnParticles(eventData);
    }

    private void SpawnParticles(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                (RectTransform)transform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 worldPoint))
        {
            // Adjust the Z position to ensure the particle is between the Canvas and its camera
            worldPoint.z = transform.position.z - 1;

            // Instantiate the particle system
            ParticleSystem instance = Instantiate(touchParticlesPrefab, worldPoint, Quaternion.identity);

            // Set the layer
            instance.gameObject.layer = LayerMask.NameToLayer("ScratchPointer");

            // Destroy the particle system once it's done playing
            Destroy(instance.gameObject, instance.main.duration + instance.main.startLifetime.constantMax);
        }
    }
}