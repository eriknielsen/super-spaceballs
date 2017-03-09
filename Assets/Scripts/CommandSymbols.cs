using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CommandSymbols : MonoBehaviour {

    [System.Serializable]
    private class Symbols
    {
        public Sprite symbolSprite;
        public Command.AvailableCommands relatedCommand;
        private Type commandType;

        public Type CommandType
        {
            get { return commandType; }
            set { commandType = value; }
        }
    }

    [SerializeField]
    private Symbols[] symbols;

    public Sprite GetSymbolSprite(Type commandType)
    {
        if (commandType != null)
        {
            for (int i = 0; i < symbols.Length; i++)
            {
                if (symbols[i].CommandType == commandType)
                {
                    return symbols[i].symbolSprite;
                }
            }
        }
        return null;
    }

    void Awake()
    {
        if (symbols != null)
        {
            for (int i = 0; i < symbols.Length; i++)
            {
                if (symbols[i].relatedCommand == Command.AvailableCommands.Move)
                {
                    symbols[i].CommandType = typeof(MoveCommand);
                }
                else if (symbols[i].relatedCommand == Command.AvailableCommands.Push)
                {
                    symbols[i].CommandType = typeof(PushCommand);
                }
            }
        }
    }
}
