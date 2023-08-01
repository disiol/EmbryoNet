using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PatchContainer : MonoBehaviour
{
    [HideInInspector] public String folderPath;
    [FormerlySerializedAs("file")] [HideInInspector] public string filePath;
}