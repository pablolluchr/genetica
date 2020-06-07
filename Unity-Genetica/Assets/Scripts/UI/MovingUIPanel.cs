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

    public void Hide()
    {
        anim.SetBool("shown", false);
        StartCoroutine(DisablePanelDelayed());
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
