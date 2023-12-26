using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 将pose之间的差异转换为0~5的分数
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    public static int ConvertDifToScore(float distance)
    {
        int score =
            (distance <= 12.0f) ? 5 :
            (distance <= 15.0f) ? 4 :
            (distance <= 17.0f) ? 3 :
            (distance <= 20.0f) ? 2 :
            (distance <= 25.0f) ? 1 : 0;

        return score;
    }
}
