using UnityEngine;

public class PanelTouchHandler : MonoBehaviour
{
    public GameObject coin; // Drag and drop the coin GameObject here from your scene
    public Camera coinCamera; // Drag and drop the coin-specific camera here
    public float spawnDistance = 50.0f; // How far from the camera you want to show the coin
    public float offset = -0.4f; // How far from the touch position you want to show the coin

    [SerializeField] private float time;
    [SerializeField] private SpriteRenderer Hand;

    private MeshRenderer coinMeshRenderer;

    private void Start()
    {
        coinMeshRenderer = coin.GetComponent<MeshRenderer>();
    }
    private void OnEnable()
    {
        time = 4;
    }
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved ||
                touch.phase == TouchPhase.Stationary)
            {
                PositionCoinAtTouch(touch.position);
                coin.SetActive(true);
                Debug.Log("Draged");
                time = 0;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            PositionCoinAtTouch(Input.mousePosition);
            coin.SetActive(true);
        }
        else
        {
            coin.SetActive(false);
        }

        time += 1 * Time.deltaTime;
      //  if (time > 4)
          //  Hand.gameObject.SetActive(true);
    }

    void PositionCoinAtTouch(Vector2 touchPosition)
    {
        Ray ray = coinCamera.ScreenPointToRay(touchPosition);
        Vector3 coinPosition = ray.GetPoint(spawnDistance);

        if (coinMeshRenderer)
        {
            Vector3 extents = coinMeshRenderer.bounds.extents;

            coinPosition.x += extents.x + offset; // Add half the width (extents is half of the bounds size)
            coinPosition.y -= extents.y + offset; // Subtract half the height
        }

        coin.transform.position = coinPosition;
    }
    private void OnMouseDrag()
    {
   //     Debug.Log("Draged");
    }
}