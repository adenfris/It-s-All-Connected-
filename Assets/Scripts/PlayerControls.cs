using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PlayerControls : MonoBehaviour
{
    private Camera camera;

    private Vector3 cameraDragOrigin;
    private Vector3 globalDragOrigin;

    private void Start() {
        camera = this.GetComponent<Camera>();
    }

    private void Update()
    {
        DragToMove();
    }

    private void DragToMove()
    {
        if (Input.GetButtonDown(ControlNames.mainClick)) // If we just pressed the button
        {
            cameraDragOrigin = Input.mousePosition;
            globalDragOrigin = camera.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetButton(ControlNames.mainClick))  // If we didn't *just* press the button, but we're still pressing it
        {
            Vector3 cameraDragNew = Input.mousePosition;
            Vector3 globalDragNew = camera.ScreenToWorldPoint(cameraDragNew);
            Vector3 moveVector = globalDragOrigin - globalDragNew;

            transform.Translate(moveVector, Space.Self);
        }
    }
}
