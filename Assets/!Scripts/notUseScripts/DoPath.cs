using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoPath : MonoBehaviour
{

    Vector3[] path =
    {
    new Vector3(0f,0f,10f),
    new Vector3(5f,0f,10f),
    new Vector3(5f,0f,0f),
    new Vector3(0f,0f,0f)
};

    private void Start()
    {    
        this.transform.position = Vector3.zero;
        //指定したPathを10秒で通り、進行方向を向く
        this.transform.DOPath(path, 10f).SetLookAt(0.01f).SetEase(Ease.Linear).SetLoops(-1,LoopType.Restart);
    }
}
