using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class UITouch : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // in charge of detecting what is clicked / what is dragged and to where
    // dragging will work by determining if the drag has gone past an additional square, 
    // if it has join those two sqaures as one sustained note


    float lengthOfCell;

    DrumRoll drumRoll;
    DrumCell cell ;
    

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {   
            cell = hit.collider.GetComponent<DrumCell>();
            if (cell != null)
                cell.OnClick();
        }
    }
    }

    public void Init(float loc, DrumRoll dr)
    {
        lengthOfCell = loc;
        drumRoll = dr;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
       
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (cell != null)
            cell.OnClick();
        Debug.Log("dragging");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
       
    }
}
