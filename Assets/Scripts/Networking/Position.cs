using System;
using UnityEngine;

[Serializable]
public struct Position {
    public float x;
    public float y;
    public float z;

    public Position(Transform transform)
    {
        x = transform.position.x;
        y = transform.position.y;
        z = transform.position.z;
    }

    public Position(Vector2 vector)
    {
        x = vector.x;
        y = vector.y;
        z = 0;
    }
    public Position(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Position(float posX, float posY)
    {
        x = posX;
        y = posY;
        z = 0;
    }
    public Vector2 V2()
    {
        return new Vector2(x, y);
    }
}