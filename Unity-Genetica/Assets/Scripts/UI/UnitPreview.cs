using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPreview : MonoBehaviour
{
    public RawImage unitPreview;
    // Start is called before the first frame update
    void Start()
    {
        //TODO: use screen resolution
        RenderTexture rt = new RenderTexture(200, 200, 16);
        GetComponent<Camera>().targetTexture = rt;
        unitPreview.texture = rt;
    }
}
