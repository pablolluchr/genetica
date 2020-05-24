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
                if (gm.IsObjectSelected()) { gm.TargetObject(); return GMState.ObjectSelection; }
                    if (gm.IsAreaSelected()) { gm.SelectSpecies(); return GMState.SpeciesSelection; }

                    if (gm.IsUnitSelected()) { gm.TargetUnit(); gm.SetTargetUnitGraphics(); return GMState.UnitSelection; }
                if (gm.selectedSpecies != gm.previousSelectedSpecies) { gm.SelectSpecies(); return GMState.SpeciesSelection; }
                break;
            }

            case GMState.ObjectSelection: {
                    if (gm.IsAreaSelected()) { gm.SelectSpecies(); return GMState.SpeciesSelection; }

                    if (gm.isDragging) { gm.FreePan(); return GMState.FreeSelection; }
                if (gm.IsObjectSelected()) { gm.TargetObject(); return GMState.ObjectSelection; }
                if (gm.IsUnitSelected()) { gm.TargetUnit(); gm.SetTargetUnitGraphics(); return GMState.UnitSelection; }
                if (gm.isShortClick) { gm.FreePan(); return GMState.FreeSelection; }
                break;
            }

            case GMState.UnitSelection: {
                    if (gm.IsAreaSelected()) { gm.SelectSpecies(); return GMState.SpeciesSelection; }

                    if (gm.forceUnitSelectionExit) { gm.ForceSelectionExit(); return GMState.FreeSelection; }
                if (gm.isDragging) { gm.FreePan(); return GMState.UnitSelection; }
                if (gm.IsObjectSelected()) { gm.OverrideUnit(); return GMState.FreeSelection; }
                if (gm.IsUnitSelected() && gm.NewUnitSelected()) { gm.TargetUnit(); gm.SetTargetUnitGraphics(); return GMState.UnitSelection; }
                if (gm.IsUnitSelected()) { gm.TargetUnit(); return GMState.UnitSelection; }
                if (gm.selectedSpecies != gm.previousSelectedSpecies) { gm.SelectSpecies(); return GMState.SpeciesSelection; }
                if (gm.PointSelected()) { gm.OverrideUnit(); gm.HideInfoPanel();  return GMState.FreeSelection; }
                break;
            }

            case GMState.SpeciesSelection: {
                if (gm.isDragging) { gm.FreePan(); return GMState.SpeciesSelection; }
                if (!gm.IsAreaSelected()) { gm.DeselectSpecies(); return GMState.FreeSelection; }
                if (gm.NewSpeciesSelected()) { gm.SelectSpecies(); return GMState.SpeciesSelection; }
                if (gm.isShortClick) {
                        gm.SetHabitat(); gm.DeselectSpecies(); return GMState.FreeSelection; }
                break;
            }
        }
        //gm.SetTargetsToNull();
        return gm.gameState;
    }

}