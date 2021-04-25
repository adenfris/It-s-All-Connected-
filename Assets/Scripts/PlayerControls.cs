using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class PlayerControls : MonoBehaviour
{
    private Camera playerCamera;

    private Vector3 cameraDragOrigin;
    private Vector3 globalDragOrigin;

    private bool areDraggingView = false;

    private void Start()
    {
        playerCamera = this.GetComponent<Camera>();
    }

    private void Update()
    {
        DragToMove();
        Zoom();
    }

    private void DragToMove()
    {
        if (Input.GetButtonDown(InputNames.dragView)) // If we just pressed the button
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] mouseHit = Physics2D.RaycastAll(mousePos, Vector2.up, 0.0000001f);
            
            if (mouseHit.Length == 0 && !EventSystem.current.IsPointerOverGameObject()) // If there are no objects under the mouse
            {
                areDraggingView = true;
                cameraDragOrigin = Input.mousePosition;
                globalDragOrigin = playerCamera.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else if (Input.GetButton(InputNames.dragView) && areDraggingView)  // If we didn't *just* press the button, but we're still pressing it
        {
            Vector3 cameraDragNew = Input.mousePosition;
            Vector3 globalDragNew = playerCamera.ScreenToWorldPoint(cameraDragNew);
            Vector3 moveVector = globalDragOrigin - globalDragNew;

            transform.Translate(moveVector, Space.Self);
        }
        else
        {
            areDraggingView = false;
        }
    }

    private void Zoom()
    {
        float scrollWheel = Input.GetAxis(InputNames.zoomAxis);
        float zoomMult = Mathf.Abs(scrollWheel) * Settings.Instance.zoomSpeed * Time.deltaTime;
        zoomMult += 1;

        if (scrollWheel > 0)
        {
            zoomMult = 1 / zoomMult;
        }

        if (scrollWheel != 0)
        {
            playerCamera.orthographicSize = playerCamera.orthographicSize * zoomMult;
        }
    }
}
