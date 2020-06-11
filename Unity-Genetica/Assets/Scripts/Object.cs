using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    public Canvas selectionGraphic;
    public Animator anim;
    public MeshRenderer lightbeam;
    readonly int selectHash = Animator.StringToHash("selected");

    protected void Start() {
        Deselect();
        if (lightbeam!=null)HideBeam();
    }

    public void Select() {
        anim.SetTrigger(selectHash);
        selectionGraphic.enabled = true;

    }

    public void Deselect() {
        selectionGraphic.enabled = false;
    }

    public void ShowBeam() {
        lightbeam.enabled = true;
        anim.SetTrigger(selectHash);


    }

    public void HideBeam() {
        lightbeam.enabled = false;

    }
}
