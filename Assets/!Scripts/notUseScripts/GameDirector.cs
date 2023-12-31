using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    public static float updateSpan;
    public static float delay;
    [Header("座標を保存する周期")]
    [SerializeField] float span = 0.1f;
    [Header("客が追いかけ始めるまでにかかる時間")]
    [SerializeField] float followDelay = 0.2f;

    private void Awake()
    {
        updateSpan = span;
        delay = followDelay;
    }
}
