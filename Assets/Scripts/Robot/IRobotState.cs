using UnityEngine;
using System.Collections;

public interface IRobotState {

    void UpdateState();
    void EnterPauseState();
    void EnterPlayState();
    //functions for handling stuff during a/deaccelerating
    void OnAccelerate();
    void OnDeaccelerate();
}
