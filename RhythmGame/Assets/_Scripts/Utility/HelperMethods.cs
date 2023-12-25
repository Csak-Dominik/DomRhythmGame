using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperMethods
{
    public static void DestoryAllChildren(this Transform transform)
    {
        foreach (Transform child in transform)
        {
            Object.Destroy(child.gameObject);
        }
    }
}