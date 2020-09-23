using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the whole collection of room infos. 
/// </summary>
[CreateAssetMenu(fileName = "RoomsInfoManager", menuName = "Rooms/Rooms Info Manager", order = 2)]
public class RoomsInfoManager : ScriptableObject
{

    public List<RoomInfo> list;
    public Dictionary<string, int> mapping;

    public List<RoomDoorAnchor> roomDoors;
    public Dictionary<Vector3, List<RoomDoorAnchor>> roomDoorsDirectionMapping;

    private System.Random rand = new System.Random();
    public RoomInfo GetByName(string name) {
        return list[mapping[name]];
    }

    public RoomInfo GetRandomRoom(RoomInfo exclude = null) {
        RoomInfo candidate = list[rand.Next(list.Count)];
        while(candidate == exclude) {
            candidate = list[rand.Next(list.Count)];
        }
        return candidate;
    }

    public RoomDoorAnchor GetRandomRoomDoor(RoomInfo exclude = null) {
        RoomDoorAnchor candidate = roomDoors[rand.Next(roomDoors.Count)];
        while (candidate.room == exclude) {
            candidate = roomDoors[rand.Next(roomDoors.Count)];
        }
        return candidate;
    }



    public RoomDoorAnchor GetRandomRoomDoor(Vector3 direction, RoomInfo exclude = null) {
        List<RoomDoorAnchor> candidates = roomDoorsDirectionMapping[direction];
        RoomDoorAnchor candidate = candidates[rand.Next(candidates.Count)];
        while (candidate.room == exclude) {
            candidate = candidates[rand.Next(candidates.Count)];
        }
        return candidate;
    }


    /// <summary>
    /// Initialize the mapping to names
    /// </summary>
    public void Initialize() {
        if(mapping == null) {
            mapping = new Dictionary<string, int>();
        }
        if(roomDoors == null) {
            roomDoors = new List<RoomDoorAnchor>();
        }
        if (roomDoorsDirectionMapping == null) roomDoorsDirectionMapping = new Dictionary<Vector3, List<RoomDoorAnchor>>();

        mapping.Clear();
        roomDoors.Clear();
        for (int i = 0; i < list.Count; i ++) {
            mapping.Add(list[i].roomName, i);
            foreach(DoorAnchor doorAnchor in list[i].doorAnchors) {
                RoomDoorAnchor roomDoor = new RoomDoorAnchor { room = list[i], doorAnchor = doorAnchor };
                roomDoors.Add(roomDoor);
                Vector3 direction = roomDoor.doorAnchor.direction;
                if (!roomDoorsDirectionMapping.ContainsKey(direction)) {
                    roomDoorsDirectionMapping.Add(direction, new List<RoomDoorAnchor>());
                }
                roomDoorsDirectionMapping[direction].Add(roomDoor);
            }
        }
    }

    /// <summary>
    /// Generate the logical neighbors of stored rooms in the manager
    /// </summary>
    public void GenerateLogicalNeighbors() {
        foreach(RoomInfo roomInfo in list) {
            GenerateLogicalNeighbors(roomInfo);
        }
    }

    /// <summary>
    /// Generate logical neighbors of a particular room. Drawn from the current manager room pool.
    /// </summary>
    /// <param name="roomInfo"> RoomInfo whose neighbors to be generated. </param>
    public void GenerateLogicalNeighbors(RoomInfo roomInfo) {
        if (roomInfo.logicalNeighbors == null) roomInfo.logicalNeighbors = new List<RoomDoorAnchor>();
        roomInfo.logicalNeighbors.Clear();
        for (int i = 0; i < roomInfo.doorAnchors.Length; i++) {
            roomInfo.logicalNeighbors.Add(GetRandomRoomDoor(-roomInfo.doorAnchors[i].direction, roomInfo));
        }
    }


}

