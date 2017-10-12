using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingTypes
{
	Nothing,
	PowerPathBegin,
	PowerPathComplete,
	Rubble,
	Rock,

}
public class TerainAdditions
{
	public BuildingTypes B_Types = BuildingTypes.Nothing;
	public GameObject Object = null;
}

public class Building_System : MonoBehaviour {

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
	public GameObject Stops;



	// Use this for initialization
	void Start ()
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
	void Update ()
	{

		RaycastHit dest;
		var cancel = false;

		if (Input.GetMouseButtonDown(0))
		{
			Ray ray0 = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray0, out dest))
			{
				if(dest.collider.gameObject.transform.tag == "Rock")
				{
					cancel = true;
				}
			}

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			var OnterrainClick = false;

			//if (System_Script.selectedGameObject == null)
			//{
				if (Physics.Raycast(ray, out dest, float.MaxValue, LayerMask.GetMask("Terrain")) && !cancel)
				{
					var P = dest.point;
					var X_ = (Mathf.Round(P.x / SelectorSize)) * SelectorSize;
					var Z_ = (Mathf.Round(P.z / SelectorSize)) * SelectorSize;

					Clicked_X = (int)X_/ SelectorSize;
					Clicked_Z = (int)Z_/ SelectorSize;

					SelectorSquare.transform.position = new Vector3(X_, 0.1f, Z_);

					if (dest.collider.gameObject.transform.tag == "Terrain")
					{
						System_Script.selectedGameObject = null;
						System_Script.SelectedGameObjects.Clear();
						OnterrainClick = true;

						CurrentBuildingType = BuildingGrid[Clicked_X, Clicked_Z].B_Types;
						CurrentObject = BuildingGrid[Clicked_X, Clicked_Z].Object;

						On_Click();
					}
				}
			//}

			if (System_Script.selectedGameObject == null && System_Script.SelectedGameObjects.Count == 0 && OnterrainClick)
			{
				SelectorSquare.SetActive(true);
			}

			if (System_Script.selectedGameObject != null || System_Script.SelectedGameObjects.Count != 0)
			{
				SelectorSquare.SetActive(false);
			}
		}

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
		if(CurrentBuildingType == BuildingTypes.Rubble)
		{
			System_Script.CurrentMenuBarNumber = 5;
			System_Script.selectedGameObject = CurrentObject;
		}

		if (CurrentBuildingType == BuildingTypes.Nothing)
		{
			System_Script.CurrentMenuBarNumber = 5;
		}

		if (CurrentBuildingType == BuildingTypes.PowerPathBegin)
		{
			System_Script.CurrentMenuBarNumber = 6;
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

		System_Script.CurrentMenuBarNumber = 1;

	}

	public void On_Click_CancelBuilding()
	{
		var Object_ = BuildingGrid[Clicked_X, Clicked_Z].Object;
		var ObectScript = Object_.GetComponent<Construction_Script>();
		var X_ = Object_.transform.position.x;
		var Z_ = Object_.transform.position.z;


		if (ObectScript.Contained_Crystal > 0)
		{
			for (int i = 0; i < ObectScript.Contained_Crystal; i++)
			{
				//var newCrystal = Instantiate(System_Script.cry, new Vector3(X_ + Random.Range(-2,2) , 0.1f , Z_ + Random.Range(-2, 2)), Quaternion.identity);
			}
		}

		if (ObectScript.Contained_Ore > 0)
		{
			for (int i = 0; i < ObectScript.Contained_Ore; i++)
			{
				var newCrystal = Instantiate(System_Script.Ore, new Vector3(X_ + Random.Range(-2, 2), 0.1f, Z_ + Random.Range(-2, 2)), Quaternion.identity);
			}
		}

		RemoveFromList(Object_);

		Destroy(Object_);

		FullReset(Clicked_X, Clicked_Z);

		System_Script.CurrentMenuBarNumber = 1;
	}

	public void FullReset(int X, int Z)
	{
		if(CurrentBuildingType == BuildingGrid[X, Z].B_Types)
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
		if(System_Script.ConstructionSites.Contains(Object))
		{
			System_Script.ConstructionSites.Remove(Object);
		}
	}

}
