using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class UITouch : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // in charge of detecting what is clicked / what is dragged and to where
    // dragging will work by determining if the drag has gone past an additional square, 
    // if it has join those two sqaures as one sustained note


    float lengthOfCell;

    DrumRoll drumRoll;
    DrumCell cell ;

    Vector2 dragStartPosistion;

    DrumCell dragCell;

    private Camera mainCamera;


    void Start()
    {
        mainCamera = Camera.main;
    }
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
            if (hit.collider.tag == "Segment")
                {
                    drumRoll.SwitchSegment(hit.collider.GetComponent<Segment>().Index);
                }
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
       dragStartPosistion = eventData.position;
       Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
       Physics.Raycast(ray, out RaycastHit hit);
       dragCell = hit.collider.GetComponent<DrumCell>();
       Debug.Log(dragStartPosistion);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 dragCurrentPosition = eventData.position;
        float dragDistanceX = dragStartPosistion.x-dragCurrentPosition.x;
        float dragDistanceY = dragStartPosistion.y-dragCurrentPosition.y;

        dragCell.amplitude -= dragDistanceY/100;
        if (dragCell.amplitude >1)
            dragCell.amplitude = 1;
        
        if (dragCell.amplitude <0)
            dragCell.amplitude = 0;
        


        dragCell.noteLength += dragDistanceX/100;
         dragCell.UpdateVisual();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
       
    }
}
