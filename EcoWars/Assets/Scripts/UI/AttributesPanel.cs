using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttributesPanel : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        //TODO: remove button and replace by slide down
        transform.Find("Close").GetComponent<Button>().onClick.AddListener(ClosePanel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OpenPanel(string speciesName)
    {
        gameObject.SetActive(true); //replace by sliding up animation
    }

    void ClosePanel()
    {

        //TODO: swipe down (kinda like the notificaiton bar). X button in the meanwhile

        //open other menus and close current
        GameManager.gameManager.bottomControls.SetActive(true);
        GameManager.gameManager.speciesSelectionPanel.SetActive(true);
        gameObject.SetActive(false); //replace by sliding up animation
    }
}
