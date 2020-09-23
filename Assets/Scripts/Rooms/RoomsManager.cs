using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Ori = Orientation;

public class RoomsManager : MonoBehaviour
{
    public int configuration = 0;
    public RoomsInfoManager roomInfoManager;
    public GameObject doorPrefab;
    public RoomInfo baseRoomInfo, roomVoidInfo;
    public Inventory playerInventory;

    private GameObjectPool doorPool;
    private GameObjectPool roomPool;

    public TimeManager timeManager;
    public Room activeRoom;
    public List<Door> visibleDoors;
    public HashSet<Room> rooms = new HashSet<Room>();
    public List<RoomInfo> roomInfos = new List<RoomInfo>();
    //bool playerInRoom = false;

    public bool random;
    float roomOffset = .1f;
    // Start is called before the first frame update

    bool voidRoomAdded = false;
    void Start()
    {
        //playerInventory.Clear();
        roomInfoManager.Initialize();
        roomInfoManager.GenerateLogicalNeighbors();
        roomInfoManager.GenerateLogicalNeighbors(baseRoomInfo);

        doorPool = new GameObjectPool();
        for (int i = 0; i < 50; i++) {
            doorPool.AddToPool(Instantiate(doorPrefab));
        }

        //TODO: Pool Rooms
        //Challenge: Different types of rooms
        roomPool = new GameObjectPool();


        //for (int i = 0; i < 5; i++) {
        //    //Heuristically refine logical connections
        //    foreach (RoomInfo roomInfo in roomInfoManager.list) {
        //        Room room = new Room(Instantiate(roomInfo.prefab), roomInfo);
        //        room.FillDoors(doorPool);
        //        for (int j = 0; j < 10; j++) {
        //            GenerateConnectedRooms(room);
        //        }

        //        foreach (Room r in rooms) {
        //            TryRemoveRoom(r, true);
        //        }
        //        TryRemoveRoom(room, true);
        //        rooms.Clear();
        //    }
        //}

        //Debug.Log(roomPassages.Count);

        InitializeBaseRoom(baseRoomInfo);

    }


    public bool endless = true;
    int returnThreshold = 10;
    public bool returnMode = false;
    //public bool returnSuccess = false;
    public int returnDoorIndex = -1;
    bool returnGenerated;
    public int returnProgress = 0;
    Room returnRoom;
    public int depth = 0;
    HashSet<Room> visited = new HashSet<Room>();

    void InitializeBaseRoom(RoomInfo info) {
        GameObject roomObject = Instantiate(info.prefab, transform);
        activeRoom = new Room(roomObject, info);
        //activeRoom.AddDoor(Ori.NORTH, doorPool.DePool());
        //activeRoom.AddDoor(Ori.SOUTH, doorPool.DePool());
        activeRoom.FillDoors(doorPool);
        rooms.Add(activeRoom);

    }

    HashSet<Room> toBeRemoved = new HashSet<Room>();
    // Update is called once per frame
    void Update()
    {
        removalTimeOut = 3f;

        GenerateConnectedRooms(activeRoom);

        toBeRemoved.Clear();
        foreach (Room room in rooms) {
            if (room.GetBounds().Contains(Camera.main.transform.position)) {
                activeRoom = room;
                room.lastObserved = Time.time;


                //VISIT NEWROOM LOGIC
                if (!visited.Contains(activeRoom)) {
                    visited.Add(activeRoom);
                    if (!endless) depth++;
                    //Debug.Log(depth);

                    if (returnMode) {
                        if (activeRoom == returnRoom && returnGenerated) {
                            returnGenerated = false;
                            returnDoorIndex = -1;
                            returnProgress++;
                            if (returnProgress > 5) {
                                ResetReturn();
                            }
                            Debug.Log("RETURN PROG: " + returnProgress);
                        } else { //Visited wrong room
                            returnProgress--;
                        }

                        //bug hot fix. need to fix this
                        if (activeRoom.info == baseRoomInfo) ResetReturn();
                    }
                }



            }
            TryRemoveRoom(room);
        }
        foreach (Room r in toBeRemoved) {
            rooms.Remove(r);
        }
        
        if(depth > returnThreshold) {
            returnMode = true;
            if (!voidRoomAdded) {
                voidRoomAdded = true;
                roomInfos.Add(roomVoidInfo);
                roomInfos.Add(roomVoidInfo);
                roomInfos.Add(roomVoidInfo);
            }
        }

    }

