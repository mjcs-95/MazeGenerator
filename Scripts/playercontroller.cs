using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playercontroller : MonoBehaviour
{

    public Camera camera_;
    public float hCameraSpeed;
    public float vCameraSpeed;
    public float moveSpeed;

    float hRotation;
    float vRotation;
    private Ray ray;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int layerMask = 1 << 8;
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.forward);

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 0.75f, layerMask)) {
            if (hit.collider.gameObject.tag == "door") {
                hit.collider.gameObject.SetActive(false);
                /*if (hit.collider.gameObject.transform.localRotation.y == 0) {
                    hit.collider.gameObject.transform.parent.Rotate(0, 90, 0);
                }*/
            }
        }
        hRotation = hCameraSpeed * Input.GetAxis("Mouse X");
        vRotation = vCameraSpeed * Input.GetAxis("Mouse Y");

        transform.Rotate(0, hRotation, 0);
        camera_.transform.Rotate(vRotation, 0, 0);

        if (Input.GetKey(KeyCode.W)) {
            transform.Translate(0, 0, moveSpeed);
        }
        if (Input.GetKey(KeyCode.S)) {
            transform.Translate(0, 0, -moveSpeed);
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.Translate(moveSpeed, 0, 0);
        }
        if (Input.GetKey(KeyCode.A)) {
            transform.Translate(-moveSpeed, 0, 0);
        }
    }
}
