using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragEventHandler : MonoBehaviour, IDragHandler, IEndDragHandler {

    public float speed = 3.5f;
    public Camera mainCamera;
    private float X;
    private float Y;
    private bool rotate;
           
   
    public void OnDrag(PointerEventData eventData) {
        rotate = true;
    }
    
    public void OnEndDrag(PointerEventData eventData) {
        rotate = false;
    }
    private void Update() {
        if(!rotate) {
            return;
        }

        if (Input.GetMouseButton(0)) {
            mainCamera.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * speed, -Input.GetAxis("Mouse X") * speed, 0));
            X = mainCamera.transform.rotation.eulerAngles.x;
            Y = mainCamera.transform.rotation.eulerAngles.y;
            mainCamera.transform.rotation = Quaternion.Euler(X, Y, 0);
        }
    }
}
