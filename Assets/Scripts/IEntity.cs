using UnityEngine;
using System.Collections;

public abstract class IEntity : MonoBehaviour
{

    public abstract void EnterPause();
    public abstract void EnterPlay();
    public abstract bool IsFinished();
}