    void ResetReturn() {
        returnMode = false;
        depth = 0;
        returnProgress = 0;
        returnGenerated = false;
        returnDoorIndex = -1;
        roomInfos.RemoveAll(x => x.Equals(roomVoidInfo));
        voidRoomAdded = false;
        visited.Clear();
        removalTimeOut = 0.5f;
    }

    float removalTimeOut = 3f;


    private void TryRemoveRoom(Room room, bool immediate = false) {
        if (Time.time - room.lastObserved > removalTimeOut || immediate) {
            if (returnRoom == room && returnMode) {
                returnGenerated = false;
                returnDoorIndex = -1;
            }

            Dictionary<DoorAnchor, Door> doors = room.doors;
            room.Remove();
            Destroy(room.roomObject);
            foreach (Door door in doors.Values) {
                if (door.rooms.Count == 0) {
                    doorPool.Pool(door.gameObject);
                }
            }
            toBeRemoved.Add(room);
        }
    }




    private int regenCounter = 0;

    public HashSet<RoomPassage> roomPassages = new HashSet<RoomPassage>();

    /// <summary>
    /// Generate neighbor rooms based on door anchors of the current active room.
    /// </summary>
    /// 
    bool GenerateConnectedRooms(Room room) {
        System.Random rnd = new System.Random();


        for (int i = 0; i < room.info.doorAnchors.Length; i++) {
            
            //For each door anchor of the active room
            //generate a room
            DoorAnchor currentAnchor = room.info.doorAnchors[i];

            if (room.neighborRooms.ContainsKey(currentAnchor)) continue;
            


            //Logical Neighbors with retained Rotation
            RoomDoorAnchor targetRoomDoorAnchor = room.info.logicalNeighbors[i];

            //Random all
            if (random) {
                targetRoomDoorAnchor = roomInfoManager.GetRandomRoomDoor();
            }

            newRoomInfo = targetRoomDoorAnchor.room;
            DoorAnchor targetAnchor = targetRoomDoorAnchor.doorAnchor;            


            if (returnMode && returnProgress > 5) {
                newRoomInfo = baseRoomInfo;
            }


            //DoorAnchor targetAnchor = newRoomInfo.doorAnchors[doorIndex];

            //Assume X = Z
            // Vector3 roomPosition = activeRoom.transform.position + currentAnchor.position + currentAnchor.direction * (newRoomInfo.dimension.x / 2 + roomOffset);

            //Need fix to support Y direction
            //Vector3 roomPosition = room.transform.position + currentAnchor.direction * (currentAnchor.position.magnitude + targetAnchor.position.magnitude + roomOffset);


            //Vector3 roomPosition = room.transform.position + (currentAnchor.position - rotation * targetAnchor.position);

            ////first rotate target anchor to FOWARD
            //Quaternion localRotation = Quaternion.Euler(0, Vector3.SignedAngle(targetAnchor.direction, Vector3.forward, Vector3.up), 0);
            ////then rotate FOWARD(now our new door anchor) to look at the currentAnchor
            //Quaternion roomRotation = Quaternion.LookRotation(-currentAnchor.direction);

            ////rotate around activeroom with activeroom rotation
            //newRoomRotation = localRotation * roomRotation * room.transform.rotation;
            //newRoomPosition = room.transform.position + room.transform.rotation * (roomPosition - room.transform.position);


            Quaternion newRoomRotation = Quaternion.Euler(0, Vector3.SignedAngle(targetAnchor.direction, - (room.transform.rotation * currentAnchor.direction), Vector3.up), 0);
            Vector3 newRoomPosition = room.transform.position + (room.transform.rotation * currentAnchor.position - newRoomRotation * targetAnchor.position);

            //Debug.Log(newRoomRotation.eulerAngles + " " + newRoomPosition);

            //Debug.Log(targetAnchor.direction + " " + (-currentAnchor.direction));

            Vector3 collisionTestPosition = newRoomPosition;
            collisionTestPosition.y += newRoomInfo.dimension.y / 2;
            //check bound collision here!
            if (Physics.CheckBox(collisionTestPosition, newRoomInfo.dimension/2 - Vector3.one/2, newRoomRotation)) {
                i--; //retry!
                regenCounter++;
                if (regenCounter > 100) {
                    regenCounter = 0;
                    Debug.Log("Collision Test: Failed to generate neighbor room after multiple tries!" + room.info.roomName + " " + newRoomInfo.roomName);
                    break;
                }
                if (!random)
                    //Reset logical neighbor if in logical mode
                    room.info.logicalNeighbors[i + 1] = roomInfoManager.GetRandomRoomDoor(-currentAnchor.direction, room.info);

                continue;
            }
            

            Room newRoom = new Room(Instantiate(newRoomInfo.prefab, newRoomPosition, newRoomRotation, transform), newRoomInfo);

            //Register Passage
            roomPassages.Add(new RoomPassage {
                src = new RoomDoorAnchor{
                room = room.info,
                doorAnchor = currentAnchor},
                dst = new RoomDoorAnchor{
                room = newRoomInfo,
                doorAnchor = targetAnchor }
            });
            //Debug.Log(roomPassages.Count);

            //Link Rooms
            room.neighborRooms[currentAnchor] = newRoom;
            newRoom.neighborRooms[targetAnchor] = room;
            newRoom.doors[targetAnchor] = room.doors[currentAnchor];
            room.doors[currentAnchor].rooms.Add(newRoom);
            newRoom.FillDoors(doorPool);

            rooms.Add(newRoom);

            if (returnMode && !returnGenerated) {
                if (rnd.Next(2) > 0) {
                    returnDoorIndex = i;
                    returnRoom = newRoom;
                    returnGenerated = true;
                }
            }
            
        }
        return true;
    }

