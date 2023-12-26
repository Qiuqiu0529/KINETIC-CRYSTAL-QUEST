using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using MoreMountains.Tools;

public class CameraMove : MonoBehaviour
{
    public CinemachineCameraOffset cinemachineCameraOffset;
    // Start is called before the first frame update
    float startTime = 0;
    [SerializeField] float duration = 0.5f;
    [SerializeField] float newX = 0;

    public void MoveLeftFunc()
    {
        StartCoroutine(MoveLeft());
    }

    public void MoveRightFunc()
    {
        StartCoroutine(MoveRight());
    }


    IEnumerator MoveRight()
    {
        startTime = Time.time;
        newX = cinemachineCameraOffset.m_Offset.x;
        while (newX > 0f)
        {
            yield return new WaitForSeconds(0.1f);
            float elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            //float progress = MMTweenDefinitions.EaseOut_Quadratic(t);
            newX = Mathf.Lerp(0, 4f, t);
            cinemachineCameraOffset.m_Offset = new Vector3(4 - newX, 0, 0);

        }
        cinemachineCameraOffset.m_Offset = new Vector3(0, 0, 0);

    }


    IEnumerator MoveLeft()
    {
        startTime = Time.time;
        newX = cinemachineCameraOffset.m_Offset.x;

        while (newX < 4f)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            float elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            //float progress = MMTweenDefinitions.EaseOut_Quadratic(t);
            newX = Mathf.Lerp(0, 4f, t);
            cinemachineCameraOffset.m_Offset = new Vector3(newX, 0, 0);

        }

        cinemachineCameraOffset.m_Offset = new Vector3(4, 0, 0);

    }
}
