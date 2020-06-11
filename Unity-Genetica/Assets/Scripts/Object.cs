using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    public Canvas selectionGraphic;
    public Animator anim;
    readonly int selectHash = Animator.StringToHash("selected");

    protected void Start() {
        Deselect();
    }

    public void Select() {
        anim.SetTrigger(selectHash);
        selectionGraphic.enabled = true;
        //lightBeam.enabled = true;

    }

    public void Deselect() {
        selectionGraphic.enabled = false;
        //lightBeam.enabled = false;
    }
}
