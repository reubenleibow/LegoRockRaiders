using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

partial class System_Script
{
	public Button CreateMan_Icon;
	public Text CreateMan_Icon_Text;
	public Image Menu_1_Background;
	public Image Menu_2_Background;
	public Image Menu_3_Background;
	public Image Menu_4_Background;
	public Image Menu_5_Background;


	public Button Drill_Icon;
	public Button PowerPath_Icon;
	public Button ClearRubble_Icon;
	public Button ClearRubbleE_Icon;



	public List<GameObject> Menus = new List<GameObject>();

	public void Initialise()
	{
		Menus.AddRange(new[] {
			Menu_1_Background.gameObject,
			Menu_2_Background.gameObject,
			Menu_3_Background.gameObject,
			Menu_4_Background.gameObject,
			Menu_5_Background.gameObject,



		});
	}
}

