using UnityEngine;
using System.Collections;

public class AddToSystemList_Script : MonoBehaviour {

	public bool IsRaider = false;
	public bool IsToolStore = false;
	public bool IsPowerStation = false;
	public bool IsBuilding = false;
	public bool IsVehicle = false;

	// Use this for initialization
	void Start ()
	{
		if (IsRaider)
			System_Script.RaidersList.Add(this.gameObject);

		if (IsToolStore)
			System_Script.ListOfAllToolStores.Add(this.gameObject);

		if (IsPowerStation)
			System_Script.ListOfAllPowerStations.Add(this.gameObject);

		if (IsBuilding)
			System_Script.AllBuildings.Add(this.gameObject);

		if (IsVehicle)
			System_Script.AllVehicles.Add(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
