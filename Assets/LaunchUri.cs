// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[AddComponentMenu("Scripts/MRTK/Examples/LaunchUri")]

public class LaunchUri : MonoBehaviour
{
    public ResetObi res;
    public void Launch(string uri)
    {
        //GameObject plane = GameObject.Find("Plane");
        //var pos = plane.transform.position;
        //print(pos);
        //plane.GetComponent<ScriptB>().Launch();
        res.reset();        
    }
}

