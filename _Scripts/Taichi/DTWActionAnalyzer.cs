using System;
using UnityEngine;
using System.Collections.Generic;

public class DTWActionAnalyzer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


}

class DTWAlgorithm
{
    public enum JointCalculationMode
    {
        AngleMode,
        VectorMode
    }


    public static float CalculateDistance(List<float[,]> sequence1, List<float[,]> sequence2, bool normalize, JointCalculationMode mode)
    {
        int n = sequence1.Count;
        int m = sequence2.Count;
        if (n == 0)
        {
            throw new ArgumentException("标准序列为空", "sequence1");
        }

        if (m == 0)
        {
            throw new ArgumentException("动捕序列为空", "sequence2");
        }
        
        // Create a 2D array to store the distance values
        float[,] distance = new float[n, m];
        
        // Calculate the distance matrix
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                float cost = 0.0f;

                switch (mode)
                {
                    case JointCalculationMode.AngleMode:
                        cost = PoseDiffCalculator.CalDiffOf2AnglePoses(
                                        sequence1[i], sequence2[j]);
                        break;
                    case JointCalculationMode.VectorMode:
                        cost = PoseDiffCalculator.CalDiffOf2VecPoses(
                            sequence1[i], sequence2[j], 2);
                        break;
                }


                if (i == 0 && j == 0)
                {
                    distance[i, j] = cost;
                }
                else if (i == 0)
                {
                    distance[i, j] = cost + distance[i, j - 1];
                }
                else if (j == 0)
                {
                    distance[i, j] = cost + distance[i - 1, j];
                }
                else
                {
                    distance[i, j] = cost + Mathf.Min(distance[i - 1, j],
                        Mathf.Min(distance[i, j - 1], distance[i - 1, j - 1]));
                }
            }
        }

        if (normalize)
        {
            return distance[n - 1, m - 1] / sequence1.Count;
        }

        // Return the final distance
        return distance[n - 1, m - 1];
    }

    public static float CalculateMaxDistance(List<float[,]> sequence1, List<float[,]> sequence2, bool normalize)
    {
        int n = sequence1.Count;
        int m = sequence2.Count;
        if (n == 0)
        {
            throw new ArgumentException("标准序列长度为0", "sequence1");
        }

        if (m == 0)
        {
            throw new ArgumentException("动捕序列长度为0", "sequence2");
        }

        int numOfJoints = sequence2[0].GetLength(0);
        int dim = sequence2[0].GetLength(1);

        float distance = 0.0f;

        // Calculate the distance matrix
        for (int i = 0; i < n; i++)
        {
            float cost = 0.0f;
            for (int j = 0; j < numOfJoints; j++)
            {
                for (int k = 0; k < dim; k++)
                {
                    cost += MathF.Pow(sequence1[i][j, k],2);
                }
            }

            distance += MathF.Sqrt(cost);
        }

        if (normalize)
        {
            return distance / sequence1.Count;
        }

        // Return the final distance
        return distance;
    }

}


