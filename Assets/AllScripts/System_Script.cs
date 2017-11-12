using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;

public partial class System_Script : MonoBehaviour
{
	public static List<SelectCode> AllSelectableGameObjects = new List<SelectCode>();
	public static List<SelectCode> SelectedGameObjects = new List<SelectCode>();
	public static List<GameObject> ListOfAllToolStores = new List<GameObject>();
	public static List<GameObject> ListOfAllPowerStations = new List<GameObject>();
	public static List<GameObject> AllBuildings = new List<GameObject>();
	public static List<GameObject> AllWorkers = new List<GameObject>();
	public static List<GameObject> AllVehicles = new List<GameObject>();

	//actual collectable gameobjects currently in the game being played
	public static List<GameObject> AllCrystals = new List<GameObject>();
	public static List<GameObject> AllOre = new List<GameObject>();
	public static List<GameObject> AllStops = new List<GameObject>();

	//larger projects that are selected and set for raiders to work on.
	public static List<GameObject> DrillRocks = new List<GameObject>();
	public static List<GameObject> ClearRubble = new List<GameObject>();
	public static List<GameObject> ConstructionSites = new List<GameObject>();

	// all collectables that are free, as in not assigned to any workers
	public static List<GameObject> CollectableOre = new List<GameObject>();
	public static List<GameObject> CollectableCrystals = new List<GameObject>();
	public static List<GameObject> CollectableStops = new List<GameObject>();
	public GameObject Light_;


	public List<Image> Images = new List<Image>();

	public GameObject ParentImage;
	public static List<GameObject> RaidersList = new List<GameObject>();


	public bool selecting = false;
	private Vector3 mouseScreenStart = Vector2.zero;
	private Vector3 mouseScreenCurrent = Vector2.zero;

	public Image SelectionBoxImage;
	public Image HighlightedSelection;

	public UnitType FocusedObject = UnitType.Nothing;

	public GameObject Toolstore;
	public GameObject Lego_Raider;
	public GameObject Ore;

	public int MaxRaiders = 8;
	public int CreateRaiderQue = 0;

	public int CurrentMenuBarNumber = 3;
	public Game_Script Game_Script;
	public IconEdit_Script Icon_Script;


	public int CrystalsCollectedCart = 0;
	public int OreCollectedCart = 0;

	public GameObject selectedGameObject;

	public GameObject Rubble;

	public Building_System Building_System;
	public int TotalStopsNeeded = 0;
	public StartConstruction StartConstruction;
	public bool StartClickUI = false;

	void Start()
	{
		Game_Script = this.GetComponent<Game_Script>();
		Icon_Script = this.GetComponent<IconEdit_Script>();
		Building_System = this.GetComponent<Building_System>();
		StartConstruction = this.GetComponent<StartConstruction>();

		Initialise();
	}

	void Update()
	{
		var MouseIsOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();

		if (Input.GetMouseButtonDown(0))
		{
			OnLeftClick(MouseIsOverUI);
			StartConstruction.OnClick_Left_Down(MouseIsOverUI);
		}

		if (Input.GetMouseButtonUp(0))
		{
			OnMouseUpCalled(MouseIsOverUI);
		}
		var IMP = Input.mousePosition;
		var MousePos = Camera.main.ScreenToWorldPoint(new Vector3(IMP.x, IMP.y+300, IMP.z));
		//Light_.transform.position = new Vector3(MousePos.x,20, MousePos.z); 

		GetTotalStops();

		if (Is_Selection_Enabled())
		{
			SelectObjects();
		}

		foreach (var obj in AllSelectableGameObjects.ToArray())
		{
			var HaveSelectionBox = obj.GetComponent<SelectCode>().HiSelection;

			//Create selection box over units
			if (obj.IsSelected && HaveSelectionBox == null)
			{
				var NewImage = Instantiate(HighlightedSelection) as Image;
				NewImage.transform.SetParent(ParentImage.transform);
				Images.Add(NewImage);
				obj.GetComponent<SelectCode>().HiSelection = NewImage;
			}
		}

		if (MaxRaiders <= RaidersList.Count)
		{
			Icon_Script.Enable_CreatMan_I(false);
		}
		else
		{
			Icon_Script.Enable_CreatMan_I(true);
		}

		CreateMan_Icon_Text.text = CreateRaiderQue.ToString();

		for (int i = 0; i < Menus.Count; i++)
		{
			Menus[i].SetActive(i == CurrentMenuBarNumber - 1);
		}

		if (SelectedGameObjects.Count > 0)
		{
			var SObject = SelectedGameObjects[0].GetComponent<AddToSystemList_Script>();
			var SObject_Core = SelectedGameObjects[0].GetComponent<Lego_Character>();

			if (SObject.IsRaider && CurrentMenuBarNumber != 2)
			{
				ChangeMenu(2);
			}

			if (SObject.IsBuilding)
			{
				ChangeMenu(7);
			}

			if (SObject.IsVehicle && !SObject_Core.DriverIsSeated)
			{
				ChangeMenu(10);
			}

			if (SObject.IsVehicle && SObject_Core.Driver != null && SObject_Core.DriverIsSeated)
			{
				ChangeMenu(9);
			}
		}

		Update2();
	}

