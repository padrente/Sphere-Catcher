using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIControl : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> points;
    [SerializeField]
    private NavMeshAgent agent;
    [SerializeField]
    private GameObject player = null;
    [SerializeField]
    private CharController metoda = null;
    [SerializeField]
    private FoV isInView = null;
    
    // private Vector3 temphold;

    public static bool death = false;

    private int currPoint;


    private void Start()
    {        
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        currPoint = 0;
        agent.destination = points[currPoint].transform.position;
    }


    
    private void Update()
    {

        if (isInView.isInFov  == true)
        {
            // temphold = this.GetComponent<NavMeshAgent>().destination;
            // AddWaypoints();
            agent.speed = 6f;
            agent.destination = player.transform.position;
        }
        if (isInView.isInFov != true)
        {
            agent.speed = 3.5f;
            agent.destination = points[currPoint].transform.position;
        }
        if (Vector3.Distance(this.transform.position, player.transform.position) <= 2f)
        {
            death = true;
            metoda.Respawn();
        }
        if (Vector3.Distance(this.transform.position, points[currPoint].transform.position)<=2f)
        {
            Iterate();
        }
    }
    void Iterate(){
        if (currPoint<points.Count -1){
            currPoint++;

        }
        else currPoint=0;
        agent.destination = points[currPoint].transform.position;
    }
    // void AddWaypoints(){
    //     points.Add(new GameObject());
    //     points[points.Count - 1].transform.position = temphold;
    // }
}
