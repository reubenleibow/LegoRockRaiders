using UnityEngine;
using System.Collections;

public class CameraLookAt_Script : MonoBehaviour
{
	public GameObject Eyes;

	void Start()
	{
	}

	void Update()
	{
		this.transform.LookAt(Eyes.transform.position);
	}
}
