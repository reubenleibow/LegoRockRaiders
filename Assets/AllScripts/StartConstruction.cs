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

public class StartConstruction : MonoBehaviour {

	public List<ConstructionSquare> BuildingSquareList = new List<ConstructionSquare>();
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
					var name = square.Square.transform.name;

					if (name == "G")
					{
                        var newPath = Instantiate(ExtraPath, square.Square.transform.position, Quaternion.identity);
                    }

                    if (name == "Y")
                    {
                        var newPath = Instantiate(BasePath, square.Square.transform.position, Quaternion.identity);
                        Construction = newPath.GetComponent<Construction_Script>();

                        Construction.ConstructionType = ConstructionTypes;
                        Construction.Angle = BuildingAngle;

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


        if (MousePosX == 0 &&  MousePosZ == 0)
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

	public void On_Click_ToolStore()
	{
		B_Type = BuildingGroundType.one_one;
		ConstructionTypes = ConstructionTypes.ToolStore;
		SetRequirements(0,0,0);
		CreateBuildingSquare(0, 0,0);
        CreateBuildingSquare(0, -1,1);
    }

    public void On_Click_TeleportPad()
	{
		B_Type = BuildingGroundType.one_one;
		ConstructionTypes = ConstructionTypes.Teleportpad;
		SetRequirements(8, 0, 4);
        CreateBuildingSquare(0, 0, 0);
        CreateBuildingSquare(0, -1, 1);
    }

    public void CreateBuildingSquare(int X, int Z, int Type)
	{
        if(Type == 0)
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
		if(BuildingSquareList.Count > 0)
		{
			foreach(var i in BuildingSquareList.ToArray())
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
