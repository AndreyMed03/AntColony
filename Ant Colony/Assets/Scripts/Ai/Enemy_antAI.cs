using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_antAI : MonoBehaviour
{
    [Header("Patrolling settings")]
    [Tooltip("Радиус, в котором выбирается следующая точка относительно текущей позиции муравья")]
    public float patrolRadius = 10f;

    [Tooltip("На каком расстоянии от цели считать, что муравей достиг точки")]
    public float pointReachedThreshold = 1.5f;

    [Tooltip("Пауза между достижением точки и выбором новой")]
    public float wanderDelay = 1.5f;

    private NavMeshAgent agent;
    private Vector3 targetPosition;
    private float waitTimer;
    private bool isInitialized = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Ожидаем, пока муравей окажется на NavMesh перед инициализацией
        if (!isInitialized && NavMesh.SamplePosition(transform.position, out _, 1f, NavMesh.AllAreas))
        {
            isInitialized = true;
            PickNewDestination();
        }

        if (!isInitialized) return;

        if (!agent.pathPending && agent.remainingDistance <= pointReachedThreshold)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= wanderDelay)
            {
                PickNewDestination();
                waitTimer = 0f;
            }
        }
    }

    void PickNewDestination()
    {
        // Центр патрулирования — текущая позиция муравья
        Vector3 center = transform.position;
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection.y = 0f; // исключаем высоту, чтобы не улетал вверх/вниз
        Vector3 randomPoint = center + randomDirection;

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit navHit, patrolRadius, NavMesh.AllAreas))
        {
            targetPosition = navHit.position;
            agent.SetDestination(targetPosition);
        }
    }
    //public NavMeshAgent agent;

    //public Transform player;

    //public LayerMask whatIsGround, whatIsPlayer;

    //Patroling
    //public Vector3 walkPoint;
    //bool walkPointSet;
    //public float walkPointRange;

    //States
    //public float sightRange;
    //public bool playerInSightRange;

    //private void Awake()
    //{
    //    agent = GetComponent<NavMeshAgent>();
    //    agent.avoidancePriority = Random.Range(50, 100);
    //}

    //private void Update()
    //{
    //    if (player == null && AntMovement.Instance != null)
    //        player = AntMovement.Instance;

    //    if (player == null) return;

    //    playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
    //    playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

    //    if (!playerInSightRange) Patroling();
    //    if (playerInSightRange) ChasePlayer();
    //    if (playerInAttackRange && playerInSightRange) StopMoving();
    //}

    //private void Patroling()
    //{
    //    if (!walkPointSet) SearchWalkPoint();

    //    if (walkPointSet)
    //        agent.SetDestination(walkPoint);

    //    Vector3 distanceToWalkPoint = transform.position - walkPoint;

    //    Walkpoint reached
    //    if (distanceToWalkPoint.magnitude < 1f)
    //        walkPointSet = false;
    //}

    //private void SearchWalkPoint()
    //{
    //    float randomZ = Random.Range(-walkPointRange, walkPointRange);
    //    float randomX = Random.Range(-walkPointRange, walkPointRange);

    //    walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

    //    if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
    //        walkPointSet = true;
    //}

    //private void ChasePlayer()
    //{
    //    if (Vector3.Distance(agent.destination, player.position) > 0.5f)
    //    {
    //        agent.SetDestination(player.position);
    //    }
    //}

    //private void StopMoving()
    //{
    //    agent.SetDestination(transform.position);
    //}

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, attackRange);
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, sightRange);
    //}
}
