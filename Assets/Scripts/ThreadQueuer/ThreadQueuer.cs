//-----------------------------------------------------------------------
// <copyright file="ThreadQueuer.cs" company="Quill18 Productions">
//     Copyright (c) Quill18 Productions. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;
using System.Threading;
using System;
using System.Collections.Generic;

public class ThreadQueuer : MonoBehaviour
{
    [SerializeField] int cuantosTreths;
    void Start()
    {
        // BIG HUGE NOTE:     WebGL doesn't do multithreading.

        Debug.Log("Start() -- Started.");
        for (int i = 0; i < cuantosTreths; i++)
        {
            StartThreadedFunction(() => { Debug.Log("Hi im in trhet: "+i); });
        }

        Debug.Log("Start() -- Done.");
    }

    public static void StartThreadedFunction(Action someFunctionWithNoParams)
    {
        Thread t = new Thread(new ThreadStart(someFunctionWithNoParams));
        t.Start();
    }
}
