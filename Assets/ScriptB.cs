using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[AddComponentMenu("Scripts/MRTK/Examples/LaunchUri")]
public class ScriptB : MonoBehaviour
{
    // Update is called once per frame
    public ResetObi res;
    public void Launch()
    {
        res.reset();
        print("Hello");
    }
    private void Update()
    {
        //print("fffff");
        int a = 0;
    }
}

