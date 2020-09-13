using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Actor
{
    public enum EnemyState
    {
        Static,
        Patrol,
        Detect,
        Attack
    }

    [Header("Enemy General Settings")]
    public EnemyState state = EnemyState.Patrol;

    [Header("Enemy Features")]
    public float detectionRadius;
    public float damageAmount = 1f;

    [Header("Enemy Vision Settings")]
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    public float maskCutawayDst = 0.25f;
    public float meshResolution;
    public int edgeResolveIterations;
    public float edgeDstThreshold;
    public MeshFilter viewMeshFilter;
    public MeshRenderer renderer;

    Mesh viewMesh;

    [Header("Enemy Vision Material Reference")]
    public Material patrolMat;
    public Material detectMat;
    public Material attackMat;

    [Header("Layer Reference")]
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public LayerMask keyMask;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool hit_in, Vector3 point_in, float dst_in, float angle_in)
        {
            hit = hit_in;
            point = point_in;
            dst = dst_in;
            angle = angle_in;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    void SetUpEnemyVision()
    {
        viewMesh = new Mesh();
        viewMesh.name = "Enemy Vision";
        viewMeshFilter.mesh = viewMesh;
    }

    private void Start()
    {
        SetUpEnemyVision();
    }

    IEnumerator FindTargetsWithDelay()
    {
        while (true)
        {
            yield return new WaitForSeconds(.2f);
            enemyStateUpdate();
        }
    }

    virtual public void enemyStateUpdate()
    {
        visibleTargets.Clear();
       
    }



}
