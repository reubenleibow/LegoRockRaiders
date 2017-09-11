using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public partial class System_Script : MonoBehaviour
{
	public static List<SelectCode> AllSelectableGameObjects = new List<SelectCode>();
	public static List<SelectCode> SelectedGameObjects = new List<SelectCode>();
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

	public int MaxRaiders = 8;
	public int CreateRaiderQue = 0;

	public int CurrentMenuBarNumber = 3;
	public Game_Script Game_Script;


	void Start()
	{
		Game_Script = this.GetComponent<Game_Script>();
		Initialise();
	}

	void Update()
	{

		SelectObjects();

		foreach (var obj in AllSelectableGameObjects)
		{
			var HaveSelectionBox = obj.GetComponent<SelectCode>().HiSelection;

			//Create selection box over units
			if (obj.IsSelected && HaveSelectionBox == null)
			{
				var NewImage = Instantiate(HighlightedSelection) as Image;
				NewImage.transform.parent = ParentImage.transform;
				Images.Add(NewImage);
				obj.GetComponent<SelectCode>().HiSelection = NewImage;
			}
		}

		if(MaxRaiders <= RaidersList.Count)
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
			Menus[i].SetActive(i == CurrentMenuBarNumber-1);
		}

		if(SelectedGameObjects.Count > 0)
		{
			CurrentMenuBarNumber = 2;
		}
		else if(CurrentMenuBarNumber == 2)
		{
			CurrentMenuBarNumber = 1;
		}


		//if(SelectedGameObjects.Count == 1)
		//{
		//	FocusedObject = SelectedGameObjects[0].ObjectType;
		//}
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

				if(obj.Selectable)
				{
					obj.IsSelected = contains;
				}
			}

			SelectionBoxImage.rectTransform.sizeDelta = new Vector2(Mathf.Abs(maxX - minX), Mathf.Abs(maxY - minY));
			SelectionBoxImage.transform.position = new Vector3(minX, minY);
		}

		//moving the unit to the destination;
		RaycastHit dest;

		if(Input.GetMouseButtonDown(1))
		{
			Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out dest);

			OnRightClick(dest);

			var destination = dest.point;

			Debug.Log(dest.collider.gameObject);

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
		if((CreateRaiderQue + RaidersList.Count) < MaxRaiders)
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

		var Taskable = Point.transform.tag == "Rock" || Point.transform.tag == "Crystal";

		if (Taskable)
		{
			var Object_ = Point.collider.gameObject.transform.parent.gameObject;
			var Drillable = false;

			if (Point.transform.tag == "Rock")
			{
				var Rock_Type = Object_.GetComponent<Rock_Script>().RockProperties.RockType;

				Drillable = (Rock_Type == RockType.LooseRock || Rock_Type == RockType.SoftRock || Rock_Type == RockType.HardRock);
			}

			foreach (var Unit in SelectedGameObjects)
			{
				var Unit_ = Unit.GetComponent<Lego_Character>();
				var NavMesh = Unit.GetComponent<NavMeshAgent>();

				Unit_.TaskObject = Object_;

				if (Point.transform.tag == "Rock")
				{
					if (Unit_.CanDrill && Drillable)
					{
						Unit_.CurrentTask = CurrentJob.WalkingToDrill;
						Unit_.DistFromJob = float.MaxValue;
					}
				}

				if (Point.transform.tag == "Crystal" && Object_.GetComponent<Collectable>().Collector == null)
				{
					Unit_.CurrentTask = CurrentJob.WalkToCrystal;
					Unit_.DistFromJob = float.MaxValue;
					Object_.GetComponent<Collectable>().Collector = Unit_.gameObject;
				}

				NavMesh.SetDestination(Object_.transform.position);
			}
		}
	}

	public void OnRockDestroyed(GameObject Rock)
	{
		foreach(var Unit in RaidersList)
		{
			if(Unit.GetComponent<Lego_Character>().TaskObject == Rock)
			{
				Unit.GetComponent<Lego_Character>().TaskObject = null;
				Unit.GetComponent<Lego_Character>().CurrentTask = CurrentJob.Nothing;

			}
		}
	}
}
