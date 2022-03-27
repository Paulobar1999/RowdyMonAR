using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewRet", menuName = "Ret")]

public class Ret : ScriptableObject
{
    public string Name;
    public string TrackID;
    public int Value;

    public GameObject Body;
    public GameObject Platform;

    //if 2d load sprite on sprite prefab
    public bool is2D;
    public Sprite sprite;



}