	public void OnMouseUpCalled(bool MouseIsOverUI)
	{
		RaycastHit dest;
		var ScanDeeper = true;
		var EndClickUI = MouseIsOverUI;

		mouseScreenStart = Vector2.zero;
		mouseScreenCurrent = Vector2.zero;
		SelectionBoxImage.rectTransform.sizeDelta = Vector2.zero;

		if (!MouseIsOverUI)
		{
			if (Is_NewMenu_Enabled() && !selecting)
			{
				// first scan the tops only
				Ray ray0 = Camera.main.ScreenPointToRay(Input.mousePosition);

				if (Physics.Raycast(ray0, out dest))
				{
					OnLeftClick(dest);

					if (dest.collider.gameObject.transform.tag == "Rock")
					{
						ScanDeeper = false;
					}
				}

				if (ScanDeeper == true)
				{
					var SelectorSize = Building_System.SelectorSize;

					if (Physics.Raycast(ray0, out dest, float.MaxValue, LayerMask.GetMask("Terrain")))
					{
						var P = dest.point;
						var X_ = (Mathf.Round(P.x / SelectorSize)) * SelectorSize;
						var Z_ = (Mathf.Round(P.z / SelectorSize)) * SelectorSize;
						Building_System.Clicked_X = (int)X_ / SelectorSize;
						Building_System.Clicked_Z = (int)Z_ / SelectorSize;

						if (dest.collider.gameObject.transform.tag == "Terrain")
						{
							//DeSelectAll();

							Building_System.SelectorSquare.transform.position = new Vector3(X_, 0.1f, Z_);
							Building_System.CurrentBuildingType = Building_System.BuildingGrid[Building_System.Clicked_X, Building_System.Clicked_Z].B_Types;
							Building_System.CurrentObject = Building_System.BuildingGrid[Building_System.Clicked_X, Building_System.Clicked_Z].Object;
							Building_System.SelectorSquare.SetActive(true);

							Building_System.On_Click();
						}
					}
				}
			}
		}

		selecting = false;
	}

	public void OnLeftClick(bool MouseIsOverUI)
	{
		mouseScreenStart = Input.mousePosition;
		StartClickUI = MouseIsOverUI;

		if (!MouseIsOverUI)
		{
			DeSelectAll();
		}
	}

	private void SelectObjects()
	{
		float selectingDistance;
		//Drag Box Selection

		if (Input.GetMouseButton(0))
		{
			if (mouseScreenStart != Vector3.zero)
			{
				mouseScreenCurrent = Input.mousePosition;
				selectingDistance = Vector3.Distance(mouseScreenCurrent, mouseScreenStart);

				if (selectingDistance > 1)
				{
					selecting = true;
				}
			}
		}

		if (selecting)
		{
			var minX = Mathf.Min(mouseScreenStart.x, mouseScreenCurrent.x);
			var minY = Mathf.Min(mouseScreenStart.y, mouseScreenCurrent.y);
			var maxX = Mathf.Max(mouseScreenStart.x, mouseScreenCurrent.x);
			var maxY = Mathf.Max(mouseScreenStart.y, mouseScreenCurrent.y);

			//Selectng the man.Where(x => x.ObjectType == FocusedObject || x.ObjectType == Type.Nothing)
			foreach (var obj in AllSelectableGameObjects)
			{
				var point = Camera.main.WorldToScreenPoint(obj.transform.position);
				var contains = minX < point.x && minY < point.y && maxX > point.x && maxY > point.y;

				if (obj.Selectable)
				{
					obj.IsSelected = contains;
				}
			}

			SelectionBoxImage.rectTransform.sizeDelta = new Vector2(Mathf.Abs(maxX - minX), Mathf.Abs(maxY - minY));
			SelectionBoxImage.transform.position = new Vector3(minX, minY);
		}
	}

