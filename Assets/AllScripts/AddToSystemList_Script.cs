using UnityEngine;
using System.Collections;

public class AddToSystemList_Script : MonoBehaviour {

	public bool IsRaider = false;
	public bool IsToolStore = false;
	public bool IsPowerStation = false;

	//public GameObject 

	// Use this for initialization
	void Start ()
	{
		if (IsRaider)
			System_Script.RaidersList.Add(this.gameObject);

		if (IsToolStore)
			System_Script.ListOfAllToolStores.Add(this.gameObject);

		if (IsPowerStation)
			System_Script.ListOfAllToolStores.Add(this.gameObject);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
