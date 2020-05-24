using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class InputManager : MonoBehaviour {

    Vector2 lastTouch;
    public Vector2 touchSpeed;
    public float touchDraggingThresholdSpeed = 0.1f;
    public float touchSpeedCorrection = 0.8f;
    private GameManager gm;
    public bool blockedByUI;
    // Update is called once per frame
    private void Start()
    {
         gm = GameManager.gameManager;

    }
    void Update() {
        if (IsPointerOverUIObject()) return;

        UpdateTouchSpeed();
        gm.isDragging = isDragging();
        gm.isShortClick = isShortClick();
        if (gm.isShortClick) {
            print("short click");
            if (!gm.IsAreaSelected())
                gm.selectedObject = objectHitWithRaycast();
            gm.selectedPoint = pointHitWithRaycast();
        }
        saveFrameInfo();
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    private void UpdateTouchSpeed()
    {
        if (Input.touchCount == 1)
        {
            Vector2 newTouchPosition = Input.GetTouch(0).position;
            if (lastTouch == Vector2.zero) touchSpeed = Vector2.zero;
            else touchSpeed = (newTouchPosition - lastTouch) / Screen.width / Time.deltaTime*touchSpeedCorrection;
            lastTouch = newTouchPosition;

        }
        else
        {
            touchSpeed = new Vector2(0f, 0f);
            lastTouch = Vector2.zero;
        }
    }

    private Vector3 pointHitWithRaycast() {
        RaycastHit hitInfo = new RaycastHit();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hit = Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("Planet"));
        if (!hit) return Vector3.zero;
        return hitInfo.point;
    }

    private GameObject objectHitWithRaycast() {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity);
        if (!hit) return null;
        return hitInfo.transform.gameObject;
    }

    private bool isShortClick() {
        GameManager gm = GameManager.gameManager;
        if (Input.touchCount == 1) {
            return Input.GetTouch(0).phase == TouchPhase.Ended && !gm.isDragging;
        } else {
            return Input.GetMouseButtonUp(0) && !gm.isDragging;
        }
    }

    private bool isDragging() {
        GameManager gm = GameManager.gameManager;
        if (Input.touchCount == 1) {;
            return touchSpeed.magnitude > touchDraggingThresholdSpeed || gm.isDragging;
            //if (Input.GetTouch(0).phase == TouchPhase.Ended) return false;
        }
        if (Input.GetMouseButton(0)) {
            if (gm.wasButtonDown) {
                float mouseMoveX = Mathf.Pow(Input.GetAxis("Mouse X") - gm.lastMouseX, 2);
                float mouseMoveY = Mathf.Pow(Input.GetAxis("Mouse Y") - gm.lastMouseY, 2);
                return mouseMoveX + mouseMoveY > 0.005f || gm.isDragging;
            }
        }
        return false;
    }

    private void saveFrameInfo() {
        GameManager gm = GameManager.gameManager;
        if (Input.GetMouseButton(0)) {
            gm.lastMouseX = Input.GetAxis("Mouse X");
            gm.lastMouseY = Input.GetAxis("Mouse Y");
            gm.wasButtonDown = true;
        } else {
            gm.wasButtonDown = false;
        }
        gm.wasDraggingInPrevFrame = gm.isDragging;
    }
}