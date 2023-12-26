using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalorieUtil
{
    public static float CalCalorie(int duration, int rank, float k = 0.0023f)
    {
        //有氧操 0.069 千卡/斤/分钟 0.138千卡/kg/分钟 0.0023千卡/kg/s
        // 热量（kcal）=体重（kg）×运动时间（小时）×指数K。指数K＝30÷速度（分钟/400米）
        float result = PlayerPrefs.GetInt(Global.weight, 60) * duration * k;
        if (rank < 0)
        {
            result /= 2;
        }
        else//rank =A & 以上 result 满，=B *0.9，=c*0.6，=F *0.4 
        {
            if (rank >= 3)
            {
                return result;
            }
            else if (rank == 2)
            {
                return result * 0.9f;
            }
            else if (rank == 1)
            {
                return result * 0.6f;
            }
            else if (rank == 0)
            {
                return result * 0.4f;
            }
        }
        return result;
    }
}