    Vector3 newRoomPosition;
    RoomInfo newRoomInfo;
    Quaternion newRoomRotation;

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        int i = 0;
        //foreach(Room room in rooms) {
        //    Gizmos.color = Color.HSVToRGB((float)i / rooms.Count, 1, 1);
        //    foreach(Door door in room.doors.Values) {
        //        Gizmos.DrawCube(room.transform.position + Vector3.up * 8, Vector3.one*2);
        //        Gizmos.DrawSphere(door.transform.position + Vector3.up * 2, .5f);
        //        Gizmos.DrawLine(room.transform.position + Vector3.up * 8, door.transform.position+Vector3.up * 2);
        //    }
        //    i++;
        //}

        //Gizmos.DrawWireCube(newRoomPosition, newRoomInfo.dimension);

        //foreach (Room room in rooms) {
        //    Gizmos.color = Color.HSVToRGB((float)i / rooms.Count, 1, 1);
        //    foreach (Room nRoom in room.neighborRooms.Values) {
        //        Gizmos.DrawCube(room.transform.position + Vector3.up * 8, Vector3.one * 2);
        //        Gizmos.DrawSphere(nRoom.transform.position + Vector3.up * 2, .5f);
        //        Gizmos.DrawLine(room.transform.position + Vector3.up * 8, nRoom.transform.position + Vector3.up * 2);
        //    }
        //    i++;
        //}

        //if (activeRoom != null) {
        //    Gizmos.color = Color.white;
        //    Gizmos.DrawCube(activeRoom.roomObject.transform.position, Vector3.one * 3);
        //    Gizmos.DrawWireCube(activeRoom.GetBounds().center, activeRoom.GetBounds().size);
        //}

        //if (doorPool != null) {
        //    int j = 0;
        //    foreach (GameObject doorObject in doorPool.collection) {
        //        if (!doorPool.IsPooled(doorObject)) {
        //            Gizmos.color = Color.HSVToRGB((float)j / doorPool.collection.Count, 1, 1);
        //            foreach (Room room in doorObject.GetComponent<Door>().rooms) {
        //                Gizmos.DrawSphere(doorObject.transform.position + Vector3.up * 2, .5f);
        //                Gizmos.DrawLine(room.transform.position + Vector3.up * 8, doorObject.transform.position + Vector3.up * 2);
        //            }
        //        }
        //    }
        //}
    }
}

