using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeciesButton : MonoBehaviour
{
    public bool selected;
    public Color selectedColor;
    public Color normalColor;
    public string speciesName;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(SelectSpecies);
    }

    public void SelectSpecies()
    {
        if (selected) {
            GameManager.gameManager.selectedSpecies = null;
            GameManager.gameManager.gameState = GameState.Panning;
            GameManager.gameManager.cameraController.StartPanning();
            selected = false;
            GetComponent<Image>().color = normalColor;
            return;
        }
        
        GameManager.gameManager.selectedSpecies = speciesName;
        GameManager.gameManager.gameState = GameState.MovingToArea;
        GameManager.gameManager.cameraController.ResetMovingToSpeed();
        
        SpeciesButton[] speciesButtons = GetComponentInParent<SpeciesMenu>().GetComponentsInChildren<SpeciesButton>();
        Debug.Log(speciesButtons.Length);
        foreach (SpeciesButton speciesButton in speciesButtons) {
            speciesButton.GetComponent<Image>().color = normalColor;
            speciesButton.selected = false;
        }
        GetComponent<Image>().color = selectedColor;
        selected = true;
    }


}
