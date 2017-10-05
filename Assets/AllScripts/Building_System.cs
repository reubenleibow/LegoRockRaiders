using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingTypes
{
	Nothing,
	PowerPathBegin,
	PowerPathComplete,
	Rubble
}
public class TerainAdditions
{
	//BuildingTypes BuildingTypes;
	//GameObject GameObject;

	public BuildingTypes B_Types = BuildingTypes.Nothing;
	public GameObject Object = null;


	//TerainAdditions()
	//{
	//	B_Types = BuildingTypes.Nothing;
	//	Object = null;
	//}
}

public class Building_System : MonoBehaviour {

	public TerainAdditions[,] BuildingGrid = new TerainAdditions[100, 100];

	public BuildingTypes CurrentBuildingType = BuildingTypes.Nothing;
	public GameObject CurrentObject = null;

	public System_Script System_Script;
	public IconEdit_Script IconEdit_Script;

	public int Clicked_X;
	public int Clicked_Z;
	public int Columns = 100;
	public int Rows = 100;
	public int SelectorSize = 12;

	public GameObject SelectorSquare;
	public bool Inbounds = false;



	// Use this for initialization
	void Start ()
	{
		System_Script = this.GetComponent<System_Script>();
		IconEdit_Script = this.GetComponent<IconEdit_Script>();

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

		if (Input.GetMouseButtonDown(0))
		{
			

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			var OnterrainClick = false;

			//if (System_Script.selectedGameObject == null)
			//{
				if (Physics.Raycast(ray, out dest, float.MaxValue, LayerMask.GetMask("Terrain")))
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
		if(CurrentBuildingType == BuildingTypes.Nothing)
		{

		}
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


}
