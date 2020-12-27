// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Obi;

//[AddComponentMenu("Scripts/MRTK/Examples/LaunchUriMove")]

public class LaunchUriMove : MonoBehaviour
{
    public ResetObi res;
    public void Launch(string uri)
    {
        //print("Moving");
        //res.reset();
        res.move();        
    }
}

