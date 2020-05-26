using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ExitMenu : MonoBehaviour
{
    //TODO: make the exit button inside the bottom panel instead
    public UnitInfo unitInfoPanel;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ForceExitMenu);
    }

    public void ForceExitMenu()
    {

        GameManager.gameManager.forceUnitSelectionExit = true;
        unitInfoPanel.Hide();
    }

    
}
