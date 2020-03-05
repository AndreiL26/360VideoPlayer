using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonePopUpController : MonoBehaviour
{
    public CanvasGroup zonePopUp;
    private bool isEnabled = true;

    public void EnablePopUp() {
        if(isEnabled == false) {
            zonePopUp.interactable = true;
            zonePopUp.alpha = 1;
            isEnabled = true;
        }
    }

    public void DisablePopUp() {
        Debug.Log("HELLO WORLD");
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
