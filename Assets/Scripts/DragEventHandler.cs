using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class DragEventHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] private Camera mainCamera;
    [SerializeField] private RectTransform bar;
    [SerializeField] private CanvasGroup canvasGroup;
    private float speed = 3.5f;
    private float X;
    private float Y;
    private bool rotate;


    void Start() {
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        canvasGroup.DOFade(0.0f, 0.25f).SetEase(Ease.OutSine).OnComplete(() => { canvasGroup.interactable = false; });
    }

    public void OnPointerExit(PointerEventData eventData) {
        canvasGroup.DOFade(1.0f, 0.25f).SetEase(Ease.InSine).OnComplete(() => { canvasGroup.interactable = true; });
    }

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
