﻿using UnityEngine;
using System.Collections;

public class GameTimer {
    public int remainingTime;
    int matchTime;
    public GameTimer(int totalMatchTime)
    {
        matchTime = totalMatchTime;
        remainingTime = matchTime;
    }
    public IEnumerator CountDownSeconds(int seconds)
    {
        while(seconds > 0)
        {
            yield return new WaitForSeconds(1f);
            seconds--;
            remainingTime--;
        }
    }

    public int MinutesRemaining()
    {
        return remainingTime / 60;
    }
    public int SecondsRemaining()
    {
        return remainingTime % 60;
    }
}
