using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonePopUpController : MonoBehaviour
{
    [SerializeField] private CanvasGroup zonePopUp;
    [SerializeField] private Transform closeButton;
    private bool isEnabled = true;

    void Update() {
        if(Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                if(hit.transform.tag == "PopUpCloseButton" && closeButton == hit.transform) {
                    DisablePopUp();
                }
            }
        }
    }

    public void EnablePopUp() {
        if(isEnabled == false) {
            zonePopUp.interactable = true;
            zonePopUp.alpha = 1;
            isEnabled = true;
        }
    }

    public void DisablePopUp() {
        if(isEnabled == true) {
            zonePopUp.interactable = false;
            zonePopUp.alpha = 0;
            isEnabled = false;
        }
    }
                
    public bool IsEnabled() {
        return isEnabled;
    }
}
