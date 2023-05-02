using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Speech", menuName = "Speech")]
public class Speech : ScriptableObject
{
    [TextArea] public string speechText;
    public AudioClip speechClip;

    [Range(1, 3)] public int positivity = 1;
}
