using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class ThreadedChunkLoader
{
    private static AutoResetEvent resetevent = new AutoResetEvent(false);
    private static Queue<Action> EventQueue = new Queue<Action>();

    public void Run()
    {
        Action task;
        while (true)
        {
            while (EventQueue.Count != 0)
            {
                lock (EventQueue)
                {
                    task = EventQueue.Dequeue();
                }
                task.Invoke();
            }
            resetevent.WaitOne();
        }
    }

    public static void AddTask(Action action)
    {
        lock (EventQueue)
        {
            EventQueue.Enqueue(action);
        }
        resetevent.Set();
    }
}