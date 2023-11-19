using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    public static float updateSpan;
    public static float delay;
    [Header("À•W‚ğ•Û‘¶‚·‚éüŠú")]
    [SerializeField] float span = 0.1f;
    [Header("‹q‚ª’Ç‚¢‚©‚¯n‚ß‚é‚Ü‚Å‚É‚©‚©‚éŠÔ")]
    [SerializeField] float followDelay = 0.2f;

    private void Awake()
    {
        updateSpan = span;
        delay = followDelay;
    }
}
