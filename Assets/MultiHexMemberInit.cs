using Assets.Scripts.MovementExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HexMember))]
public class MultiHexMemberInit : MonoBehaviour
{
    public HexMember[] ChildMembers;
    // Start is called before the first frame update
    void Start()
    {
        var myMember = GetComponent<HexMember>();
        var tileManager = myMember.tilemapManager;
        foreach(var child in ChildMembers)
        {
            child.startingPosition = myMember.Position + child.Position;
            child.tilemapManager = myMember.tilemapManager;
            //tileManager.RegisterNewMapMember(child, myMember.Position + child.Position);
        }
        foreach(var child in ChildMembers)
        {
            child.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
