using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Class", menuName = "NSObjects/ClassScriptableObject", order = 1)]
public class PlayableClass : ScriptableObject
{
    public string className;
    public Material tempColor;
}
