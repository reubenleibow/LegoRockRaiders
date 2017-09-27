using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rubble_Script : MonoBehaviour {

	public float Health = 100;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if(Health <= 0)
		{
			Destroy(this.gameObject);
		}
	}

	private void OnDestroy()
	{
		if(System_Script.ClearRubble.Contains(this.gameObject))
		{
			System_Script.ClearRubble.Remove(this.gameObject);
		}
	}
}
