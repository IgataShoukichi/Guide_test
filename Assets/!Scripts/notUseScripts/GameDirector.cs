using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    public static float updateSpan;
    public static float delay;
    [Header("���W��ۑ��������")]
    [SerializeField] float span = 0.1f;
    [Header("�q���ǂ������n�߂�܂łɂ����鎞��")]
    [SerializeField] float followDelay = 0.2f;

    private void Awake()
    {
        updateSpan = span;
        delay = followDelay;
    }
}
