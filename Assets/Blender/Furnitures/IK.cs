using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UIElements;
using UnityEngine.XR;

//https://github.com/ditzel/SimpleIK/blob/master/FastIK/Assets/FastIK/Scripts/FastIK/FastIKFabric.cs

public class IK : MonoBehaviour
{
    //public Animator anim;

    public Transform target, pole;

    public int iterations = 10;

    public int chainLength = 3;

    public float delta = 0.001f;
    public float snapBackStrength = 0.1f;
    Quaternion[] boneStartRotations;
    Vector3[] startDirectionSucc;
    Quaternion startRotationRoot = Quaternion.identity;
    Quaternion startRotationTarget;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        ResolveIK();
    }

    private void ResolveIK() {
        if (target == null) return;
        if (boneLengths.Length != chainLength) Init();

        //get current position
        for(int i = 0; i < bones.Length; i ++) {
            positions[i] = bones[i].position;
        }

        var rootRot = (bones[0].parent != null) ? bones[0].rotation : Quaternion.identity;
        var rootRotDiff = rootRot * Quaternion.Inverse(startRotationRoot);

        //edge case: target > max length: stretch all the way
        if ((target.position - bones[0].position).sqrMagnitude >= totalLength * totalLength) {
            var direction = (target.position - positions[0]).normalized;
            for (int i = 1; i < positions.Length; i++) {
                positions[i] = positions[i - 1] + direction * boneLengths[i - 1];
            }
        } else {

            for (int i = 0; i < positions.Length - 1; i++)
                positions[i + 1] = Vector3.Lerp(positions[i + 1], positions[i] + rootRotDiff * startDirectionSucc[i], snapBackStrength);

            for (int itr = 0; itr < iterations; itr++) {
                //back
                for (int i = positions.Length - 1; i > 0; i--) {
                    if (i == positions.Length - 1) {
                        positions[i] = target.position; 
                    } else {
                        positions[i] = positions[i + 1] + (positions[i] - positions[i + 1]).normalized * boneLengths[i];
                    }
                }

                //foward
                for (int i = 1; i < positions.Length; i++)
                    positions[i] = positions[i - 1] + (positions[i] - positions[i - 1]).normalized * boneLengths[i - 1];


                //close enough?
                if ((positions[positions.Length - 1] - target.position).sqrMagnitude < delta * delta)
                    break;
            }
        }

        //Move towards pole
        if (pole) {
            for (int i = 1; i < positions.Length -1; i++) {
                //
                var plane = new Plane(positions[i + 1] - positions[i - 1], positions[i - 1]);
                var projectedPole = plane.ClosestPointOnPlane(pole.position);
                var projectedBone = plane.ClosestPointOnPlane(positions[i]);
                var angle = Vector3.SignedAngle(projectedBone - positions[i - 1], projectedPole - positions[i - 1], plane.normal);
                positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (positions[i]- positions[i - 1]) + positions[i-1];
            }
        }


        //set new position
        for (int i = 0; i < bones.Length; i++) {
            bones[i].position = positions[i];

            if (i == positions.Length - 1)
                bones[i].rotation = target.rotation * Quaternion.Inverse(startRotationTarget) * boneStartRotations[i];
            else
                bones[i].rotation = Quaternion.FromToRotation(startDirectionSucc[i], positions[i + 1] - positions[i]) * boneStartRotations[i];
        }

    }


    void Awake() {
        Init();
    }

    protected Transform[] bones;
    protected Vector3[] positions;
    protected float[] boneLengths;
    protected float totalLength;

    void Init() {
        bones = new Transform[chainLength + 1];
        positions = new Vector3[chainLength + 1];
        boneLengths = new float[chainLength];
        boneStartRotations = new Quaternion[chainLength + 1];
        startDirectionSucc = new Vector3[chainLength + 1];
        totalLength = 0;

        if (target == null) {
            target = new GameObject(gameObject.name + " Target").transform;
            target.position = transform.position;
        }
        startRotationTarget = target.rotation;

        var current = transform;
        for (var i = bones.Length - 1; i >=0; i--) {
            bones[i] = current;
            boneStartRotations[i] = current.rotation;

            if (i == bones.Length - 1) {
                // is leaf
                startDirectionSucc[i] = target.position - current.position;

            } else {

                startDirectionSucc[i] = bones[i+1].position - current.position;
                boneLengths[i] = (bones[i + 1].position - current.position).magnitude;
                totalLength += boneLengths[i];
            }

            current = current.parent;
        }
    }

    //void OnDrawGizmos() {

    //    var current = this.transform;
    //    for(int i = 0; i < chainLength; i++) {
    //        if (current == null || current.parent == null) break;

    //        var scale = Vector3.Distance(current.position, current.parent.position) * .1f;
    //        Handles.matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position), new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
    //        Handles.color = Color.white;
    //        Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
    //        //Handles.color = Color.yellow;
            
    //        current = current.parent;
    //    }

    //}
    
}
