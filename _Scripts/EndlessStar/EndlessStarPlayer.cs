using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Forever;
using MoreMountains.Feedbacks;

public class EndlessStarPlayer : Singleton<EndlessStarPlayer>
{
    LaneRunner runner;
    float boost = 0f;
    bool canBoost = true;
    float speed = 0f;
    float startSpeed = 0f;

    public MMF_Player completeFB,heart1FB,heart2FB,heart3FB;
    public bool init;
    public Transform targetCollider;
    public KekoAnim keko;
    bool jumping;
    public int life=3;



    private void Awake()
    {
        base.Awake();
        runner = GetComponent<LaneRunner>();
        startSpeed = speed = runner.followSpeed;
        MathGate.onAnswer += OnAnswer;
        EndScreen.onRestartClicked += OnRestart;
        life=3;
    }

    public void Jump()
    {
        if (jumping)
        {
            return;
        }
        jumping = true;
        keko.Jump();
        targetCollider.localPosition = new Vector3(0, 3f, 0);
        StartCoroutine(JumpEnd());
    }

    public IEnumerator JumpEnd()
    {
        yield return new WaitForSeconds(0.7f);
        Land();
    }

    public void Land()
    {
        keko.Land();
        jumping = false;
        targetCollider.localPosition = Vector3.zero;
    }

    void OnRestart()
    {
        LevelGenerator.instance.Restart();
        runner.followSpeed = speed = startSpeed;
        boost = 0f;
        canBoost = true;
    }

    void OnAnswer()
    {
        canBoost = true;
        boost = 0f;
    }

    public int LandCount()
    {
        return runner.laneCount;
    }


    public void SetLane(int index)
    {
        if (boost == 0f)
        {
            runner.lane = index;
        }
    }

    public void pressBoost()
    {
        if (boost == 0f && canBoost)
        {
            boost = 1f;
            canBoost = false;
        }

    }

    private void Update()
    {
        if (!init)
        {
            return;
        }
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     Jump();
        // }
        if (HighWayListener.Instance.IsJump())
        {
            Jump();

        }
        if (boost > 0f) runner.followSpeed = speed + boost * speed * 2f;
        else runner.followSpeed = speed;
        boost = Mathf.MoveTowards(boost, 0f, Time.deltaTime * speed * 0.075f);
    }

    public void Init()
    {
        init = true;
        LevelGenerator.instance.StartGeneration();
    }

    public void EncounterTrap()
    {
        life--;
        if(life==2)
        {
            heart1FB.PlayFeedbacks();
        }
        else if(life==1)
        {
            heart2FB.PlayFeedbacks();
        }
        else
        {
            heart3FB.PlayFeedbacks();
        }

        if(life<=0)
        {
            SetSpeed(0);
        }
    }


    public void SetSpeed(float speed)
    {
        this.speed = speed;
        runner.followSpeed = speed;
        if (speed <= 0f)
        {
            if(init)
            {
               completeFB.PlayFeedbacks();
            }

            init=false;
        }
    }

    public float GetSpeed()
    {
        return speed;
    }
}
