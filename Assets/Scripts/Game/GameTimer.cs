using UnityEngine;
using System.Collections;

public class GameTimer {

	public int remainingTime;
	public bool InOvertime { get; private set; }

	int matchTime;

    public GameTimer(int totalMatchTime){
        matchTime = totalMatchTime;
        remainingTime = matchTime;
    }

    public IEnumerator CountDownSeconds(int seconds){
        while (seconds > 0){
            if (remainingTime > 0){
                yield return new WaitForSeconds(1f);
                seconds--;
                remainingTime--;
            }
            else {
                break;
            }
        }
    }
    
    /// <summary>
    /// adds overtime to the current remaining time
    /// </summary>
    public void AddOvertime(int overtime){
        remainingTime += overtime;
        InOvertime = true;
    }

    /// <summary>
    /// called from the outside to check if the matchtime has run out
    /// </summary>
    public bool NoRemainingTime(){ //if remaining time > 0 then false else true
        return remainingTime > 0 ? false : true;
    }

    public int MinutesRemaining(){
        return remainingTime / 60;
    }

    public int SecondsRemaining(){
        return remainingTime % 60;
    }
}
