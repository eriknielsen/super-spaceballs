using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadVolume : MonoBehaviour {

	public string volumeType;

	void Start () {
		GetComponent<Slider>().value = PlayerPrefs.GetFloat(volumeType);
		GetComponent<Slider>().colors = ToolBox.Instance.ButtonColors;
	}
}