using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Table : MonoBehaviour
{
    [SerializeField] private int myNum;
    [SerializeField] public Vector3[] exitsPos;
    [NonSerialized] public float count;

    private void Start()
    {
        count = exitsPos.Length;
    }
}