	public void Update2()
	{
		RaycastHit dest;

		if (Input.GetMouseButtonDown(1))
		{
			Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out dest);
			var destination = dest.point;

			foreach (var obj in SelectedGameObjects)
			{
				//check if the unit selected can be moved before trying
				if (obj.Movable == true)
				{
					obj.GetComponent<Lego_Character>().MoveTo(destination);
				}
			}
			OnRightClick(dest);
		}
	}

	public static void SingleSelection(SelectCode obj)
	{
		foreach (var sel in SelectedGameObjects.ToArray())
			sel.IsSelected = false;

		if (obj.Selectable)
		{
			obj.IsSelected = true;
		}
	}

	//create raiders
	public void OnClick_CreateMan()
	{
		if ((CreateRaiderQue + RaidersList.Count) < MaxRaiders)
		{
			CreateRaiderQue++;
		}
	}

	public void OnClick_Back()
	{
		Building_System.CurrentBuildingType = BuildingTypes.Nothing;
		Building_System.CurrentObject = null;
		ChangeMenu(1);
		Building_System.SelectorSquare.SetActive(false);

		DeSelectAll();
		StartConstruction.OnBackClicked();
	}

	public void OnClick_Buid()
	{
		ChangeMenu(3);
	}

	public void OnRightClick(RaycastHit Point)
	{
		var hasParent = Point.transform.tag == "Rock" || Point.transform.tag == "Crystal" || Point.transform.tag == "Ore";
		var Collectable = false;
		var TaskAvaliable = true;
		Collectable CollectableObjectScript = new Collectable();
		GameObject ClickedObject = new GameObject();
		var Drillable = false;

		if (hasParent)
		{
			ClickedObject = Point.collider.gameObject.transform.parent.gameObject;
		}
		else
		{
			ClickedObject = Point.collider.gameObject;
		}

		if (ClickedObject.GetComponent<Collectable>() != null)
		{
			CollectableObjectScript = ClickedObject.GetComponent<Collectable>();
			Collectable = true;
			CollectableObjectScript.HostChanged = false;
		}

		if (Point.transform.tag == "Rock")
		{
			var Rock_Type = ClickedObject.GetComponent<Work_Script>().RockProperties.RockType;
			Drillable = (Rock_Type == RockType.LooseRock || Rock_Type == RockType.SoftRock || Rock_Type == RockType.HardRock);
		}

		if (Point.transform.tag == "Terrain")
		{
			TaskAvaliable = false;
		}

		var PositionOfObject = new Vector3(ClickedObject.transform.position.x, 0, ClickedObject.transform.position.z);

		foreach (var Unit in SelectedGameObjects)
		{
			var Unit_ = Unit.GetComponent<Lego_Character>();
			Unit_.SetToDefault();
			Unit_.Arrived = false;

			if (TaskAvaliable)
			{
				if (Unit_.Items.Count > 0)
				{
					Unit_.Items[0].GetComponent<Collectable>().DropItem();
					Unit_.Items.Clear();
				}

				Unit_.TaskObject = ClickedObject;
				Unit_.MoveTo(PositionOfObject);
			}
			else
			{
				if (Unit_.Items.Count > 0)
				{
					Unit_.CurrentTask = CurrentJob.WanderAroungWithItem;
				}
				else
				{
					Unit_.CurrentTask = CurrentJob.WalkToPoint;
				}

				Unit_.MoveTo(Point.point);
			}

			if (Point.transform.tag == "Rubble")
			{
				Unit_.StartClearingProcess();
			}

			if (ClickedObject.transform.tag == "Rock")
			{
				if (Unit_.CanDrill && Drillable)
				{
					Unit_.StartDrillingProcess();
				}
			}

			if (Collectable)
			{
				TaskAvaliable = false;
				//new host not been set
				if (CollectableObjectScript.HostChanged == false)
				{
					//if another raider is going for it then make that raider drop it
					if (CollectableObjectScript.Collector != null)
					{
						CollectableObjectScript.ClearAllCollectors();
					}

					Unit_.StartCollecting();
					CollectableObjectScript.Collector = Unit_.gameObject;

					if (Point.transform.tag == "Crystal")
					{
						Unit_.ItemType = CollectableType.Crystal;
						Unit_.TaskChassis = TaskChassis.GatherCrystals;
					}

					if (Point.transform.tag == "Ore")
					{
						Unit_.ItemType = CollectableType.Ore;
						Unit_.TaskChassis = TaskChassis.GatherOre;
					}

					CollectableObjectScript.HostChanged = true;
				}
			}
		}
	}

	// currently only used for clicking on rocks
	public void OnLeftClick(RaycastHit Point)
	{
		var taskable = false;
		var Object = "";
		Object = Point.transform.tag;

		if (Is_NewMenu_Enabled())
		{
			if (Object == "Rock")
			{
				taskable = true;
				Object = Point.transform.tag;

				selectedGameObject = Point.collider.transform.parent.gameObject;
			}

			//for objects with colliders below parents
			if (taskable)
			{
				if (Object == "Rock")
				{
					var RockShape = selectedGameObject.GetComponent<Work_Script>().RockProperties.RockShape;

					if (RockShape != RockShape.Square && RockShape != RockShape.SquareBottomLeft && RockShape != RockShape.SquareBottomRight && RockShape != RockShape.SquareTopLeft && RockShape != RockShape.SquareTopRight)
					{

						ChangeMenu(4);

						if (selectedGameObject.GetComponent<Work_Script>().WorkedOn)
						{
							Icon_Script.Enable_Drill(false);
						}
						else
						{
							Icon_Script.Enable_Drill(true);
						}
					}
					else
					{
						ChangeMenu(1);
					}
				}
			}
		}
	}

	//Work on Code
	public void Onclick_Drill()
	{
		if (selectedGameObject.GetComponent<Work_Script>().WorkedOn == false)
		{
			selectedGameObject.GetComponent<Work_Script>().WorkedOn = true;
			DrillRocks.Add(selectedGameObject);
		}
		ChangeMenu(1);
	}

	public void Onclick_CreateSmallVehicle()
	{
		ChangeMenu(8);
	}

	public void OnClick_ClearRubble()
	{
		if (selectedGameObject.GetComponent<Work_Script>().WorkedOn == false)
		{
			selectedGameObject.GetComponent<Work_Script>().WorkedOn = true;
			ClearRubble.Add(selectedGameObject);
		}
		ChangeMenu(1);
	}

	public void OnClick_Climbout()
	{
		var DiverOfTheVehicle = SelectedGameObjects[0].GetComponent<Lego_Character>().Driver;
		var car = SelectedGameObjects[0].GetComponent<Lego_Character>();
		DiverOfTheVehicle.GetComponent<Lego_Character>().SetToDefault();
		car.RaiderGotOutOfVehicle();
		OnClick_Back();
	}

	public void OnClick_CallRaider()
	{
		SelectedGameObjects[0].GetComponent<Lego_Character>().CallForDriver = true;
		OnClick_Back();
	}

	public void DropItem()
	{
		if (SelectedGameObjects.Count > 0)
		{
			foreach (var Unit in SelectedGameObjects)
			{
				var Unit_ = Unit.GetComponent<Lego_Character>();

				if (Unit_.Items.Count > 0)
				{
					Unit_.Items[0].GetComponent<Collectable>().DropItem();
					Unit_.Items.Clear();
					Unit_.TaskChassis = TaskChassis.Nothing;
					Unit_.CurrentTask = CurrentJob.Nothing;
					Unit_.DistFromJob = float.MaxValue;
					Unit_.TaskObject = null;
				}
			}
		}
	}

	public void GetTotalStops()
	{
		TotalStopsNeeded = 0;

		foreach (var item in ConstructionSites.ToArray())
		{
			var script = item.GetComponent<Construction_Script>();
			var stopsNo = script.Required_Stops - (script.Contained_Stops + script.Workerlist_Stops.Count);

			TotalStopsNeeded = +stopsNo;
		}
	}

	public void DeSelectAll()
	{
		foreach (var obj in AllSelectableGameObjects.ToArray())
		{
			obj.IsSelected = false;
		}
		SelectedGameObjects.Clear();

		Building_System.CurrentBuildingType = BuildingTypes.Nothing;
		Building_System.CurrentObject = null;
		Building_System.SelectorSquare.SetActive(false);
	}

	public bool Is_NewMenu_Enabled()
	{
		if (StartConstruction.BuildingSquareList.Count == 0)
		{
			return (true);
		}

		return (false);
	}

	public bool Is_Selection_Enabled()
	{
		if (StartConstruction.BuildingSquareList.Count == 0)
		{
			return (true);
		}

		return (false);
	}

	public void ChangeMenu(int Number)
	{
		CurrentMenuBarNumber = Number;
	}

	public void TeleportUint()
	{
		foreach (var unit in SelectedGameObjects)
		{
			DropItem();
			Destroy(unit);
		}
	}
}
