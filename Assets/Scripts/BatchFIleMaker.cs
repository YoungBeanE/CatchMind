using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;

public class BatchFIleMaker : MonoBehaviour
{
    [field: SerializeField] private string CurPath { get; set; }
    [field: SerializeField] private string BatPath { get; set; }
    [field: SerializeField] private string FileName { get; set; }

    private void Awake()
    {
        CurPath = Directory.GetCurrentDirectory();
        CurPath += "/Assets/Resources";
        FileName = "/KeyWordMaker.exe";

        //UnityEngine.Debug.Log(CurPath);
        //if (File.Exists(CurPath + BatFileName))
        //{
        //    Process.Start(CurPath + BatFileName);
        //}
        //else
        //{
        //    UnityEngine.Debug.Log("bye");
        //}
    }

}
