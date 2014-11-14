using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class ThreadEventHandler : MonoBehaviour
{
    public static ThreadEventHandler EventHandler;
    public Queue<Action> EventQueue = new Queue<Action>();
    private Thread chunkThread;

    private ThreadEventHandler()
    {
        EventHandler = this;
    }

    void Start()
    {
        chunkThread = new Thread(new ThreadStart(new ThreadedChunkLoader().Run));
        chunkThread.Start();
    }

    void Update()
    {
        while (EventQueue.Count != 0)
        {
            lock (EventHandler.EventQueue)
            {
                EventQueue.Dequeue().Invoke();
            }
        }
    }

    public static void RunOnMainThread(Action action)
    {
        lock (EventHandler.EventQueue)
        {
            ThreadEventHandler.EventHandler.EventQueue.Enqueue(action);
        }
    }

    private void OnApplicationQuit()
    {
        chunkThread.Abort();
    }
}
