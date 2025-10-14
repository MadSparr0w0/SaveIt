using UnityEngine;

public class AdvancedCameraController : MonoBehaviour
{
    [Header("Настройки перемещения")]
    public float moveSpeed = 10f;
    public float edgeScrollSpeed = 5f;
    public float edgeBoundary = 50f;
    public bool edgeScrollingEnabled = true;

    [Header("Настройки зума")]
    public float zoomSpeed = 5f;
    public float minZoom = 5f;
    public float maxZoom = 20f;
    public float zoomLerpSpeed = 5f;

    [Header("Границы карты")]
    public Vector2 mapSize = new Vector2(50f, 50f);
    public float padding = 5f;

    [Header("Сглаживание")]
    public bool smoothMovement = true;
    public float smoothTime = 0.1f;

    private Camera cam;
    private Vector3 dragOrigin;
    private bool isDragging = false;
    private float targetZoom;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        cam = GetComponent<Camera>();
        targetZoom = cam.orthographicSize;

        transform.position = new Vector3(mapSize.x / 2f, transform.position.y, mapSize.y / 2f);
    }

    void Update()
    {
        HandleMouseInput();
        HandleKeyboardInput();
        HandleEdgeScrolling();
        HandleZoom();
        ClampCameraPosition();
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = GetMouseWorldPosition();
            isDragging = true;
        }

        if (Input.GetMouseButton(1) && isDragging)
        {
            Vector3 difference = dragOrigin - GetMouseWorldPosition();

            if (smoothMovement)
            {
                transform.position = Vector3.SmoothDamp(transform.position, transform.position + difference, ref velocity, smoothTime);
            }
            else
            {
                transform.position += difference;
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
        }
    }

    void HandleKeyboardInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            Vector3 movement = new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;

            if (smoothMovement)
            {
                transform.position = Vector3.SmoothDamp(transform.position, transform.position + movement, ref velocity, smoothTime);
            }
            else
            {
                transform.position += movement;
            }
        }
    }

    void HandleEdgeScrolling()
    {
        if (!edgeScrollingEnabled || isDragging) return;

        Vector3 movement = Vector3.zero;
        Vector2 mousePosition = Input.mousePosition;

        if (mousePosition.x < edgeBoundary)
            movement.x = -1;
        else if (mousePosition.x > Screen.width - edgeBoundary)
            movement.x = 1;

        if (mousePosition.y < edgeBoundary)
            movement.z = -1;
        else if (mousePosition.y > Screen.height - edgeBoundary)
            movement.z = 1;

        if (movement != Vector3.zero)
        {
            movement = movement.normalized * edgeScrollSpeed * Time.deltaTime;
            transform.position += movement;
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            targetZoom -= scroll * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        if (Mathf.Abs(cam.orthographicSize - targetZoom) > 0.01f)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomLerpSpeed);
        }
        else
        {
            cam.orthographicSize = targetZoom;
        }

        if (cam.orthographicSize >= maxZoom - 0.1f)
        {
            CenterCameraOnMap();
        }
    }

    void ClampCameraPosition()
    {
        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;

        float minX = horzExtent - padding;
        float maxX = mapSize.x - horzExtent + padding;
        float minZ = vertExtent - padding;
        float maxZ = mapSize.y - vertExtent + padding;

        if (maxX <= minX)
        {
            transform.position = new Vector3(mapSize.x / 2f, transform.position.y, transform.position.z);
        }
        else
        {
            float x = Mathf.Clamp(transform.position.x, minX, maxX);
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }

        if (maxZ <= minZ)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, mapSize.y / 2f);
        }
        else
        {
            float z = Mathf.Clamp(transform.position.z, minZ, maxZ);
            transform.position = new Vector3(transform.position.x, transform.position.y, z);
        }
    }

    void CenterCameraOnMap()
    {
        if (smoothMovement)
        {
            Vector3 targetPosition = new Vector3(mapSize.x / 2f, transform.position.y, mapSize.y / 2f);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
        else
        {
            transform.position = new Vector3(mapSize.x / 2f, transform.position.y, mapSize.y / 2f);
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    public void UpdateMapSize(Vector2 newMapSize)
    {
        mapSize = newMapSize;
        ClampCameraPosition();
    }

    public void ForceCenterCamera()
    {
        transform.position = new Vector3(mapSize.x / 2f, transform.position.y, mapSize.y / 2f);
        targetZoom = minZoom;
    }
}