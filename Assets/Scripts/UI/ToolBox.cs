using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolBox : MonoBehaviour { //Currently not a true toolbox, just a simple singleton http://wiki.unity3d.com/index.php/Toolbox http://stackoverflow.com/questions/5985661/methods-inside-enum-in-c-sharp

	public ColorBlock buttonColors; //For easy setting of all ingame button colors
	public Color32 leftTeamColor;
	public Color32 rightTeamColor;

	public static ToolBox Instance = null;

	void Awake(){
		if (Instance == null)
			Instance = this;
		else if (Instance != this){ //Makes sure there's only one instance of the script
			Destroy(gameObject); //Goes nuclear
		}
		DontDestroyOnLoad(gameObject);
	}
}