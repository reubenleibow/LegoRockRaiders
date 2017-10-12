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
	Nothing
}

public class StartConstruction : MonoBehaviour {

	public List<GameObject> BuildingSquareList = new List<GameObject>();
	public GameObject BuildingSquare_Y;
	public GameObject BuildingSquare_G;
	public GameObject ExtraPath;
	public GameObject BasePath;


	public BuildingGroundType B_Type = BuildingGroundType.blank;
	public ConstructionTypes ConstructionTypes = ConstructionTypes.Nothing;

	public int OreForProject = 0;
	public int CrystalsForProject = 0;
	public int StoprForProject = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		RaycastHit dest;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out dest, float.MaxValue, LayerMask.GetMask("Terrain")))
		{
			if(dest.collider.gameObject.transform.tag == "Terrain")
			{
				MousePositionOnTerrain(dest.point);
			}
		}

		if (B_Type != BuildingGroundType.blank)
		{
			var Construction = new Construction_Script();

			if (Input.GetMouseButtonDown(0))
			{
				foreach (var square in BuildingSquareList.ToArray())
				{
					var name = square.transform.name;

					if (name == "G")
					{
						var newPath = Instantiate(ExtraPath, square.transform.position, Quaternion.identity);
					}

					if (name == "Y")
					{
						var newPath = Instantiate(BasePath, square.transform.position, Quaternion.identity);
						Construction = newPath.GetComponent<Construction_Script>();

						Construction.ConstructionType = ConstructionTypes;

						Construction.Required_Ore = OreForProject;
						Construction.Required_Crystal = CrystalsForProject;
						Construction.Required_Stops = StoprForProject;
					}
				}


				var StopObj = this.GetComponent<Building_System>().Stops;
				var Toolstore = this.GetComponent<System_Script>().Toolstore;

				for (int i = 0; i < StoprForProject; i++)
				{
					var NewStop = Instantiate(StopObj, Toolstore.transform.position, Quaternion.identity);
				}

				OnBackClicked();
			}
		}
	}

	public void MousePositionOnTerrain(Vector3 Pos)
	{
		var MouseX = (Mathf.Round(Pos.x / 12)) * 12;
		var MouseZ = (Mathf.Round(Pos.z / 12)) * 12;

		if (B_Type == BuildingGroundType.one_one)
		{
			foreach(var square in BuildingSquareList.ToArray())
			{
				var name = square.transform.name;

				if(name == "G")
				{
					square.transform.position = new Vector3(MouseX, 0.1f, MouseZ + 12);
				}

				if (name == "Y")
				{
					square.transform.position = new Vector3(MouseX, 0.1f, MouseZ );
				}
			}
		}
	}

	public void On_Click_ToolStore()
	{
		B_Type = BuildingGroundType.one_one;
		ConstructionTypes = ConstructionTypes.ToolStore;
		CreateBuildingSquare(1, 1);
		SetRequirements(0,0,0);
	}

	public void On_Click_TeleportPad()
	{
		B_Type = BuildingGroundType.one_one;
		ConstructionTypes = ConstructionTypes.Teleportpad;
		CreateBuildingSquare(1, 1);
		SetRequirements(8, 0, 4);
	}

	public void CreateBuildingSquare(int Green, int Yellow)
	{
		for (int i = 0; i < Green; i++)
		{
			var GreenS = Instantiate(BuildingSquare_G, new Vector3(0, 0, 0), Quaternion.identity);
			GreenS.transform.name = "G";
			BuildingSquareList.Add(GreenS);
		}

		for (int i = 0; i < Yellow; i++)
		{
			var YellowS = Instantiate(BuildingSquare_Y, new Vector3(0, 0, 0), Quaternion.identity);
			YellowS.transform.name = "Y";
			BuildingSquareList.Add(YellowS);
		}
	}

	public void OnBackClicked()
	{
		if(BuildingSquareList.Count > 0)
		{
			foreach(var i in BuildingSquareList.ToArray())
			{
				BuildingSquareList.Remove(i);
				Destroy(i);
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
