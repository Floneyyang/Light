using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : Enemy
{
    [Header("Vision Settings")]
    public float viewRadius;
    public float blindDetectRadius;
    [Range(0, 360)]
    public float viewAngle;
    public float maskCutawayDst = 0.25f;
    public float meshResolution = 1;
    public int edgeResolveIterations = 4;
    public float edgeDstThreshold = .5f;
    public MeshFilter viewMeshFilter;
    public MeshRenderer renderer;
    Mesh viewMesh;

    [Header("Vision Material Reference")]
    public Material patrolMat;
    public Material detectMat;
    public Material attackMat;

    [Header("Layer Reference")]
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    private GameObject hiddenTarget;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    //Timer settings
    private bool startTimer;
    private float timer = 0;
    private bool startTimer2;
    private float timer2 = 0;
    private bool startTimer3;
    private float timer3 = 0;



    private Transform myplayer;



    //Vision Setup
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

    void DrawFieldOfView()
    {
        int count = Mathf.RoundToInt(viewAngle * meshResolution);
        float AngleSize = viewAngle / count;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i <= count; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + AngleSize * i;
            //Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true)* viewRadius, Color.red);
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }

            }


            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }


        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i] + Vector3.forward * maskCutawayDst);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    ViewCastInfo ViewCast(float angle)
    {
        Vector3 dir = DirFromAngle(angle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, angle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, angle);
        }
    }

    private void Start()
    {

        SetUpVision();
        myplayer = GameObject.FindWithTag("Player").transform;
        if(myplayer == null) myplayer = GameObject.FindWithTag("Hidden").transform;
        StartCoroutine(FindTargetsWithDelay());
    }

    private void Update()
    {
        if (startTimer)
        {
            timer += Time.deltaTime;
        }

        if (startTimer2)
        {
            timer2 += Time.deltaTime;
        }

        if (startTimer3)
        {
            timer3 += Time.deltaTime;
            /*
            if (timer3 >= damageRate)
            {
                playerhealth.UpdateHealth(1f);
                timer3 = 0;
            }*/
        }

        //SwitchTimer += Time.deltaTime;



        if (state == EnemyState.Detect || state == EnemyState.Attack)
        {
            transform.LookAt(myplayer.position + Vector3.up * transform.position.y);

        }

        //visionUpdate();
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }


    public void SetUpVision()
    {
        viewMesh = new Mesh();
        viewMesh.name = "Vision";
        viewMeshFilter.mesh = viewMesh;
        renderer.material = patrolMat;
    }

    //Vision Update
    IEnumerator FindTargetsWithDelay()
    {
        while (true)
        {
            yield return new WaitForSeconds(.2f);
            visionUpdate();
        }
    }

    void visionUpdate()
    {
        /*
        if (enemyType == EnemyType.Switch)
        {
            if (SwitchTimer >= OnRate || state == EnemyState.Attack || state == EnemyState.Detect)
            {
                //viewRadius = 6.5f;//smoothtransaction
                StartCoroutine(GrowVision());
            }

            if (SwitchTimer >= OnRate + 3f && (state == EnemyState.Patrol || state == EnemyState.Static))
            {
                StartCoroutine(CloseVision());
                SwitchTimer = 0;
            }
        }*/

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        if (state == EnemyState.Patrol || state == EnemyState.Static)
        {
            if (Mathf.Abs(transform.position.x - myplayer.position.x) <= detectionRadius
                && Mathf.Abs(transform.position.z - myplayer.position.z) <= detectionRadius
                && myplayer.tag == "Player")
            {
                //Debug.Log("in 0.1");
                state = EnemyState.Detect;
                StartCoroutine(detect_player());
                timer = 0;
                startTimer = true;
            }


        }

        if (state == EnemyState.Detect)
        {
            //Debug.Log("in 0.3");

            transform.LookAt(myplayer.position + Vector3.up * transform.position.y);
            //Debug.Log("timer :" + timer);

            if (timer > 0.8f)
            {
                if (myplayer.tag == "Player")
                {
                    timer = 0;
                    startTimer = false;
                    state = EnemyState.Attack;
                    startTimer2 = true;
                    startTimer3 = true;
                    StartCoroutine(attack_player());
                }

                if (myplayer.tag == "Hidden")
                {
                    if (Mathf.Abs(transform.position.x - myplayer.position.x) <= detectionRadius
                        && Mathf.Abs(transform.position.z - myplayer.position.z) <= detectionRadius)
                    {
                        //Debug.Log("in 0.2");
                        timer = 0;
                        startTimer = false;
                        state = EnemyState.Attack;
                        StartCoroutine(attack_player());
                        startTimer2 = true;
                        startTimer3 = true;
                    }
                }

                if (state != EnemyState.Attack
                    && (Mathf.Abs(transform.position.x - myplayer.position.x) > viewRadius
                    || Mathf.Abs(transform.position.z - myplayer.position.z) > viewRadius))
                {
                    state = EnemyState.Patrol;
                    timer = 0;
                    startTimer = false;
                    timer2 = 0;
                    startTimer2 = false;
                    timer3 = 0;
                    startTimer3 = false;
                    StartCoroutine(player_left_range());
                }
            }


        }


        /********* targets in visionmesh **********/

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            Vector3 visibleTargetPos = new Vector3(0, -100, 0);
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                    visibleTargetPos = target.position;
                }


                if (myplayer.position == visibleTargetPos)
                {
                    if ((state == EnemyState.Patrol || state == EnemyState.Static) && myplayer.tag == "Player")
                    {
                        //Debug.Log("in 1");

                        state = EnemyState.Detect;
                        StartCoroutine(detect_player());
                        timer = 0;
                        startTimer = true;
                    }



                    if (state == EnemyState.Attack)
                    {
                        //Debug.Log("in 3");

                        timer2 = 0;
                        startTimer3 = true;

                    }

                }

                if (myplayer.position != visibleTargetPos)
                {

                    if (state == EnemyState.Detect)
                    {
                        //Debug.Log("in 4");
                        //Debug.Log("detect player");
                        startTimer = false;
                        timer = 0;
                        state = EnemyState.Patrol;
                        StartCoroutine(player_left_range());

                    }

                    if (state == EnemyState.Attack)
                    {
                        //Debug.Log("in 5");
                        //Debug.Log("timer2: "+ timer2);
                        startTimer3 = false;
                        if (timer2 >= 6f
                            || Mathf.Abs(transform.position.x - myplayer.position.x) > viewRadius
                            || Mathf.Abs(transform.position.z - myplayer.position.z) > viewRadius)
                        {
                            state = EnemyState.Patrol;
                            timer2 = 0;
                            startTimer2 = false;
                            timer3 = 0;
                            startTimer3 = false;
                            StartCoroutine(player_left_range());
                        }

                    }
                }
            }


        }






    }


    //Change State: vision change

    private IEnumerator detect_player()
    {
        //DetectAudio.Play();
        yield return new WaitForSeconds(0.3f);
        renderer.material = detectMat;

    }

    private IEnumerator attack_player()
    {
        yield return new WaitForSeconds(0.1f);
        renderer.material = attackMat;
    }

    private IEnumerator player_left_range()
    {
        //transition = true;
        yield return new WaitForSeconds(2f);
        renderer.material = patrolMat;
        //transition = false;
    }



    private IEnumerator GrowVision()
    {
        float finalRadius = 6.5f;

        while (viewRadius < finalRadius)
        {
            viewRadius += 0.1f;
            yield return new WaitForSeconds(0.015f);
        }
    }

    private IEnumerator CloseVision()
    {
        float finalRadius = 0f;

        while (viewRadius > finalRadius)
        {
            viewRadius -= 0.1f;
            yield return new WaitForSeconds(0.015f);
        }
    }



}


