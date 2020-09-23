using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class FDGraph : MonoBehaviour
{

    public GameObject nodePrefab;
    public GameObject linePrefab;
    public GameObject doorAnchorPrefab;
    public PlayerInput playerInput;

    private List<Node> nodes;
    private List<Edge> edges;

    System.Random rand = new System.Random();

    RoomsManager roomsManager;
    HashSet<RoomPassage> passages;
    Dictionary<RoomDoorAnchor, Node> roomDoorNodes;
    Dictionary<RoomInfo, Node> roomNodes;
    Dictionary<GameObject, Node> objNodeMap;

    GameObjectPool linePool;
    GameObjectPool nodePool;
    GameObjectPool doorAnchorPool;

    Dictionary<RoomPassage, EdgeDisplay> edgeDisplays;
    List<LineRenderer> lineList;
    bool m_active;

    public float centering = 1f;

    // Start is called before the first frame update
    void Start()
    {
        m_active = false;
        transform.gameObject.SetActive(false);

        roomDoorNodes = new Dictionary<RoomDoorAnchor, Node>();
        roomNodes = new Dictionary<RoomInfo, Node>();
        objNodeMap = new Dictionary<GameObject, Node>();
        roomsManager = GameObject.FindGameObjectWithTag("RoomsManager").GetComponent<RoomsManager>();

        passages = roomsManager.roomPassages;
        nodes = new List<Node>();
        edges = new List<Edge>();

        linePool = new GameObjectPool();
        nodePool = new GameObjectPool();
        doorAnchorPool = new GameObjectPool();
        edgeDisplays = new Dictionary<RoomPassage, EdgeDisplay>();

        lineList = new List<LineRenderer>();

        for (int i = 0; i < 20; i++) {
            linePool.AddToPool(Instantiate(linePrefab, transform));
            nodePool.AddToPool(Instantiate(nodePrefab, transform));
            doorAnchorPool.AddToPool(Instantiate(doorAnchorPrefab, transform));
        }

        #region cube graph
        //for (int i = 0; i < 8; i++) {
        //    GameObject n = nodePool.DePool();
        //    n.transform.localPosition = new Vector3(rand.Next(-5, 5), rand.Next(-5, 5), rand.Next(-5, 5));
        //    nodes.Add(new Node(n));
        //}

        //edges.Add(new Edge {
        //    u = nodes[0],
        //    v = nodes[1]
        //});
        //edges.Add(new Edge {
        //    u = nodes[1],
        //    v = nodes[2]
        //});
        //edges.Add(new Edge {
        //    u = nodes[2],
        //    v = nodes[3]
        //});
        //edges.Add(new Edge {
        //    u = nodes[3],
        //    v = nodes[0]
        //});
        //edges.Add(new Edge {
        //    u = nodes[4],
        //    v = nodes[5]
        //});
        //edges.Add(new Edge {
        //    u = nodes[5],
        //    v = nodes[6]
        //});
        //edges.Add(new Edge {
        //    u = nodes[6],
        //    v = nodes[7]
        //});
        //edges.Add(new Edge {
        //    u = nodes[7],
        //    v = nodes[4]
        //});
        //edges.Add(new Edge {
        //    u = nodes[0],
        //    v = nodes[4]
        //});
        //edges.Add(new Edge {
        //    u = nodes[1],
        //    v = nodes[5]
        //});
        //edges.Add(new Edge {
        //    u = nodes[2],
        //    v = nodes[6]
        //});
        //edges.Add(new Edge {
        //    u = nodes[3],
        //    v = nodes[7]
        //});

        //foreach(Edge e in edges) {
        //    lineList.Add(linePool.DePool().GetComponent<LineRenderer>());
        //}

        #endregion
    }

    public void UpdateGraph() {

        edges.Clear();
        //foreach (RoomPassage p in passages) {
        //    if (!roomDoorNodes.ContainsKey(p.src)) {
        //        Node newNode = new Node(Instantiate(nodePrefab, transform));
        //        newNode.roomDoor = p.src;
        //        nodes.Add(newNode);
        //        roomDoorNodes.Add(p.src, newNode);
        //    }
        //    if (!roomDoorNodes.ContainsKey(p.dst)) {
        //        Node newNode = new Node(Instantiate(nodePrefab, transform));
        //        newNode.roomDoor = p.dst;
        //        nodes.Add(newNode);
        //        roomDoorNodes.Add(p.dst, newNode);
        //    }

        //    edges.Add(new Edge {
        //        u = roomDoorNodes[p.src],
        //        v = roomDoorNodes[p.dst]
        //    });
        //}


        foreach(RoomPassage p in passages) {
            if (!roomNodes.ContainsKey(p.src.room)) {
                GameObject nodeObj = nodePool.DePool();
                Node newNode = new Node(nodeObj, doorAnchorPool, p.src.room);
                objNodeMap.Add(nodeObj, newNode);
                nodes.Add(newNode);
                roomNodes.Add(p.src.room, newNode);
                newNode.nodeObject.transform.localPosition = new Vector3(rand.Next(-5, 5), rand.Next(-5, 5), rand.Next(-5, 5));
                newNode.nodeObject.GetComponent<NodeDisplay>().text.SetText(p.src.room.roomName);
            }
            if (!roomNodes.ContainsKey(p.dst.room)) {
                GameObject nodeObj = nodePool.DePool();
                Node newNode = new Node(nodeObj, doorAnchorPool, p.dst.room);
                objNodeMap.Add(nodeObj, newNode);
                nodes.Add(newNode);
                roomNodes.Add(p.dst.room, newNode);
                newNode.nodeObject.transform.localPosition = new Vector3(rand.Next(-5, 5), rand.Next(-5, 5), rand.Next(-5, 5));
                newNode.nodeObject.GetComponent<NodeDisplay>().text.SetText(p.dst.room.roomName);
            }

            Edge e = new Edge { u = roomNodes[p.src.room], v = roomNodes[p.dst.room] };
            Edge ee = new Edge { v = roomNodes[p.src.room], u = roomNodes[p.dst.room] };

            if (edges.Contains(e) && !edges.Contains(ee)) {
                e.w++;
            } else {
                edges.Add(e);
            }
            //if (!edges.Contains(e) && edges.Contains(ee)) {
            //    ee.w++;
            //} else {
            //    edges.Add(ee);
            //}

            if (!edgeDisplays.ContainsKey(p))
                edgeDisplays.Add(p, linePool.DePool().GetComponent<EdgeDisplay>());
        }

            
        t = 1;
        prevPassageCount = passages.Count;
        Debug.Log(edges.Count);
    }

    public float k = 100;
    public float t = 1;
    public bool cool = false;
    int prevPassageCount;
    // Update is called once per frame
    void Update()
    {
        if (prevPassageCount != passages.Count)
            UpdateGraph();

        //Repulsive Force
        foreach (Node v in nodes) {
            Vector3 fr = Vector3.zero;
            v.speed = Vector3.zero;
            foreach (Node u in nodes) {
                if (v != u) {
                    Vector3 v_pos = v.nodeObject.transform.localPosition;
                    Vector3 u_pos = u.nodeObject.transform.localPosition;
                    fr = k * k / Vector3.Distance(v_pos , u_pos) * (v_pos - u_pos).normalized;
                    v.speed += fr;
                }
            }

            //Repulsive from curve controls 
            foreach (EdgeDisplay eDisplay in edgeDisplays.Values) {
                Vector3 v_pos = v.nodeObject.transform.localPosition;
                Vector3 u_pos = eDisplay.control;
                fr = k * k / Vector3.Distance(v_pos, u_pos) * (v_pos - u_pos).normalized;
                if (float.IsNaN(fr.x)) continue;
                //Debug.Log(fr);
                v.speed += fr;
            }
        }

        //Attractive forces from connected nodes
        foreach (Edge e in edges) {
            Vector3 v_pos = e.v.nodeObject.transform.localPosition;
            Vector3 u_pos = e.u.nodeObject.transform.localPosition;
            Vector3 fa = Vector3.SqrMagnitude(v_pos - u_pos) * (v_pos - u_pos).normalized / k * e.w;
            e.v.speed -= fa;
            e.u.speed += fa;
        }


        foreach(Node v in nodes) {
            //apply force assmue m = 1, a = f/m = f, dv = adt;
            v.speed -= v.nodeObject.transform.localPosition * centering;
            v.nodeObject.transform.localPosition += Mathf.Min(t, v.speed.magnitude * Time.deltaTime) * v.speed.normalized * .2f;

            v.nodeObject.transform.rotation = Quaternion.LookRotation(Vector3.forward);
        }

        if(cool)
            t = Mathf.Max(0, t *.99f);

        //For each passage draw a line


        foreach(RoomPassage p in edgeDisplays.Keys) {
            EdgeDisplay edgeDisplay = edgeDisplays[p];
            //edgeDisplay.srcPos =  roomNodes[p.src.room].nodeObject.transform.position + p.src.doorAnchor.direction / 80f;

            edgeDisplay.srcPos = roomNodes[p.src.room].doorAnchorObjects[p.src.doorAnchor].transform.position;

            edgeDisplay.srcHandle = edgeDisplay.srcPos + (edgeDisplay.srcPos - roomNodes[p.src.room].nodeObject.transform.position) * 3f;
            edgeDisplay.dstHandle = edgeDisplay.dstPos + (edgeDisplay.dstPos - roomNodes[p.dst.room].nodeObject.transform.position) * 3f;

            //edgeDisplay.dstPos =  roomNodes[p.dst.room].nodeObject.transform.position + p.dst.doorAnchor.direction / 80f;
            edgeDisplay.dstPos = roomNodes[p.dst.room].doorAnchorObjects[p.dst.doorAnchor].transform.position;

        }

        //for(int i = 0; i < edges.Count; i++) {
        //    LineRenderer l = lineList[i];
        //    l.startWidth = 0.002f;
        //    l.startWidth = 0.002f;
        //    l.SetPosition(0, edges[i].u.nodeObject.transform.position);
        //    l.SetPosition(1, edges[i].v.nodeObject.transform.position);
        //}






        if (focusMode) {
            RoomInfo activeRoomInfo = roomsManager.activeRoom.info;
            CenterRoomNode(activeRoomInfo);
        }


        //Interaction Logic
        if (!drag_mode) {
            foreach (Node n in roomNodes.Values) {
                n.display.Selected = false;
            }
            selectedNode = null;
        }

        RaycastHit hit;
        if (!m_rotateMode) {
            if (!drag_mode) {
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out hit, 10.0f)) {
                    if (hit.transform.GetComponent<NodeDisplay>() != null) {
                        hit.transform.GetComponent<NodeDisplay>().Selected = true;
                        selectedNode = objNodeMap[hit.transform.gameObject];
                    }
                }
            }

            if (selectedNode != null && drag_mode) {
                Debug.Log(selectedNode);
                Vector2 mousePos = Mouse.current.position.ReadValue();
                float objZ = Camera.main.WorldToScreenPoint(selectedNode.nodeObject.transform.position).z;
                Vector3 cursorPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, objZ));
                //Debug.Log(cursorPos);
                //cursorObjOffset = selectedNode.nodeObject.transform.position - cursorPos;
                if (cursorObjOffset != null)
                    selectedNode.nodeObject.transform.position = cursorPos + cursorObjOffset;

            }
        }

        if (m_rotateMode && !drag_mode) {
            transform.Rotate(Vector3.up, -Vector3.Dot(m_rotate, Vector3.right) * 0.2f, Space.World);
            transform.Rotate(Camera.main.transform.right, Vector3.Dot(m_rotate, Vector3.up) * 0.2f, Space.World);
        }

    }
    public bool focusMode = false;
    bool drag_mode = false;
    Node selectedNode;
    Vector3 cursorObjOffset;

    void CenterRoomNode(RoomInfo roomInfo) {
        //center a node
        Vector3 offset = roomNodes[roomInfo].nodeObject.transform.localPosition;
        foreach (Node n in nodes) {
            n.nodeObject.transform.localPosition -= offset;
        }
    }


    //https://arxiv.org/pdf/1201.3011.pdf

    void OnDrawGizmos() {
        //if (nodes != null) {
        //    foreach (Node node in nodes) {
        //        foreach (Node neighbor in node.neighbors) {
        //            Gizmos.DrawLine(node.nodeObject.transform.position, neighbor.nodeObject.transform.position);
        //        }
        //    }
        //}

        if(edges != null) {
            foreach (Edge e in edges) {
                Gizmos.DrawLine(e.u.nodeObject.transform.position, e.v.nodeObject.transform.position);
            }
        }

        if (passages != null) {
            foreach (RoomPassage p in passages) {
                Gizmos.color = Color.red;
                if (p.src.room == p.dst.room)
                    continue;
                if (roomNodes.ContainsKey(p.src.room) && roomNodes.ContainsKey(p.dst.room)) {
                    Vector3 src = roomNodes[p.src.room].nodeObject.transform.position + p.src.doorAnchor.direction/8f;
                    Vector3 dst = roomNodes[p.dst.room].nodeObject.transform.position + p.dst.doorAnchor.direction/8f;
                    Gizmos.DrawLine(src, dst);
                }
            }
        }
    }


    public void OnToggleMap(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started) {
            if (!transform.gameObject.activeSelf) transform.gameObject.SetActive(true);
            StartCoroutine(ToggleMap());
        }
    }

    Vector2 m_rotate;
    public void OnRotateMap(InputAction.CallbackContext context) {
        m_rotate = context.ReadValue<Vector2>();
    }

    bool m_rotateMode; 

    public void OnLeftClick(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started) {
            m_rotateMode = true;
            if (selectedNode != null) {
                Vector2 mousePos = Mouse.current.position.ReadValue();
                float objZ = Camera.main.WorldToScreenPoint(selectedNode.nodeObject.transform.position).z;
                Vector3 cursorPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, objZ));
                cursorObjOffset = selectedNode.nodeObject.transform.position - cursorPos;
                drag_mode = true;
                m_rotateMode = false;
            }
        } else if (context.phase == InputActionPhase.Canceled) {
            m_rotateMode = false;
            drag_mode = false;
            selectedNode = null;
            
        }

        //Debug.Log(context.phase);
    }

    public void OnMouseMove(InputAction.CallbackContext context) {
        //Debug.Log(Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>()));
        //Debug.Log(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
    }


    IEnumerator ToggleMap() {
        if(m_active) {
            m_active = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            yield return null;
            playerInput.SwitchCurrentActionMap("Player");
            transform.gameObject.SetActive(false);
        } else {
            m_active = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            yield return null;
            playerInput.SwitchCurrentActionMap("Map");
        }
    }
    

}


public class Edge {
    public Node u;
    public Node v;
    public float w = 1;
}

public class Node {

    public List<Node> neighbors;
    public Vector3 speed;

    public NodeDisplay display;

    //These are specific
    public GameObject nodeObject;
    public Dictionary<DoorAnchor, GameObject> doorAnchorObjects;
    public RoomDoorAnchor roomDoor;
    public RoomInfo roomInfo;


    public Node(GameObject obj) {
        this.nodeObject = obj;
        this.display = obj.GetComponent<NodeDisplay>();
        this.neighbors = new List<Node>();
    }
    public Node(GameObject obj, GameObjectPool doorAnchorPool, RoomInfo roomInfo) {
        this.nodeObject = obj;
        this.display = obj.GetComponent<NodeDisplay>();
        this.roomInfo = roomInfo;
        this.neighbors = new List<Node>();
        doorAnchorObjects = new Dictionary<DoorAnchor, GameObject>();

        foreach(DoorAnchor da in roomInfo.doorAnchors) {
            GameObject daObject = doorAnchorPool.DePool();
            daObject.transform.parent = nodeObject.transform;
            daObject.transform.localPosition = da.position.normalized/2;
            doorAnchorObjects.Add(da, daObject);
        }
    }


}

