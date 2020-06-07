using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GMState {
    UnitSelection,
    ObjectSelection,
    SpeciesSelection,
    FreeSelection,
    SpeciesAttributes,
}

public static class GameManagerStateMachine {

    public static GMState NextState() {
        GameManager gm = GameManager.gameManager;

        switch (gm.gameState) {
            case GMState.FreeSelection: {
                    if (gm.IsObjectSelected()) { gm.TargetObject(); return GMState.ObjectSelection; }
                    if (gm.IsSpeciesSelected()) { gm.SelectSpecies(); return GMState.SpeciesSelection; }
                    if (gm.IsUnitSelected()) {gm.SelectUnit(); return GMState.UnitSelection;}
                    if (gm.selectedSpecies != gm.previousSelectedSpecies) { gm.SelectSpecies(); return GMState.SpeciesSelection; }
                    break;
                }

            case GMState.ObjectSelection: {
                    if (gm.IsSpeciesSelected()) { gm.SelectSpecies(); return GMState.SpeciesSelection; }
                    if (gm.isDragging) { gm.FreePan(); return GMState.FreeSelection; }
                    if (gm.IsObjectSelected()) { gm.TargetObject(); return GMState.ObjectSelection; }
                    if (gm.IsUnitSelected()) { gm.SelectUnit(); return GMState.UnitSelection; }
                    if (gm.isShortClick) { gm.FreePan(); return GMState.FreeSelection; }
                    break;
                }

            case GMState.UnitSelection: {
                    if (gm.forceUnitSelectionExit) { gm.FreePan(); gm.DeselectUnit(); gm.ShowSpeciesSelectionPanel(); return GMState.FreeSelection; }
                    if (gm.isDragging) { gm.FreePan(); return GMState.UnitSelection; }
                    if (gm.IsObjectSelected()) { gm.DeselectUnit(); gm.TargetObject(); return GMState.ObjectSelection; }
                    if (gm.NewUnitSelected()) { gm.SelectUnit(); return GMState.UnitSelection; }
                    break;
                }

            case GMState.SpeciesSelection: {
                    if (gm.isDragging) { gm.FreePan(); return GMState.SpeciesSelection; }
                    if (!gm.IsSpeciesSelected()) {
                        gm.DeselectSpecies(); gm.ShowSpeciesSelectionPanel();
                        return GMState.FreeSelection;
                    }
                    if (gm.newSpeciesSelected) { gm.SelectSpecies(); return GMState.SpeciesSelection; }
                    if (gm.IsObjectSelected()) { gm.SetHabitatTargets(); return GMState.SpeciesSelection; }
                    if (gm.IsUnitSelected()) { gm.SelectUnit(); return GMState.UnitSelection; }

                    break;
                }
        }
        //gm.SetTargetsToNull();
        return gm.gameState;
    }

}