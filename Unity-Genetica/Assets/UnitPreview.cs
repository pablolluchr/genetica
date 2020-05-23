using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPreview : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //TODO: use screen resolution
        RenderTexture rt = new RenderTexture(200, 200, 16);
        GetComponent<Camera>().targetTexture = rt;
        GameManager.gameManager.infoPanel.transform.Find("UnitPreview").GetComponent<RawImage>().texture = rt;
    }
}
