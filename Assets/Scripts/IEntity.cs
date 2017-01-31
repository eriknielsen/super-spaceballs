using UnityEngine;
using System.Collections;

public abstract class Entity : MonoBehaviour{

    public abstract void EnterPause();
    public abstract void EnterPlay();
    public abstract bool IsFinished();
}
