using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

[Serializable]
public class QuizOption
{
    [TextArea]
    public string content;
}

[Serializable]
public class SingleQuiz
{
     [TextArea]
    public string questionContent;
    public int answerIndex;
    public List<QuizOption> options;
}

[CreateAssetMenu(fileName = "new QuizData", menuName = "_Scripts/QuizData")]
public class QuizData : SerializedScriptableObject
{
    public string title;
    public List<SingleQuiz> questions;
}
