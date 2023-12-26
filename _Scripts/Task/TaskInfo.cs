using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class TaskInfo 
{
    public int taskID;
    public string taskName;
    public string taskDescription;
    public bool finished;
    public int addExperience;

    public bool CanFinished()
    {
        return !finished;
    }

    public void Finished()
    {
        finished=true;
    }
}

