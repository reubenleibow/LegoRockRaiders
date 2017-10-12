using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Construction_Script : MonoBehaviour {

	public List<GameObject> Workerlist_Ore = new List<GameObject>();
	public List<GameObject> Workerlist_Crystal = new List<GameObject>();
	public List<GameObject> Workerlist_Stops = new List<GameObject>();
	//pre made up of emptygame objects on path
	public List<GameObject> RequiredStopsListPoints = new List<GameObject>();
	public List<GameObject> aquiredObj = new List<GameObject>();


	public int Required_Ore = 2;
	public int Required_Crystal = 0;
	public int Required_Stops = 0;


	public bool Ore_Worker = false;
	public bool Crystal_Worker = false;
	public bool Stops_Worker = false;
	public bool CompletedConstruction = false;


	public int Contained_Ore = 0;
	public int Contained_Crystal = 0;
	public int Contained_Stops = 0;

	public AllBuildings AllBuildings;
	public Building_System Building_System;
	public GameObject systemObj;

	public GameObject StopPoint1;
	public GameObject StopPoint2;
	public GameObject StopPoint3;
	public GameObject StopPoint4;

	public ConstructionTypes ConstructionType = ConstructionTypes.Nothing;
	public bool changeInStops = false;


	// Use this for initialization
	void Start ()
	{
		systemObj = GameObject.Find("System");
		AllBuildings = systemObj.GetComponent<AllBuildings>();
		Building_System = systemObj.GetComponent<Building_System>();

		if(ConstructionType == ConstructionTypes.Teleportpad)
		{
			RequiredStopsListPoints.Add(StopPoint1);
			RequiredStopsListPoints.Add(StopPoint2);
			RequiredStopsListPoints.Add(StopPoint3);
			RequiredStopsListPoints.Add(StopPoint4);
		}
	}

	// Update is called once per frame
	void Update()
	{
		if(Workerlist_Ore.Count > 0)
		{
			foreach (var units in Workerlist_Ore.ToArray())
			{
				var unitsTask = units.GetComponent<Lego_Character>().TaskObject;

				if(unitsTask != this.gameObject)
				{
					Workerlist_Ore.Remove(units);
				}
			}
		}
		if (Workerlist_Crystal.Count > 0)
		{
			foreach (var units in Workerlist_Crystal.ToArray())
			{
				var unitsTask = units.GetComponent<Lego_Character>().TaskObject;

				if (unitsTask != this.gameObject)
				{
					Workerlist_Crystal.Remove(units);
				}
			}
		}

		if (Workerlist_Stops.Count > 0)
		{
			foreach (var units in Workerlist_Stops.ToArray())
			{
				var unitsTask = units.GetComponent<Lego_Character>().TaskObject;

				if (unitsTask != this.gameObject)
				{
					Workerlist_Stops.Remove(units);
				}
			}
		}

		if (Required_Ore > Workerlist_Ore.Count + Contained_Ore)
		{
			Ore_Worker = true;
		}
		else
		{
			Ore_Worker = false;
		}

		if (Required_Crystal > Workerlist_Crystal.Count + Contained_Crystal)
		{
			Crystal_Worker = true;
		}
		else
		{
			Crystal_Worker = false;
		}

		if (Required_Stops > Workerlist_Stops.Count + Contained_Stops)
		{
			Stops_Worker = true;
		}
		else
		{
			Stops_Worker = false;
		}

		//if the building is not complete
		if(Crystal_Worker || Ore_Worker || Stops_Worker)
		{
			if(!System_Script.ConstructionSites.Contains(this.gameObject))
			{
				System_Script.ConstructionSites.Add(this.gameObject);
			}
		}

		//if the building is complete
		if (Required_Ore <= Contained_Ore  && Required_Crystal <= Contained_Crystal && Required_Stops <= Contained_Stops)
		{
			CompletedConstruction = true;
			Completed();
		}

		//if(RequiredStopsListPoints )
	}

	public void PlaceOre()
	{

	}

	public void Completed()
	{
		if (System_Script.ConstructionSites.Contains(this.gameObject))
		{
			System_Script.ConstructionSites.Remove(this.gameObject);
		}

		foreach (var item in aquiredObj.ToArray())
		{
			Destroy(item);
		}

		Destroy(this.gameObject);
	}

	public void OnDestroy()
	{
		if(CompletedConstruction && ConstructionType == ConstructionTypes.PowerPath)
		{
			var newPowerpathcomplete = Instantiate(AllBuildings.PowerPathComplete, this.transform.position, Quaternion.identity);
			var X = Mathf.Round(this.transform.position.x / 12);
			var Z = Mathf.Round(this.transform.position.z / 12);

			Building_System.BuildingGrid[(int)X, (int)Z].Object = newPowerpathcomplete;
			Building_System.BuildingGrid[(int)X, (int)Z].B_Types = BuildingTypes.PowerPathComplete;
		}

		if (CompletedConstruction && ConstructionType == ConstructionTypes.ToolStore)
		{
			var script = systemObj.GetComponent<Building_System>();
			var script1 = systemObj.GetComponent<StartConstruction>();
			var building = Instantiate(script.ToolStore, this.transform.position, Quaternion.identity);
			var path = Instantiate(script1.ExtraPath, this.transform.position, Quaternion.identity);
		}

		if (CompletedConstruction && ConstructionType == ConstructionTypes.Teleportpad)
		{
			var script = systemObj.GetComponent<Building_System>();
			var script1 = systemObj.GetComponent<StartConstruction>();
			var building = Instantiate(script.Teleportpad, this.transform.position, Quaternion.identity);
			var path = Instantiate(script1.ExtraPath, this.transform.position, Quaternion.identity);
		}
	}
}
