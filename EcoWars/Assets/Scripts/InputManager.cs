using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    // Update is called once per frame
    void Update() {
        GameManager gm = GameManager.gameManager;
        gm.isDragging = isDragging();
        gm.isShortClick = isShortClick();
        if (gm.isShortClick) {
            gm.selectedObject = objectHitWithRaycast();
            gm.selectedPoint = pointHitWithRaycast();
        }
        saveFrameInfo();
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
            return Input.GetTouch(0).phase == TouchPhase.Ended && !gm.wasDraggingInPrevFrame;
        } else {
            return Input.GetMouseButtonUp(0) && !gm.isDragging;
        }
    }

    private bool isDragging() {
        GameManager gm = GameManager.gameManager;
        if (Input.touchCount == 1) {
            if (Input.GetTouch(0).phase == TouchPhase.Moved) return true;
            if (Input.GetTouch(0).phase == TouchPhase.Ended) return false;
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