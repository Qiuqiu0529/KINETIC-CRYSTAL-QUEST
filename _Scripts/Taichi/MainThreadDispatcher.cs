using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();

    private static MainThreadDispatcher _instance;

    public static MainThreadDispatcher Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MainThreadDispatcher>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("MainThreadDispatcher");
                    _instance = obj.AddComponent<MainThreadDispatcher>();
                }
            }
            return _instance;
        }
    }

    private void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                Action action = _executionQueue.Dequeue();
                action.Invoke();
            }
        }
    }

    public void Enqueue(Action action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }
}

