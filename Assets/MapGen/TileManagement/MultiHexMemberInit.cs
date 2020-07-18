using Assets.MapGen.TileManagement;
using UnityEngine;

[RequireComponent(typeof(HexMember))]
public class MultiHexMemberInit : MonoBehaviour
{
    public HexMember[] ChildMembers;
    // Start is called before the first frame update
    void Start()
    {
        var myMember = GetComponent<HexMember>();
        foreach (var child in ChildMembers)
        {
            child.localPosition = myMember.LocalPosition + child.LocalPosition;
        }
        foreach (var child in ChildMembers)
        {
            child.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
