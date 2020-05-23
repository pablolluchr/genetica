using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public InputManager inputManager;
    public void OnPointerEnter()
    {
        inputManager.blockedByUI = true;
        print("entered UI");
    }

    public void OnPointerExit()
    {
        inputManager.blockedByUI = false;
        print("exited UI");


    }
}
