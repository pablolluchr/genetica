using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BottomButton : MonoBehaviour
{
    public GameObject speciesSelectionPanel;
    public BottomButtonType bottomButtonType;
    public bool selected;
    public Color selectedColor;
    public Color normalColor;

    // Start is called before the first frame update
    void Start()
    {
        selected = false;
        GetComponent<Button>().onClick.AddListener(ToggleSpeciesSelectionPanel);
    }


    void ToggleSpeciesSelectionPanel()
    {
        if (selected)//UNSELECT
        {
            //hide speciesSelectionPanel
            if (bottomButtonType == BottomButtonType.Unit || bottomButtonType == BottomButtonType.Enemy)
                speciesSelectionPanel.SetActive(false);

            //update selection graphic
            GetComponent<Image>().color = normalColor;
        }

        else if (!selected) //SELECT
        { 
            //unselect other buttons
            BottomButton[] bottomControlButtons = transform.parent.GetComponentsInChildren<BottomButton>();
            foreach (BottomButton button in bottomControlButtons)
            {
                button.GetComponent<Image>().color = normalColor;
                button.selected = false;
            }
            
            if (bottomButtonType == BottomButtonType.Unit)
            {
                //show speciesSelectionPanel of pets
                speciesSelectionPanel.GetComponent<SpeciesSelectionPanel>().unitType = "Pet";
                speciesSelectionPanel.GetComponent<SpeciesSelectionPanel>().ShowSpecies();
                speciesSelectionPanel.SetActive(true);
            }
           
            else if (bottomButtonType == BottomButtonType.Enemy)
            {
                //show speciesSelectionPanel of enemies
                speciesSelectionPanel.GetComponent<SpeciesSelectionPanel>().unitType = "Hostile";
                speciesSelectionPanel.GetComponent<SpeciesSelectionPanel>().ShowSpecies();
                speciesSelectionPanel.SetActive(true);
            }
            
            else if (bottomButtonType == BottomButtonType.Settings)
            {
                //show settings panel (todo) and hide speciesSelectionPanel 
                speciesSelectionPanel.SetActive(false);
            }

            else if (bottomButtonType == BottomButtonType.QuickActions)
            {
                //show quickActions panel (todo) and hide speciesSelectionPanel 

                speciesSelectionPanel.SetActive(false);
            }

            //update selection graphic
            GetComponent<Image>().color = selectedColor;
        }

        //toggle selection
        selected = !selected;
    }
}


public enum BottomButtonType
{
    Unit,
    Enemy,
    QuickActions,
    Settings
}
