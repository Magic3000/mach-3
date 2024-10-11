using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Dot : MonoBehaviour
{
    public enum DotType
    {
        Banana,
        Cherries,
        Grapes,
        Orange,
        Strawberry,
        Watermelon
    }

    public int X;
    public int Y;
    public DotType dotType;
}
