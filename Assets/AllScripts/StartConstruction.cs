using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingGroundType
{
	one_one,
	blank
}

public enum ConstructionTypes
{
	PowerPath,
	ToolStore,
	Teleportpad,
	SupportStation,
	Nothing,
	Building,
}

public enum ConstructionAngle
{
	Left,
	Right,
	Up,
	Down,
	Nothing
}
public class ConstructionSquare
{
	public GameObject Square;
	public int X = 0;
	public int Z = 0;
}
/// <summary>
/// This is at the moment only used for the initiation for building creating
/// </summary>
public class StartConstruction : MonoBehaviour
{

	public List<ConstructionSquare> BuildingSquareList = new List<ConstructionSquare>();
	public List<GameObject> BuildingStopsList = new List<GameObject>();
	public GameObject BuildingSquare_Y;
	public GameObject BuildingSquare_G;
	public GameObject ExtraPath;
	public GameObject BasePath;


	public BuildingGroundType B_Type = BuildingGroundType.blank;
	public ConstructionTypes ConstructionTypes = ConstructionTypes.Nothing;
	public ConstructionAngle ConstructionAngle = ConstructionAngle.Nothing;


	public int OreForProject = 0;
	public int CrystalsForProject = 0;
	public int StoprForProject = 0;
	public Vector3 BuildingAngle = Vector3.zero;


	public int MousePosX = 0;
	public int MousePosZ = 0;

	public GameObject systemObj;
	public Building_System Building_System;


	// Use this for initialization
	void Start()
	{
		systemObj = GameObject.Find("System");
		Building_System = systemObj.GetComponent<Building_System>();
	}

	// Update is called once per frame
	void Update()
	{
		RaycastHit dest;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		var cancel = false;
		var overUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();

		if (overUI)
		{
			cancel = true;
		}

		if (Physics.Raycast(ray, out dest, float.MaxValue, LayerMask.GetMask("Terrain")))
		{
			if (dest.collider.gameObject.transform.tag == "Terrain")
			{
				MousePositionOnTerrain(dest.point);
			}
		}

		//at the moment it is only used as a power path creator for plans
		if (B_Type != BuildingGroundType.blank)
		{
			var Construction = new Construction_Script();

			if (Input.GetMouseButtonDown(0))
			{
				var MainNewBuildSite = new GameObject();
				var CurrentNewBuilding = new GameObject();
				var Set = false;
				BuildingStopsList.Clear();

				//Loops through the BuildingplansSquares to create the powerpaths for buildings

				if(!cancel)
				{

				foreach (var square in BuildingSquareList.ToArray())
				{
					var name = square.Square.transform.name;
					var Pos = square.Square.transform.position;
					var PosX = (int)Pos.x / 12;
					var PosZ = (int)Pos.z / 12;

					if (name == "Y")
					{
						var newPath = Instantiate(BasePath, square.Square.transform.position, Quaternion.identity);
						Construction = newPath.GetComponent<Construction_Script>();
						CurrentNewBuilding = newPath;

						Construction.ConstructionType = ConstructionTypes;
						Construction.Angle = BuildingAngle;

						if (!Set)
						{
							Construction.Required_Ore = OreForProject;
							Construction.Required_Crystal = CrystalsForProject;
							Construction.Required_Stops = StoprForProject;

							MainNewBuildSite = newPath;
							MainNewBuildSite.transform.name = "TheBase";
							Set = true;
						}
						else
						{
							Construction.Fake = true;
						}

						BuildingStopsList.Add(Construction.StopPoint1);
						BuildingStopsList.Add(Construction.StopPoint2);
						BuildingStopsList.Add(Construction.StopPoint3);
						BuildingStopsList.Add(Construction.StopPoint4);
					}

					if (name == "G")
					{
						var newPath = Instantiate(ExtraPath, square.Square.transform.position, Quaternion.identity);
						CurrentNewBuilding = newPath;
					}

					Building_System.BuildingGrid[PosX, PosZ].B_Types = BuildingTypes.Building;
					Building_System.BuildingGrid[PosX, PosZ].Object = MainNewBuildSite;
					Building_System.BuildingGrid[PosX, PosZ].Object.GetComponent<Construction_Script>().ExtraPaths.Add(CurrentNewBuilding);
				}
				}

				if(BuildingStopsList.Count > 0)
				{
					foreach (var stop1 in BuildingStopsList.ToArray())
					{
						var Pos1 = stop1.transform.position;
						var VectorPos1 = new Vector3(Pos1.x,0, Pos1.z);

						foreach (var stop2 in BuildingStopsList.ToArray())
						{
							var Pos2 = stop2.transform.position;
							var VectorPos2 = new Vector3(Pos2.x, 0, Pos2.z);

							var distance = Vector3.Distance(VectorPos1, VectorPos2);

							if(distance > 0 && distance < 6)
							{
								BuildingStopsList.Remove(stop1);
								BuildingStopsList.Remove(stop2);
							}
						}
					}
					MainNewBuildSite.GetComponent<Construction_Script>().RequiredStopsListPoints = new List<GameObject>(BuildingStopsList);
				}

				BuildingStopsList.Clear();

				var StopObj = this.GetComponent<Building_System>().Stops;
				var Toolstore = this.GetComponent<System_Script>().Toolstore;

				//create the stops
				for (int i = 0; i < StoprForProject; i++)
				{
					var NewStop = Instantiate(StopObj, Toolstore.transform.position, Quaternion.identity);
				}

				//trigger this as a short cut
				OnBackClicked();
			}
		}
	}
	
