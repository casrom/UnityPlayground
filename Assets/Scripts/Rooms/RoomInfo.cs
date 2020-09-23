using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomInfo", menuName = "Rooms/RoomInfo", order = 1)]
public class RoomInfo : ScriptableObject
{
    public string roomName;
    public GameObject prefab;
    /// <summary>
    /// Available orientations for doors
    /// </summary>
    // public Orientation[] orientations;
    public Vector3 dimension;
    public DoorAnchor[] doorAnchors;
    public List<RoomDoorAnchor> logicalNeighbors;
}
