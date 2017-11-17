using UnityEngine;
using System.Collections.Generic;

public enum BuildingTypes
{
	Nothing,
	PowerPathBegin,
	PowerPathComplete,
	Rubble,
	Rock,
	Building,
	CompleteBuilding,
	PlaceHolder

}
public class TerainAdditions
{
	public BuildingTypes B_Types = BuildingTypes.Nothing;
	public GameObject Object = null;
}

public class Building_System : MonoBehaviour
{

	public TerainAdditions[,] BuildingGrid = new TerainAdditions[100, 100];

	public BuildingTypes CurrentBuildingType = BuildingTypes.Nothing;
	public GameObject CurrentObject = null;

	public System_Script System_Script;
	public IconEdit_Script IconEdit_Script;
	public AllBuildings AllBuildings_Script;


	public int Clicked_X;
	public int Clicked_Z;
	public int Columns = 100;
	public int Rows = 100;
	public int SelectorSize = 12;

	public GameObject SelectorSquare;
	public bool Inbounds = false;


	public GameObject ToolStore;
	public GameObject Teleportpad;
	public GameObject SupportStation;

	public GameObject Stops;

	public List<GameObject> CorrectWorkersList = new List<GameObject>();
	public bool CurrentlySelecting = false;

	// Use this for initialization
	void Start()
	{
		System_Script = this.GetComponent<System_Script>();
		IconEdit_Script = this.GetComponent<IconEdit_Script>();
		AllBuildings_Script = this.GetComponent<AllBuildings>();

		for (int iX = 0; iX < Rows; iX++)
		{
			for (int iY = 0; iY < Columns; iY++)
			{
				BuildingGrid[iX, iY] = new TerainAdditions { B_Types = BuildingTypes.Nothing, Object = null };
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (Clicked_X >= 0 && Clicked_X <= Columns && Clicked_Z >= 0 && Clicked_Z <= Rows)
		{
			Inbounds = true;
		}
		else
		{
			Inbounds = false;
		}

		UpdateIcons();
	}

	// on click within bounds
	public void On_Click()
	{
		if (CurrentBuildingType == BuildingTypes.Rubble)
		{
			System_Script.ChangeMenu(5);
			System_Script.selectedGameObject = CurrentObject;
		}

		if (CurrentBuildingType == BuildingTypes.Nothing)
		{
			System_Script.ChangeMenu(5);
		}

		if (CurrentBuildingType == BuildingTypes.PowerPathBegin)
		{
			System_Script.ChangeMenu(6);
		}

		if (CurrentBuildingType == BuildingTypes.Building)
		{
			System_Script.ChangeMenu(6);
		}

		if (CurrentBuildingType == BuildingTypes.PlaceHolder)
		{
			System_Script.ChangeMenu(1);
		}
	}

	public void UpdateIcons()
	{
		if (CurrentBuildingType == BuildingTypes.Rubble && CurrentObject != null)
		{
			if (CurrentObject.GetComponent<Work_Script>().WorkedOn)
			{
				IconEdit_Script.Enable_Rubble(false);
			}
			else
			{
				IconEdit_Script.Enable_Rubble(true);
			}
		}
	}

	public void On_PowerPath_Click()
	{
		var newPowerPath = Instantiate(AllBuildings_Script.PowerPathBegin, new Vector3(Clicked_X * SelectorSize, 0.1f, Clicked_Z * SelectorSize), Quaternion.identity);

		newPowerPath.GetComponent<Construction_Script>().ConstructionType = ConstructionTypes.PowerPath;
		BuildingGrid[Clicked_X, Clicked_Z].Object = newPowerPath;
		BuildingGrid[Clicked_X, Clicked_Z].B_Types = BuildingTypes.PowerPathBegin;

		System_Script.ChangeMenu(1);
	}

	public void On_Click_CancelBuilding()
	{
		var Object_ = BuildingGrid[Clicked_X, Clicked_Z].Object;
		var ObectScript = Object_.GetComponent<Construction_Script>();
		var X_ = Object_.transform.position.x;
		var Z_ = Object_.transform.position.z;


		if (ObectScript.ExtraPaths.Count > 0)
		{
			foreach (var item in ObectScript.ExtraPaths)
			{
				var Pos = item.transform.position;
				var PosX = (int)Pos.x;
				var PosZ = (int)Pos.z;

				BuildingGrid[PosX / 12, PosZ / 12].Object = null;
				BuildingGrid[PosX / 12, PosZ / 12].B_Types = BuildingTypes.Nothing;
				Destroy(item);
			}

			foreach (var item in ObectScript.aquiredObj)
			{
				item.GetComponent<Collectable>().MakeUnStatic();
			}
		}

		RemoveFromList(Object_);


		foreach (var item in System_Script.AllWorkers)
		{
			var WorkerScript = item.GetComponent<Lego_Character>();

			if (WorkerScript.TaskObject == Object_)
			{
				item.GetComponent<Lego_Character>().ItemDropOffPointDestroyed();
			}
		}

		Destroy(Object_);

		FullReset(Clicked_X, Clicked_Z);

		System_Script.ChangeMenu(1);
	}

	public void FullReset(int X, int Z)
	{
		if (CurrentBuildingType == BuildingGrid[X, Z].B_Types)
		{
			CurrentBuildingType = BuildingTypes.Nothing;
		}

		if (CurrentObject == BuildingGrid[X, Z].Object)
		{
			CurrentObject = null;
		}

		BuildingGrid[X, Z].Object = null;
		BuildingGrid[X, Z].B_Types = BuildingTypes.Nothing;
	}

	public void RemoveFromList(GameObject Object)
	{
		if (System_Script.ConstructionSites.Contains(Object))
		{
			System_Script.ConstructionSites.Remove(Object);
		}

		System_Script.GetTotalStops();

	}
}
