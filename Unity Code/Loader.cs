using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.ARFoundation;
using System;
using UnityEngine.XR.ARSubsystems;

public class Loader : MonoBehaviour
{
    ARTrackedImageManager manager;
    public Ret[] AllRets;
    public TMP_Text Post_Text;
    public GameObject HolderPrefab;
    public GameObject AlertPanel;
    public int score;
    public Dictionary<string, GameObject> Holders = new Dictionary<string, GameObject>();
    List<Ret> DiscoveredRets = new List<Ret>();

    private void Awake()
    {
        //Load manager and all Rets from Resources/Rets
        manager = gameObject.GetComponent<ARTrackedImageManager>();
        AllRets = Resources.LoadAll<Ret>("Rets");

        //Load all rets on holders and into a dictionary (string, gameobject)
        foreach (Ret curRet in AllRets)
        {
            GameObject newHolder = Instantiate(HolderPrefab, Vector3.zero, Quaternion.identity);
            newHolder.name = curRet.TrackID;
            LoadHolder(newHolder.GetComponent<Holder>(),curRet);
            newHolder.SetActive(false);
            Holders.Add(curRet.TrackID, newHolder);
        }

    }

    //Load Ret into holder and spawn/set vars
    private void LoadHolder(Holder holder,Ret retToLoad)
    {
        //Spawn Body
        GameObject retBody = Instantiate(retToLoad.Body, holder.Body.transform.position, holder.Body.transform.rotation);
        retBody.transform.SetParent(holder.Body.transform,false);

        //Spawn Platform
        GameObject retPlat = Instantiate(retToLoad.Platform, holder.Platform.transform.position, holder.Platform.transform.rotation);
        retPlat.transform.SetParent(holder.Platform.transform, false);

        //Set nametag and ret
        holder.NameTag.SetText(retToLoad.Name);
        holder.heldRet = retToLoad;

        //If 2D then set the sprite image
        if (retToLoad.is2D) {
            retBody.GetComponentInChildren<SpriteRenderer>().sprite = retToLoad.sprite;
        }

    }

    private void OnEnable()
    {
        manager.trackedImagesChanged += ImageChanged;
    }
    private void OnDisable()
    {
        manager.trackedImagesChanged -= ImageChanged;
    }

    private void ImageChanged(ARTrackedImagesChangedEventArgs eventArgs) {

        //If image detected scan image
        foreach (ARTrackedImage trackedImage in eventArgs.added)
            {
            ScanImage(trackedImage);
            }
        //If image has been updated, scan image, if not trackable set it to inactive
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                ScanImage(trackedImage);
            }
            else
                Holders[trackedImage.referenceImage.name].SetActive(false);

        }

        //If we lost the tracked image, set prefab to inactive 
        foreach (ARTrackedImage trackedImage in eventArgs.removed) {
            Holders[trackedImage.referenceImage.name].SetActive(false);

        }
    }

    //Scan image for trackable object that we'll use to position our prefab
    private void ScanImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;
        Vector3 position = trackedImage.transform.position;
        GameObject spawned = Holders[name];

        //Check if we have seen this Ret before
        Ret scannedRet = spawned.GetComponent<Holder>().heldRet;
        if(scannedRet!=null)
        DiscoveryCheck(scannedRet);
        
        //Set pos and rot and make active 
        spawned.transform.position = position;
        spawned.transform.rotation = trackedImage.transform.rotation;
        spawned.SetActive(true);

        //sort through holders to set any we don't see to inactive
        foreach (GameObject gameobj in Holders.Values)
        {
            if(gameobj.name != name)
                gameobj.SetActive(false);
        }

    }

    //Checks if we have seen ret before, if not we display an alert and add points to the score 
    private void DiscoveryCheck(Ret heldRet)
    {
        if (!DiscoveredRets.Contains(heldRet)) {
            AlertPanel.SetActive(false);
            AlertPanel.GetComponentInChildren<TMP_Text>().SetText("Discovered a "+heldRet.name+"\n"+heldRet.Value+" Points!");
            score += heldRet.Value;
            Post_Text.SetText("SCORE " + score);
            DiscoveredRets.Add(heldRet);
            AlertPanel.SetActive(true);

        }

    }
}
