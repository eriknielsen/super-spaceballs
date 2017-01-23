using UnityEngine;
using System.Collections;

public interface IRobotState {


    void UpdateState();
    void EnterPauseState();
    void EnterPlayState();
}
