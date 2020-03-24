using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMoveScript : MonoBehaviour
{
    [Header("Control")]
    public float triggerDist = 8f;
    public float maxDegrees = 45f;

    [Header("NavMesh")]
    public GameObject waypoint;
    public GameObject[] waypoints;

    [Space(10)]

    private NavMeshAgent navAgent;

    [Header("GameObjects")]
    public Transform turret;

    [Header("Debug")]
    private GameObject target;
    private EnemyShootScript shootScript;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        shootScript = GetComponent<EnemyShootScript>();

        waypoints = GameObject.FindGameObjectsWithTag("WayPoint");
    }

    void Update()
    {
        if (target != null)
        {
			// Chase Target
            navAgent.SetDestination(target.transform.position);
            navAgent.isStopped = (target.transform.position - transform.position).magnitude < triggerDist;

			// Rotate Turret
            Vector3 targetDir = target.transform.position - transform.position;
            turret.rotation = Quaternion.RotateTowards(turret.rotation, Quaternion.LookRotation(targetDir), Time.time * maxDegrees * Mathf.Deg2Rad);

			// Enable Shooting
            shootScript.canShoot = Mathf.Abs(turret.rotation.y / Quaternion.LookRotation(targetDir).y) == 1f;
        } else
        {
            navAgent.isStopped = false;

            if (waypoint == null)
            {
				// Choose a Waypoint
                waypoint = waypoints[Random.Range(0, waypoints.Length)];
                navAgent.SetDestination(waypoint.transform.position);
            } else
            {
				// Reset at Target
                if ((waypoint.transform.position - transform.position).magnitude < 5) waypoint = null;
            }

			// Look Forward
            turret.localRotation = Quaternion.RotateTowards(turret.localRotation, Quaternion.identity, Time.time * maxDegrees * Mathf.Deg2Rad);

			// Disable Shooting
            shootScript.canShoot = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
		// Target Player
        if (other.gameObject.tag == "Player") target = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
		// Check for Player
        if (ReferenceEquals(other.gameObject, target))
        {
            target = null;

			// Choose a Waypoint
            waypoint = waypoints[Random.Range(0, waypoints.Length)];
            navAgent.SetDestination(waypoint.transform.position);
        }
    }

    void OnDrawGizmosSelected()
    {
		// Render Sphere at Destination
        Gizmos.color = new Color(255, 0, 255);
        Gizmos.DrawSphere(navAgent.destination, 1f);
    }
}