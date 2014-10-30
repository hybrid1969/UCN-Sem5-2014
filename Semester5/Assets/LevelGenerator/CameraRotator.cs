using UnityEngine;
using System.Collections;

public class CameraRotator : MonoBehaviour
{
	void Update ()
    {
        this.transform.RotateAround(new Vector3(2048, 1, 2048), new Vector3(0, 1, 0), 0.1f);

        this.transform.Translate(this.transform.TransformDirection(0,0,1) * (Input.GetAxis("Mouse ScrollWheel") * 1000), Space.World);

        if (Input.GetKey(KeyCode.Mouse1))
        {
            this.transform.RotateAround(new Vector3(2048, 1, 2048), this.transform.TransformDirection(1, 0, 0), Input.GetAxis("Mouse Y"));
        }
	}
}
