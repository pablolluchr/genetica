using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GMState {
    UnitSelection,
    ObjectSelection,
    SpeciesSelection,
    FreeSelection,
    SpeciesAttributes, //todo
}

public static class GameManagerStateMachine {

    public static GMState NextState() {
        GameManager gm = GameManager.gameManager;

        switch (gm.gameState) {
            case GMState.FreeSelection: {
                    if (gm.IsObjectSelected()) {
                        gm.TargetObject(); gm.HideSpeciesSelectionPanel();
                        return GMState.ObjectSelection;
                    }
                    if (gm.IsSpeciesSelected()) {
                        gm.SelectSpecies(); gm.HideSpeciesSelectionPanel();
                        return GMState.SpeciesSelection;
                    }
                    if (gm.IsUnitSelected()) {
                        gm.SelectUnit(); gm.HideSpeciesSelectionPanel();
                        return GMState.UnitSelection;
                    }
                    if (gm.selectedSpecies != gm.previousSelectedSpecies) {
                        gm.SelectSpecies();
                        gm.HideSpeciesSelectionPanel(); return GMState.SpeciesSelection;
                    }
                    break;
                }

            case GMState.ObjectSelection: {
                    if (gm.forcePanelExit) {
                        gm.FreePan(); gm.HideObjectSelection();
                        gm.ShowSpeciesSelectionPanel(); return GMState.FreeSelection;
                    }
                    if (gm.isDragging) { gm.FreePan(); gm.HideObjectSelection(); gm.ShowSpeciesSelectionPanel();
                        return GMState.FreeSelection; }
                    if (gm.IsObjectSelected()) { gm.TargetObject(); return GMState.ObjectSelection; }
                    if (gm.IsUnitSelected()) { gm.SelectUnit(); gm.HideObjectSelection(); return GMState.UnitSelection; }
                    break;
                }

            case GMState.UnitSelection: {
                    if (gm.forcePanelExit) { gm.FreePan(); gm.DeselectUnit(); gm.ShowSpeciesSelectionPanel(); return GMState.FreeSelection; }
                    if (gm.isDragging) { gm.FreePan(); return GMState.UnitSelection; }
                    if (gm.IsObjectSelected()) { gm.DeselectUnit(); gm.TargetObject(); return GMState.ObjectSelection; }
                    if (gm.NewUnitSelected()) { UnitActions.DisableAllSelectionGraphics(); gm.SelectUnit(); return GMState.UnitSelection; }
                    if (gm.openSpeciesAttributes) {
                        gm.ShowSpeciesAttributes(); gm.DeselectUnit();
                        gm.FreePan(); return GMState.SpeciesAttributes;
                    }
                    break;
                }

            case GMState.SpeciesSelection: {
                    if (gm.isDragging) { gm.FreePan(); return GMState.SpeciesSelection; }
                    if (!gm.IsSpeciesSelected()) {
                        gm.DeselectSpecies(); gm.ShowSpeciesSelectionPanel();
                        return GMState.FreeSelection;
                    }
                    if (gm.IsObjectSelected()) { gm.SetHabitatTargets(); return GMState.SpeciesSelection; }
                    if (gm.isShortClick) { gm.OverrideSelectedSpeciesUnitTargets(); return GMState.SpeciesSelection; }
                    //if (gm.IsObjectSelected()) { gm.DeselectSpecies(); gm.TargetObject(); return GMState.ObjectSelection; }

                    if (gm.newSpeciesSelected) { gm.SelectSpecies(); return GMState.SpeciesSelection; }
                    if (gm.IsUnitSelected()) { gm.SelectUnit(); gm.DeselectSpecies(); return GMState.UnitSelection; }
                    //todo: is base selected

                    break;
                }
            case GMState.SpeciesAttributes: {
                    if (gm.forcePanelExit) { gm.HideSpeciesSelectionPanel(); return GMState.FreeSelection; }
                    break;
                }
        }
        //gm.SetTargetsToNull();
        return gm.gameState;
    }

}