using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ToolBox : MonoBehaviour { //Not a true toolbox? http://wiki.unity3d.com/index.php/Toolbox http://stackoverflow.com/questions/5985661/methods-inside-enum-in-c-sharp

	public bool MatchOver; //For endscreen
	public ColorBlock ButtonColors; //For easy setting of all ingame button colors
	public Color32 LeftTeamColor;
	public Color32 RightTeamColor;

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