using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum Type
{
	Man,
	Building,
	Nothing
}


public class SelectCode : MonoBehaviour
{
	private bool isSelected;
	public System_Script System_Script;
	public bool Movable = false;
	public Vector2 GameObjectCoordinates = Vector2.zero;
	public Image HiSelection;
	public float HiSelectionBoxSize = 40;
	public bool Selectable = true;
	public Type ObjectType = Type.Man;
	public bool IsSelected
	{
		get { return isSelected; }
		set
		{
			isSelected = value;

			var sel = System_Script.SelectedGameObjects;

			if (isSelected)
			{
				if (!sel.Contains(this))
				{
					sel.Add(this);
				}
			}
			else
			{
				sel.Remove(this);
			}
		}
	}

	void Start()
	{
		System_Script.AllSelectableGameObjects.Add(this);
		System_Script = GameObject.Find("System").GetComponent<System_Script>();
	}

	void OnDestroy()
	{
		System_Script.AllSelectableGameObjects.Remove(this);
	}

	void Update()
	{
		GameObjectCoordinates = this.transform.position;

		if (HiSelection != null)
		{
			HiSelection.transform.position = Camera.main.WorldToScreenPoint(this.transform.position);
			HiSelection.rectTransform.sizeDelta = new Vector2(HiSelectionBoxSize, HiSelectionBoxSize);
		}

		if (!isSelected && HiSelection != null)
		{
			Destroy(HiSelection.gameObject);
		}

	}

	public void OnMouseOver()
	{
		if (Input.GetMouseButtonUp(0) && System_Script.Is_Selection_Enabled())
		{
			System_Script.SingleSelection(this);
		}
	}
}
