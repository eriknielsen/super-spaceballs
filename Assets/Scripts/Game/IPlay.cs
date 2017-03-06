using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public interface IPlay {

        void SelectCommand(Command.AvailableCommands command);
        void DeselectRobot();
}

