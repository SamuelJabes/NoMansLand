using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAnimation : MonoBehaviour
{
    public Animator animator;
    private NavMeshAgent agent;
    private Vector2 lastMoveDir = Vector2.down;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        // Se usa NavMesh2D:
        Vector2 vel = new Vector2(agent.velocity.x, agent.velocity.y);

        if (vel.sqrMagnitude > 0.01f)
            lastMoveDir = vel.normalized;

        animator.SetFloat("Xinput", lastMoveDir.x);
        animator.SetFloat("Yinput", lastMoveDir.y);
        animator.SetFloat("Speed", vel.magnitude);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
            agent.isStopped = true;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
            agent.isStopped = false;
    }
}