	//moving the buildingplans around
	public void MousePositionOnTerrain(Vector3 Pos)
	{
		var MouseX = (Mathf.Round(Pos.x / 12)) * 12;
		var MouseZ = (Mathf.Round(Pos.z / 12)) * 12;


		if (MousePosX == 0 && MousePosZ == 0)
		{
			MousePosX = (int)MouseX;
			MousePosZ = (int)MouseZ;
		}

		if (MousePosX != (int)MouseX || MousePosZ != (int)MouseZ)
		{
			if (MousePosZ > (int)MouseZ && ConstructionAngle != ConstructionAngle.Down)
			{
				ConstructionAngle = ConstructionAngle.Down;
				BuildingAngle = new Vector3(0, 90, 0);
			}

			if (MousePosZ < (int)MouseZ && ConstructionAngle != ConstructionAngle.Up)
			{
				ConstructionAngle = ConstructionAngle.Up;
				BuildingAngle = new Vector3(0, 270, 0);
			}

			if (MousePosX > (int)MouseX && ConstructionAngle != ConstructionAngle.Right)
			{
				ConstructionAngle = ConstructionAngle.Right;
				BuildingAngle = new Vector3(0, 180, 0);
			}

			if (MousePosX < (int)MouseX && ConstructionAngle != ConstructionAngle.Left)
			{
				ConstructionAngle = ConstructionAngle.Left;
				BuildingAngle = new Vector3(0, 0, 0);
			}
		}

		foreach (var item in BuildingSquareList.ToArray())
		{
			var X_ = item.X;
			var Z_ = item.Z;
			var XNew = item.X;
			var ZNew = item.Z;
			//var TileObject = Building_System.BuildingGrid[X_, Z_].B_Types;
			//var Name_ = item.Square.transform.name;

			//if (TileObject == BuildingTypes.Nothing)
			//{
			//	if(Name_ == "Y")
			//	{
			//	//	item.Square
			//	}
			//}
			//else
			//{

			//}

			if (ConstructionAngle == ConstructionAngle.Up)
			{
			}

			if (ConstructionAngle == ConstructionAngle.Down)
			{
				XNew = item.X * -1;
				ZNew = item.Z * -1;
			}

			if (ConstructionAngle == ConstructionAngle.Left)
			{
				XNew = item.Z;
				ZNew = item.X;
			}

			if (ConstructionAngle == ConstructionAngle.Right)
			{
				XNew = item.Z * -1;
				ZNew = item.X * -1;
			}

			item.Square.transform.position = new Vector3(MouseX + (XNew * 12), 0.1f, MouseZ + (ZNew * 12));
		}

		MousePosX = (int)MouseX;
		MousePosZ = (int)MouseZ;
	}

	//Create building item clicked on
	public void On_Click_ToolStore()
	{
		B_Type = BuildingGroundType.one_one;
		ConstructionTypes = ConstructionTypes.ToolStore;
		SetRequirements(0, 0, 0);
		CreateBuildingSquare(0, 0, 0);
		CreateBuildingSquare(0, -1, 1);
	}

	//Create building item clicked on
	//public void On_Click_TeleportPad()
	//{
	//	B_Type = BuildingGroundType.one_one;
	//	ConstructionTypes = ConstructionTypes.Teleportpad;
	//	SetRequirements(8, 0, 4);
	//	CreateBuildingSquare(0, 0, 0);
	//	CreateBuildingSquare(0, -1, 1);
	//}

	//Create building item clicked on
	public void On_Click_TeleportPad()
	{
		B_Type = BuildingGroundType.one_one;
		ConstructionTypes = ConstructionTypes.Teleportpad;
		SetRequirements(8, 0, 4);
		CreateBuildingSquare(0, 0, 0);
		//CreateBuildingSquare(1, 0, 0);

		CreateBuildingSquare(0, -1, 1);
		//CreateBuildingSquare(1, -1, 1);

	}

	public void On_Click_SupportStation()
	{
		B_Type = BuildingGroundType.one_one;
		ConstructionTypes = ConstructionTypes.SupportStation;
		SetRequirements(0, 2, 4);
		CreateBuildingSquare(0, 0, 0);
		CreateBuildingSquare(0, -1, 1);
	}


	// building plan types
	public void CreateBuildingSquare(int X, int Z, int Type)
	{
		if (Type == 0)
		{
			var New = new ConstructionSquare();
			var YellowS = Instantiate(BuildingSquare_Y, new Vector3(0, 0, 0), Quaternion.identity);
			YellowS.transform.name = "Y";

			New.Square = YellowS;
			New.X = X;
			New.Z = Z;
			BuildingSquareList.Add(New);
		}

		if (Type == 1)
		{
			var New = new ConstructionSquare();
			var GreenS = Instantiate(BuildingSquare_G, new Vector3(0, 0, 0), Quaternion.identity);
			GreenS.transform.name = "G";

			New.Square = GreenS;
			New.X = X;
			New.Z = Z;
			BuildingSquareList.Add(New);
		}


	}

	public void OnBackClicked()
	{
		if (BuildingSquareList.Count > 0)
		{
			foreach (var i in BuildingSquareList.ToArray())
			{
				BuildingSquareList.Remove(i);
				Destroy(i.Square);
			}
		}

		B_Type = BuildingGroundType.blank;
	}

	public void SetRequirements(int ore, int cry, int stop)
	{
		OreForProject = ore;
		CrystalsForProject = cry;
		StoprForProject = stop;
	}
}
