using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score {

    public int teamOne = 0;
    public int teamTwo = 0;

    public string GetScoreAsString()
    {
        //return the score as a string
        string s = teamOne + " : " + teamTwo;

        return s;
    }
	
}
