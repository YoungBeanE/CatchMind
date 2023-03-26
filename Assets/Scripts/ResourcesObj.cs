using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffData
{
    public AudioClip myClip;
    public ParticleSystem fx;
}

[CreateAssetMenu(fileName = "New ResourceDataObj", menuName = "ScriptableObjects/ResourceDataObj", order = 5)]
public class ResourcesObj : ScriptableObject
{
    public Sprite[] PlayerImg = null;

    
    public EffData myEffData;
    public EffData[] myEffDataArray;
}
