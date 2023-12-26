using Vector3 = System.Numerics.Vector3;
using System;
using System.ComponentModel;

public class PoseDiffCalculator 
{

    /// <summary>
    /// 使用欧式距离计算两个欧拉角pose的差异
    /// </summary>
    /// <param name="poseA"></param>
    /// <param name="poseB"></param>
    /// <returns></returns>
    public static float CalDiffOf2AnglePoses(float[,] poseA, float[,] poseB)
    {
        int numOfJoints = poseA.GetLength(0); // 参与计算的关节数量

        int numOfDimensions = poseA.GetLength(1); //欧拉角维度数

        float diffValue = 0.0f;

        for (int i = 0; i < numOfJoints; i++)
        {
            for (int j = 0; j < numOfDimensions; j++)
            {
                diffValue += MathF.Pow(poseA[i,j] - poseB[i,j], 2);
            }
        }

        diffValue = MathF.Sqrt(diffValue);

        return diffValue;
    }

    /// <summary>
    /// 计算pose之间由关节向量余弦相似度得到的差异
    /// </summary>
    /// <param name="poseA"></param>
    /// <param name="poseB"></param>
    /// <returns></returns>
    public static float CalDiffOf2VecPoses(float[,] poseA, float[,] poseB, int dim)
    {
        int numOfJoints = poseA.GetLength(0); // 参与计算的关节数量        

        float diffValue = 0.0f;

        for (int i = numOfJoints - 1; i < numOfJoints; i++)
        {
            Vector3 vecA = new Vector3();
            Vector3 vecB = new Vector3();
            if (dim==2)
            {
                vecA = new Vector3(poseA[i, 0], poseA[i, 1], 0);
                vecB = new Vector3(poseB[i, 0], poseB[i, 1], 0);
            }
            else if (dim==3)
            {
                vecA = new Vector3(poseA[i, 0], poseA[i, 1], poseA[i, 2]);
                vecB = new Vector3(poseB[i, 0], poseB[i, 1], poseB[i, 2]);
            }

            float dif = CalculateAngleBetweenVectors(vecA, vecB);            
            diffValue += MathF.Pow(dif, 2);
        }

        diffValue = MathF.Sqrt(diffValue);

        return diffValue;
    }

    private static float CalculateDifBetweenVectors(Vector3 sysVectorA, Vector3 sysVectorB)
    {
        float dotProduct = Vector3.Dot(sysVectorA, sysVectorB);
        float normA = sysVectorA.Length();
        float normB = sysVectorB.Length();

        float cosineSimilarity = dotProduct / (normA * normB);      

        return 1-cosineSimilarity;
    }

    private static float CalculateAngleBetweenVectors(Vector3 sysVectorA, Vector3 sysVectorB)
    {
        float dotProduct = Vector3.Dot(sysVectorA, sysVectorB);
        float normA = sysVectorA.Length();
        float normB = sysVectorB.Length();       

        float cosineTheta = dotProduct / (normA * normB);
        float angleInRadians = (float)Math.Acos(cosineTheta);

        float angleInDegrees = angleInRadians * (180.0f / (float)Math.PI);

        return angleInDegrees;
    }

}

