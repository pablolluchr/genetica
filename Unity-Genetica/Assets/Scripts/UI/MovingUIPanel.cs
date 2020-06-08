using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingUIPanel : MonoBehaviour
{
    private Canvas canvas;
    private Animator anim;
    // Start is called before the first frame update
    void Awake()
    {
        canvas = GetComponent<Canvas>();
        anim = GetComponent<Animator>();

    }

    public bool Hide() //returs true if succeeded, false if it was already closed
    {
        if (anim.GetBool("shown") == false) return false;
        anim.SetBool("shown", false);
        StartCoroutine(DisablePanelDelayed());
        return true;
    }

    public void Show()
    {
        canvas.enabled = true;
        anim.SetBool("shown", true);

    }

    IEnumerator DisablePanelDelayed()
    {
        yield return new WaitForSeconds(0.3f);
        canvas.enabled = false;
    }
}
