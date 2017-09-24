using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Work_Script : MonoBehaviour {

	public GameObject Worker;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(Worker != null)
		{
			if(Worker.GetComponent<Lego_Character>().TaskObject != this.gameObject)
			{
				Worker = null;

				if(GetComponent<Rock_Script>().WorkedOn)
				{
					AddtoList();
				}
			}

			if (System_Script.DrillRocks.Contains(this.gameObject))
			{
				System_Script.DrillRocks.Remove(this.gameObject);
			}
		}
	}

	public void AddtoList()
	{
		System_Script.DrillRocks.Add(this.gameObject);
	}
}
