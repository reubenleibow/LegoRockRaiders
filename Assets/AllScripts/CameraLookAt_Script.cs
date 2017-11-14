using UnityEngine;
using System.Collections;

public class CameraLookAt_Script : MonoBehaviour
{
	public GameObject Eyes;
	public Camera MainCamera;
	public GameObject CameraHolder;
	public float Zoom;
	private int ScrollSpeed = 80;
	public bool Enabled = false;



	void Start()
	{

	}

	void Update()
	{
		if (Enabled)
		{

			//var fromYBorders = (Screen.height * 0.95) - (Screen.height - Input.mousePosition.y);
			if (Input.mousePosition.y > Screen.height - 10)
			{
				CameraHolder.transform.Translate(Vector3.forward * ScrollSpeed * Time.deltaTime);
			}

			if (Input.mousePosition.y < 10)
			{
				CameraHolder.transform.Translate(Vector3.forward * -ScrollSpeed * Time.deltaTime);
			}

			if (Input.mousePosition.x < 10)
			{
				CameraHolder.transform.Translate(Vector3.left * ScrollSpeed * Time.deltaTime);
			}

			if (Input.mousePosition.x > Screen.width - 10)
			{
				CameraHolder.transform.Translate(Vector3.right * ScrollSpeed * Time.deltaTime);
			}


			if (Input.GetKey("w"))
			{
				CameraHolder.transform.Translate(Vector3.forward * ScrollSpeed * Time.deltaTime);
			}

			if (Input.GetKey("s"))
			{
				CameraHolder.transform.Translate(Vector3.forward * -ScrollSpeed * Time.deltaTime);
			}

			if (Input.GetKey("a"))
			{
				CameraHolder.transform.Translate(Vector3.left * ScrollSpeed * Time.deltaTime);
			}

			if (Input.GetKey("d"))
			{
				CameraHolder.transform.Translate(Vector3.right * ScrollSpeed * Time.deltaTime);
			}
		}
			if (Input.GetAxis("Mouse ScrollWheel") != 0)
			{
				Zoom = Input.GetAxis("Mouse ScrollWheel");
				Camera.main.fieldOfView -= Zoom * 10;
				Camera.main.orthographicSize -= Zoom * 10;
			}
	}
}
