using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerControls : MonoBehaviour
{
    private Camera playerCamera;

    private Vector3 cameraDragOrigin;
    private Vector3 globalDragOrigin;

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
        if (Input.GetButtonDown(ControlNames.mainClick)) // If we just pressed the button
        {
            cameraDragOrigin = Input.mousePosition;
            globalDragOrigin = playerCamera.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetButton(ControlNames.mainClick))  // If we didn't *just* press the button, but we're still pressing it
        {
            Vector3 cameraDragNew = Input.mousePosition;
            Vector3 globalDragNew = playerCamera.ScreenToWorldPoint(cameraDragNew);
            Vector3 moveVector = globalDragOrigin - globalDragNew;

            transform.Translate(moveVector, Space.Self);
        }
    }

    private void Zoom()
    {
        float scrollWheel = Input.GetAxis(ControlNames.scrollWheel);
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