public enum Orientation { 
    NORTH, SOUTH, 
    WEST, EAST
}

[Serializable]
public class Room {
    public RoomInfo info;
    public GameObject roomObject;
    public MeshCollider roomMeshCollider;
    public Dictionary<DoorAnchor, Door> doors = new Dictionary<DoorAnchor, Door>();
    public Dictionary<DoorAnchor, Room> neighborRooms = new Dictionary<DoorAnchor, Room>();
    public Transform transform;

    public Orientation[] localOrientations;
    public float lastObserved = Time.time; //timer in seconds
    float roomOffset = 0.1f;

    public Room(GameObject roomObject, RoomInfo info) {
        this.roomObject = roomObject;
        this.info = info;
        roomMeshCollider = roomObject.transform.GetChild(0).GetChild(0).GetComponent<MeshCollider>();
        transform = roomObject.transform;
    }

    public Bounds GetBounds() {
        return roomMeshCollider.bounds;
    }

    public Room GetRandomNeighbor() {
        System.Random rand = new System.Random();
        return neighborRooms.ElementAt(rand.Next(0, neighborRooms.Count)).Value;
    }

    public void Remove() {
        //disable room
        //remove links
        foreach(Door door in doors.Values) {
            door.rooms.Remove(this);
        }

        foreach(Room room in neighborRooms.Values) {
            room.RemoveNeighborRoom(this);
            
        }
    }

    public void RemoveNeighborRoom(Room room) {
        bool removal = false;
        DoorAnchor anchor = info.doorAnchors[0];

        foreach (KeyValuePair<DoorAnchor, Room> pair in neighborRooms) {
            if (pair.Value == room) {
                removal = true;
                anchor = pair.Key;
                break;
            }
        }

        if (removal) {
            neighborRooms.Remove(anchor);
        }
    }
    
    public void Rotate90() {
        this.localOrientations = new Ori[] { Ori.EAST, Ori.WEST };
        roomObject.transform.rotation = Quaternion.Euler(0, 90, 0);
    }

    public void AddDoor(DoorAnchor anchor, Door door) {
        GameObject doorObject = door.gameObject;
        Vector3 pos;
        Quaternion rot;

        pos = transform.position + anchor.position + anchor.direction * roomOffset/2;
        rot = Quaternion.LookRotation(anchor.direction);
        doorObject.transform.position = pos;
        doorObject.transform.rotation = rot;
        doorObject.transform.RotateAround(transform.position, transform.up, transform.rotation.eulerAngles.y);

        if (doors.ContainsKey(anchor)) {
            Debug.LogWarning("Adding door but door already exists at that anchor.");
        } else {
            doors.Add(anchor, door);
            door.rooms.Add(this);
        }
    }

    public void AddDoor(DoorAnchor anchor, GameObject doorObject) {
        AddDoor(anchor, doorObject.GetComponent<Door>());
    }

    public void FillDoors(GameObjectPool doorPool) {
        foreach(DoorAnchor anchor in info.doorAnchors) {
            if(!doors.ContainsKey(anchor)) {
                AddDoor(anchor, doorPool.DePool());
            }
        }
    }
}

///// <summary>
///// Prefab of a room and layout information of a type of the room
///// </summary>
//[Serializable]
//public class RoomInfo {
//    public string name;
//    public GameObject prefab;
//    /// <summary>
//    /// Available orientations for doors
//    /// </summary>
//    // public Orientation[] orientations;
//    public Vector3 dimension;
//    public DoorAnchor[] doorAnchors;
//}


/// <summary>
/// Represent the position and direction of a door relative to a room. 
/// </summary>
[Serializable]
public struct DoorAnchor {
    public Vector3 position; //the position of the door
    public Vector3 direction;//the facing direction of the door, pointing from the center of the room to outside
}

[Serializable]
public struct RoomDoorAnchor {
    public RoomInfo room;
    public DoorAnchor doorAnchor;
}

/// <summary>
/// Represents a passage from a source room to a destination room
/// </summary>
[Serializable]
public struct RoomPassage {
    public RoomDoorAnchor src;
    public RoomDoorAnchor dst;
}

