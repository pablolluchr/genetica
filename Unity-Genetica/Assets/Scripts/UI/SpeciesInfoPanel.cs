using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeciesInfoPanel : MovingUIPanel
{
    public Species species;
    private void Start()
    {
        Hide();
    }
    public void Show(Species species)
    {
        this.species = species;
        Show();
    }

    new public void Hide()
    {
        GameManager.gameManager.selectedSpecies = null;
        base.Hide();

    }
}
