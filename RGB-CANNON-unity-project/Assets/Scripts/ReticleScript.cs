using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleScript : MonoBehaviour {
    public float offSetZ;
    float rotationAngle;
    public float rotationSpeed;

    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        rotationAngle = rotationSpeed * Time.deltaTime;
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = cursorPos;
        transform.position = new Vector3(cursorPos.x, cursorPos.y, offSetZ);
        transform.Rotate(Vector3.forward, rotationAngle);
    }
}
