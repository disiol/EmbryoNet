using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PatchContainer : MonoBehaviour
{
    [HideInInspector] public String folderPath;
    [HideInInspector] public string filePath;
    [FormerlySerializedAs("fileOrrder")] [HideInInspector] public int fileOrder;
}