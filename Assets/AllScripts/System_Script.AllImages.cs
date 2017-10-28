using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

partial class System_Script
{
	public Text CreateMan_Icon_Text;
	public Image Menu_1_Background;
	public Image Menu_2_Background;
	public Image Menu_3_Background;
	public Image Menu_4_Background;
	public Image Menu_5_Background;
	public Image Menu_6_Background;
	public Image Menu_7_Background;
	public Image Menu_8_Background;
	public Image Menu_9_Background;
	public Image Menu_10_Background;




	public Material Yellow;
	public Material Red_1;
	public Material Green;




	public List<GameObject> Menus = new List<GameObject>();

	public void Initialise()
	{
		Menus.AddRange(new[] {
			Menu_1_Background.gameObject,
			Menu_2_Background.gameObject,
			Menu_3_Background.gameObject,
			Menu_4_Background.gameObject,
			Menu_5_Background.gameObject,
			Menu_6_Background.gameObject,
			Menu_7_Background.gameObject,
			Menu_8_Background.gameObject,
			Menu_9_Background.gameObject,
			Menu_10_Background.gameObject,


		});
	}
}

