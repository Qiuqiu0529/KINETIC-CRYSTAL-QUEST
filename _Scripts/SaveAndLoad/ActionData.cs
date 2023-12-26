using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

[Serializable]
public class TaichiActionData
{
    public int actionID;
    public string animName;
    public string nameText;
    public int defaultCount=3;
    [TextArea]
    public string description;
}


[Serializable]
public class TaichiActionSingle
{
    public int actionID;
    public int count;

}
