using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeInput : MonoBehaviour {

	[SerializeField]
	GameObject shockwavePrefab;

	float timeInput;
	float shockwaveLife;
	float maxInputTime;
	float deltaDistance;
	float previewInputTime;
	float secondsPerDistance = 0.3f;
	Text cursorText;
	Vector3 cursorPosition;
	Vector3 cursorScreenPosition;
	Vector3 deltaPosition;
	RobotBehaviour selectedRB;

	void Awake(){
		cursorText = GameObject.Find("CursorText").GetComponent<Text>();
		shockwaveLife = shockwavePrefab.GetComponent<ShockwaveBehaviour>().intendedLifetime;
		HideCursorText();
	}

	public void HideCursorText(){
        Debug.Log(cursorText);
        cursorText.text = "";
		Cursor.visible = true;
	}

	public float SetAndDisplay(int selectedRobotIndex, GameObject selectedRobot, Command.AvailableCommands selectedCommand, List<List<MovingTrail>> robotMovingTrails){

		Cursor.visible = false;

		if (selectedRobot != null && selectedCommand != Command.AvailableCommands.None){
			selectedRB = selectedRobot.GetComponent<RobotBehaviour>();
			cursorPosition = Input.mousePosition;
			cursorScreenPosition = Camera.main.ScreenToWorldPoint(cursorPosition);

			if (robotMovingTrails[selectedRobotIndex].Count > 0){
				deltaPosition = cursorScreenPosition - robotMovingTrails[selectedRobotIndex].Last().Node.transform.position;
			} else {
				deltaPosition = cursorScreenPosition - selectedRobot.transform.position;
			}
			deltaDistance = Mathf.Sqrt(Mathf.Pow(deltaPosition.x, 2) + Mathf.Pow(deltaPosition.y, 2));

			previewInputTime = secondsPerDistance * deltaDistance;

			if (selectedCommand == Command.AvailableCommands.Push){
				//add a bit of time to make sure the shockwave dies before pausing
				maxInputTime = selectedRB.freeTime - (shockwaveLife + 0.1f);
				if (maxInputTime < 0){
					maxInputTime = 0;
				}
			} else {
				maxInputTime = selectedRB.freeTime;
			}
			if (previewInputTime <= maxInputTime){
				timeInput = previewInputTime;
			} else {
				timeInput = maxInputTime;
			}
			cursorText.text = System.Math.Round(timeInput, 2).ToString();
			cursorText.transform.position = cursorPosition;
		}
		return timeInput;
	}
}
