using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseDataDebuger : MonoBehaviour
{
    public TMPro.TMP_Text tMP_Text;
    public bool isShow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vector = this.GetComponent<GestureControlManager>().poseDataServer.GetJointPosition((int)PoseIndex.neck);
        string vecString = vector.ToString("G4");

        if (!isShow)
        {
            return;
        }

        if (tMP_Text != null)
        {
            tMP_Text.text = vecString;
        }
        else
        {
            print(vecString);
        }
    }
}
