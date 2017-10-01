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

	public Image CrystalsStrip;
	public Image OreStrip;

	public System_Script Sys_Script;

	// Use this for initialization
	void Start () {
		Sys_Script = GameObject.Find("System").GetComponent<System_Script>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		CrystalsStrip.rectTransform.sizeDelta = new Vector2(CrystalsStrip.rectTransform.sizeDelta.x,Sys_Script.CrystalsCollectedCart * 34);
		OreStrip.rectTransform.sizeDelta = new Vector2(OreStrip.rectTransform.sizeDelta.x, Sys_Script.OreCollectedCart * 34);
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
