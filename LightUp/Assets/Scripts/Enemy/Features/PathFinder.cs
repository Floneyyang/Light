using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Vision))]
public class PathFinder : MonoBehaviour
{
    public Transform[] locations;
    public int startPos;
    private int index;
    private Transform myplayer;
    private Quaternion initialRotation;
    private Vision vision;
    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        vision = this.GetComponent<Vision>();
        initialRotation = this.transform.rotation;
        if (vision.state != Enemy.EnemyState.Static && locations.Length > 0)
        {
            if (startPos < locations.Length)
            {
                agent.SetDestination(locations[startPos].position);
            }

        }
        Debug.Log(vision == null);
        myplayer = GameObject.FindWithTag("Player").transform;
        if(myplayer == null) myplayer = GameObject.FindWithTag("Hidden").transform;
        index = startPos;
    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log(locations[index].position);
        //Debug.Log(transform.position);
        //Debug.Log(transform.position.x == locations[index].position.x);
        //Debug.Log(transform.position.z == locations[index].position.z);
        //Debug.Log(this.gameObject.GetComponent<EnemyVision>().state);
        Debug.Log(vision.state);
        if (vision.state == Enemy.EnemyState.Patrol)
        {
            if (locations.Length > 0)
            {
                if (index < locations.Length)
                {
                    if ((Mathf.Abs(transform.position.x - locations[index].position.x) <= 0.5f && Mathf.Abs(transform.position.z - locations[index].position.z) <= 0.5f)
                || agent.speed == 5.95f || agent.speed == 4f || locations.Length == 1)
                    {
                        //Debug.Log(index);
                        agent.speed = 3.5f;
                        index++;
                        if (index == locations.Length)
                        {
                            index = 0;
                        }

                        //Debug.Log(locations[index].position);
                        agent.SetDestination(locations[index].position);

                    }
                }

            }



        }
        else if (vision.state == Enemy.EnemyState.Attack)
        {
            agent.speed = 4f;
            agent.SetDestination(myplayer.position);
        }
        else
        {
            agent.velocity = Vector3.zero;
        }
    }

}
