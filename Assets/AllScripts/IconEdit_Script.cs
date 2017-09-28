using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class IconEdit_Script : MonoBehaviour {

	public Button CreateMan_Icon;
	public Button Drill_Icon;
	public Button PowerPath_Icon;
	public Button ClearRubble_Icon;
	public Sprite ClearRubble_Icon_D;
	public Sprite ClearRubble_Icon_E;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Enable_CreatMan_I(bool order)
	{
		CreateMan_Icon.interactable = order;
	}

	public void Enable_Drill(bool order)
	{
		Drill_Icon.interactable = order;

		if(order)
		{
			ClearRubble_Icon.GetComponent<Image>().sprite = ClearRubble_Icon_E;
		}
		else
		{
			ClearRubble_Icon.GetComponent<Image>().sprite = ClearRubble_Icon_D;
		}
	}

	public void Enable_Rubble(bool order)
	{
		ClearRubble_Icon.interactable = order;

		if (order)
		{
			ClearRubble_Icon.GetComponent<Image>().sprite = ClearRubble_Icon_E;
		}
		else
		{
			ClearRubble_Icon.GetComponent<Image>().sprite = ClearRubble_Icon_D;
		}
	}

}
