using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

[CreateAssetMenu(fileName = "new TaichiActionDB", menuName = "_Scripts/TaichiActionDB")]
public class TaichiActionDB : SerializedScriptableObject
{
    public List<TaichiActionData> taichiActions;

}
