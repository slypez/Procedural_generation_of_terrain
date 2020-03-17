using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse_Interaction : MonoBehaviour
{
    [SerializeField] private float scrollSpeed;
    [SerializeField] private float rayLength;
    [SerializeField] private Camera cam;
    private Transform movedObj;
    private bool tryingToMove;

    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red);
        
        if (Physics.Raycast(ray, out hit, rayLength) && tryingToMove == false)
        {
            Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.green);
            if (hit.transform.tag == "Ball" )
            {
                movedObj = hit.transform;
            }
        }
        
        if (Input.GetKey(KeyCode.Mouse0))
        {
            tryingToMove = true;
            if (movedObj != null)
            {
                Vector3 newPosition = ray.origin + ray.direction * (movedObj.transform.position - cam.transform.position).magnitude; 
                newPosition.z = movedObj.transform.position.z + Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
                movedObj.transform.position = newPosition;
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            tryingToMove = false;
            movedObj = null;
        }
    }

}
