using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeciesButton : MonoBehaviour
{
    public string speciesName;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(SelectSpecies);
    }

    public void SelectSpecies()
    {
        GameManager.gameManager.selectedSpecies = speciesName;
        GameManager.gameManager.gameState = GameState.MovingToArea;
    }


}
