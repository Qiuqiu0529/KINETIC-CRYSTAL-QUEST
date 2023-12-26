using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nova;
public class RankPicMgr : MonoBehaviour
{
    public Sprite[] rankSprites;
    public UIBlock2D rankImage;
    private void Start()
    {
        int rank=CountTime.Instance.rank;
        if(rank>=0&&rank<=5)
        {
            rankImage.Visible=true;
            rankImage.SetImage(rankSprites[rank]);
        }
        else
        {
            rankImage.Visible=false;
        }
    }

}
