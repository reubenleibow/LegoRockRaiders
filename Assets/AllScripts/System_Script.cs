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
	public static List<GameObject> AllCrystals = new List<GameObject>();
	public static List<GameObject> AllOre = new List<GameObject>();
	public static List<GameObject> DrillRocks = new List<GameObject>();
	public static List<GameObject> ClearRubble = new List<GameObject>();



	public static List<GameObject> CollectableOre = new List<GameObject>();
	public static List<GameObject> CollectableCrystals = new List<GameObject>();

	public List<Image> Images = new List<Image>();

	public GameObject ParentImage;
	public static List<GameObject> RaidersList = new List<GameObject>();


	private bool selecting = false;
	private Vector3 mouseScreenStart = Vector2.zero;
	private Vector3 mouseScreenCurrent = Vector2.zero;

	public Image SelectionBoxImage;
	public Image HighlightedSelection;

	public Type FocusedObject = Type.Nothing;

	public GameObject Toolstore;
	public GameObject Lego_Raider;
	public GameObject SelectorSquare;

	public int MaxRaiders = 8;
	public int CreateRaiderQue = 0;

	public int CurrentMenuBarNumber = 3;
	public Game_Script Game_Script;

	public int CrystalsCollectedCart = 0;
	public int OreCollectedCart = 0;

	public GameObject selectedGameObject;

	public GameObject Rubble;



	void Start()
	{
		Game_Script = this.GetComponent<Game_Script>();
		Initialise();
	}

	void Update()
	{
		SelectObjects();

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
			CreateMan_Icon.interactable = false;
		}
		else
		{
			CreateMan_Icon.interactable = true;
		}

		CreateMan_Icon_Text.text = CreateRaiderQue.ToString();

		for (int i = 0; i < Menus.Count; i++)
		{
			Menus[i].SetActive(i == CurrentMenuBarNumber - 1);
		}

		if (SelectedGameObjects.Count > 0)
		{
			CurrentMenuBarNumber = 2;
		}
		else if (CurrentMenuBarNumber == 2)
		{
			CurrentMenuBarNumber = 1;
		}

		
		//---------------------------WorkOn
		if(selectedGameObject != null || SelectedGameObjects.Count != 0)
		{
			SelectorSquare.SetActive(false);
		}
	}

	private void SelectObjects()
	{
		//Drag Box Selection
		if (Input.GetMouseButtonDown(0))
		{
			mouseScreenStart = Input.mousePosition;
			//FocusedObject = Type.Nothing;
		}

		if (Input.GetMouseButton(0))
		{
			if (mouseScreenStart != Vector3.zero)
			{
				selecting = true;
				mouseScreenCurrent = Input.mousePosition;
			}
		}
		else
		{
			selecting = false;
			mouseScreenStart = Vector2.zero;
			mouseScreenCurrent = Vector2.zero;
			SelectionBoxImage.rectTransform.sizeDelta = Vector2.zero;
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

		//moving the unit to the destination;
		RaycastHit dest;

		if (Input.GetMouseButtonDown(1))
		{
			Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out dest);

			OnRightClick(dest);

			var destination = dest.point;

			OnClickAnyGameObject(dest);

			foreach (var obj in SelectedGameObjects)
			{
				//check if the unit selected can be moved before trying
				if (obj.Movable == true)
				{
					obj.GetComponent<NavMeshAgent>().SetDestination(destination);
				}
			}
		}

		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out dest))
			{
				OnLeftClick(dest);
			}

			if (selectedGameObject == null)
			{
				if (Physics.Raycast(ray, out dest, float.MaxValue, LayerMask.GetMask("Terrain")))
				{
					var P = dest.point;
					var X_ = (Mathf.Round(P.x / 12)) * 12;
					var Z_ = (Mathf.Round(P.z / 12)) * 12;

					SelectorSquare.transform.position = new Vector3(X_, 0.1f, Z_);
				}
			}

			if (selectedGameObject == null && SelectedGameObjects.Count == 0)
			{
				SelectorSquare.SetActive(true);
				CurrentMenuBarNumber = 5;

			}
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
		CurrentMenuBarNumber = 1;
	}

	public void OnClick_Buid()
	{
		CurrentMenuBarNumber = 3;
	}

	public void OnClickAnyGameObject(RaycastHit obj)
	{
		//if (obj.collider.transform.tag == "Rock")
		//{
		//	var rockParent = obj.collider.gameObject.transform.parent.gameObject;
		//	Game_Script.OnDestroyRock(rockParent.GetComponent<Rock_Script>().RockProperties);
		//
		//	Debug.Log(rockParent);
		//}
	}

	public void OnRightClick(RaycastHit Point)
	{
		var Taskable = Point.transform.tag == "Rock" || Point.transform.tag == "Crystal" || Point.transform.tag == "Ore";

		//Remove all tasked objects
		foreach (var Unit in SelectedGameObjects)
		{
			var Unit_ = Unit.GetComponent<Lego_Character>();
			Unit_.TaskChassis = TaskChassis.JWalking;
			Unit_.CurrentTask = CurrentJob.Nothing;


			Unit_.TaskObject = null;

			if (Unit_.Items.Count > 0)
			{
				Unit_.CurrentTask = CurrentJob.WanderAroungWithItem;
			}
		}

		if (Taskable)
		{
			var Object_ = Point.collider.gameObject.transform.parent.gameObject;
			var Collectable = Object_.GetComponent<Collectable>();
			var Drillable = false;

			if (Point.transform.tag == "Rock")
			{
				var Rock_Type = Object_.GetComponent<Work_Script>().RockProperties.RockType;

				Drillable = (Rock_Type == RockType.LooseRock || Rock_Type == RockType.SoftRock || Rock_Type == RockType.HardRock);
			}

			//part(1/3)
			if ((Point.transform.tag == "Crystal" || Point.transform.tag == "Ore"))
			{
				Collectable.HostChanged = false;
			}


			foreach (var Unit in SelectedGameObjects)
			{
				var Unit_ = Unit.GetComponent<Lego_Character>();
				var NavMesh = Unit.GetComponent<NavMeshAgent>();

				if (Point.transform.tag == "Rock")
				{
					if (Unit_.CanDrill && Drillable)
					{
						Unit_.TaskChassis = TaskChassis.Drilling;
						Unit_.CurrentTask = CurrentJob.WalkingToDrill;
						Unit_.DistFromJob = float.MaxValue;
						Unit_.TaskObject = Object_;
						//-----------------------------------------------------------------------------DODGYCODE
						if (Unit_.Items.Count > 0)
						{
							Unit_.Items[0].GetComponent<Collectable>().DropItem();
							Unit_.Items.Clear();
						}

					}
				}

				var collectable = Object_.GetComponent<Collectable>();

				//part(2/3)
				if ((Point.transform.tag == "Crystal" || Point.transform.tag == "Ore"))
				{

					//new host not been set
					if (collectable.HostChanged == false)
					{
						Unit_.TaskObject = Object_;

						//if another raider is going for it then make that raider drop it
						if (collectable.Collector != null)
						{
							collectable.ClearAllCollectors();
						}

						//If unit has an item in its hand then drop it.(edit here to make for multiple)
						if (Unit_.Items.Count > 0)
						{
							Unit_.Items[0].GetComponent<Collectable>().DropItem();
							Unit_.Items.Clear();
						}

						//set some values
						Unit_.CurrentTask = CurrentJob.WalkToCollectable;
						Unit_.DistFromJob = float.MaxValue;
						collectable.Collector = Unit_.gameObject;

						//part(3/3)
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

						//Set host changed to true
						collectable.HostChanged = true;
					}
				}

				NavMesh.SetDestination(Object_.transform.position);
			}
		}
	}

	public void OnRockDestroyed(GameObject Rock)
	{
		foreach (var Unit in RaidersList)
		{
			var character = Unit.GetComponent<Lego_Character>();

			if (character.TaskObject == Rock)
			{
				character.TaskObject = null;
				character.CurrentTask = CurrentJob.Nothing;

			}
		}
	}

	public void OnLeftClick(RaycastHit Point)
	{
		var taskable = false;
		var Object = "";
		selectedGameObject = null;
		Object = Point.transform.tag;

		if (Point.transform.tag == "Rock")
		{
			taskable = true;
			Object = Point.transform.tag;

			selectedGameObject = Point.collider.transform.parent.gameObject;
		}

		if(Point.transform.tag == "Rubble")
		{
			CurrentMenuBarNumber = 5;
			selectedGameObject = Point.collider.gameObject;
			Debug.Log("dasd");
		}

		if (taskable)
		{
			if (Object == "Rock")
			{
				CurrentMenuBarNumber = 4;

				if (selectedGameObject.GetComponent<Work_Script>().WorkedOn)
				{
					Drill_Icon.enabled = false;
				}
				else
				{
					Drill_Icon.enabled = true;
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

		CurrentMenuBarNumber = 1;
	}

	public void OnClick_ClearRubble()
	{
		if (selectedGameObject.GetComponent<Work_Script>().WorkedOn == false)
		{
			selectedGameObject.GetComponent<Work_Script>().WorkedOn = true;
			ClearRubble.Add(selectedGameObject);
		}

		CurrentMenuBarNumber = 1;
	}
}
