using UnityEngine;

public class SpermRunnerEnemy : MonoBehaviour
{
    private enum EnemyState
    {
        Patrol,
        Chase
    }

    [SerializeField] private Transform player;
    [SerializeField] private Transform patrolPointA;
    [SerializeField] private Transform patrolPointB;
    [SerializeField] private float moveSpeed = 6.5f;
    [SerializeField] private float detectionRange = 9f;
    [SerializeField] private float chaseRange = 13f;
    [SerializeField] private float losePlayerDelay = 3f;
    [SerializeField] private float arriveDistance = 0.4f;

    private EnemyState state;
    private Transform patrolTarget;
    private float playerOutsideRangeTime;

    private void Start()
    {
        patrolTarget = patrolPointB != null ? patrolPointB : patrolPointA;
    }

    private void Update()
    {
        if (player == null)
            return;

        float playerDistance = Vector3.Distance(transform.position, player.position);
        if (state == EnemyState.Patrol && playerDistance <= detectionRange)
        {
            state = EnemyState.Chase;
            playerOutsideRangeTime = 0f;
        }

        if (state == EnemyState.Chase)
        {
            if (playerDistance > chaseRange)
            {
                playerOutsideRangeTime += Time.deltaTime;
                if (playerOutsideRangeTime >= losePlayerDelay)
                {
                    state = EnemyState.Patrol;
                    playerOutsideRangeTime = 0f;
                }
            }
            else
            {
                playerOutsideRangeTime = 0f;
            }
        }

        Vector3 destination = state == EnemyState.Chase ? player.position : GetPatrolDestination();
        MoveTowards(destination);
    }

    private Vector3 GetPatrolDestination()
    {
        if (patrolTarget == null)
            return transform.position;

        if (Vector3.Distance(transform.position, patrolTarget.position) <= arriveDistance)
            patrolTarget = patrolTarget == patrolPointA ? patrolPointB : patrolPointA;

        return patrolTarget != null ? patrolTarget.position : transform.position;
    }

    private void MoveTowards(Vector3 destination)
    {
        Vector3 direction = destination - transform.position;
        if (direction.sqrMagnitude <= 0.001f)
            return;

        Vector3 movement = direction.normalized * moveSpeed * Time.deltaTime;
        if (movement.sqrMagnitude > direction.sqrMagnitude)
            movement = direction;

        transform.position += movement;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(direction.normalized, Vector3.up),
            8f * Time.deltaTime);
    }
}
