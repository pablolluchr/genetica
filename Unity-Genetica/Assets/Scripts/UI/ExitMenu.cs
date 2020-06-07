using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ExitMenu : MonoBehaviour
{
    //TODO: make the exit button inside the bottom panel instead
    private GameObject panelToClose; //panel needs to have an open/close animator
    // Start is called before the first frame update
    void Start()
    {
        panelToClose = transform.parent.gameObject;
        GetComponent<Button>().onClick.AddListener(Exit);
    }

    public void Exit()
    {
        panelToClose.SendMessage("Hide");
    }

    
}
