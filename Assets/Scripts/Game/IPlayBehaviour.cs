﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayBehaviour {

    void SelectCommand(Command.AvailableCommands command);
    void DeselectRobot();

    void PreOnGoalScored();
}
