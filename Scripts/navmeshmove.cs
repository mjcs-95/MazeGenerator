using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class navmeshmove : MonoBehaviour
{

    GameObject target;
    bool hasTarget;
    NavMeshAgent pathfinder;
    // Start is called before the first frame update
    float myCollisionRadius;
    float targetCollisionRadius;

    private void Awake() {
        
    }
    void Start()
    {
        hasTarget = true;
        target = GameObject.FindGameObjectWithTag("Player");
        myCollisionRadius = GetComponent<CapsuleCollider>().radius;
        targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

        pathfinder = GetComponent<NavMeshAgent>();
        StartCoroutine(UpdatePath());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator UpdatePath() {
        while (hasTarget) {            
            Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
            Vector3 targetPosition = target.transform.position - dirToTarget*(myCollisionRadius+ targetCollisionRadius / 2);
            pathfinder.SetDestination(targetPosition);            
            yield return new WaitForSeconds(.5f);
        }
    }

}
