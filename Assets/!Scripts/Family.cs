using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Family : MonoBehaviour
{
    public Transform parentPos;
    public Vector3 setPos;
    public Vector3[] myPos;
    public GameObject table;

    void Start()
    {
        GameObject child = transform.GetChild(0).gameObject;
        parentPos = child.transform;
    }

    void Update()
    {
        gameObject.GetComponent<SphereCollider>().center = parentPos.localPosition;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Table"))
        {
            table = other.gameObject;
            setPos = other.gameObject.transform.position;
            Vector3[] _myPos =  
            {
                setPos + new Vector3(0.5f, 0, 0.5f),
                setPos + new Vector3(0.5f, 0, -0.5f),
                setPos + new Vector3(-0.5f, 0, -0.5f),
                setPos + new Vector3(-0.5f, 0, 0.5f)
            };
            myPos = _myPos;
        }
    }

}
