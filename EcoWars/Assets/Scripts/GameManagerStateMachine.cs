using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GMState {
    UnitSelection,
    ObjectSelection,
    SpeciesSelection,
    FreeSelection
}

public static class GameManagerStateMachine {

    public static GMState NextState() {
        GameManager gm = GameManager.gameManager;

        switch (gm.gameState) {
            case GMState.FreeSelection: {
                if (gm.IsUnitSelected()) { gm.TargetUnit(); return GMState.UnitSelection; }
                if (gm.IsObjectSelected()) { gm.TargetObject(); return GMState.ObjectSelection; }
                if (gm.selectedSpecies != gm.previousSelectedSpecies) { gm.SelectSpecies(); return GMState.SpeciesSelection; }
                break;
            }

            case GMState.ObjectSelection: {
                if (gm.isDragging) { gm.FreePan(); return GMState.FreeSelection; }
                if (gm.IsObjectSelected()) { gm.TargetObject(); return GMState.ObjectSelection; }
                if (gm.IsUnitSelected()) { gm.TargetUnit(); return GMState.UnitSelection; }
                if (gm.isShortClick) { gm.FreePan(); return GMState.FreeSelection; }
                break;
            }

            case GMState.UnitSelection: {
                if (gm.isDragging) { gm.FreePan(); return GMState.UnitSelection; }
                if (gm.IsUnitSelected()) { gm.TargetUnit(); return GMState.UnitSelection; }
                if (gm.IsObjectSelected()) { gm.OverrideUnit(); return GMState.FreeSelection; }
                if (gm.selectedSpecies != gm.previousSelectedSpecies) { gm.SelectSpecies(); return GMState.SpeciesSelection; }
                if (gm.PointSelected()) { gm.OverrideUnit(); return GMState.FreeSelection; }
                break;
            }

            case GMState.SpeciesSelection: {
                if (gm.isDragging) { gm.FreePan(); return GMState.SpeciesSelection; }
                if (gm.selectedSpecies == null) { gm.DeselectSpecies(); return GMState.FreeSelection; }
                if (gm.selectedSpecies != gm.previousSelectedSpecies) { gm.SelectSpecies(); return GMState.SpeciesSelection; }
                if (gm.PointSelected()) { gm.SetHabitat(); return GMState.SpeciesSelection; }
                break;
            }
        }
        gm.SetTargetsToNull();
        return gm.gameState;
    }

}