using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SpermRunnerPlayerCollision : MonoBehaviour
{
    [SerializeField] private SpermRunnerGameController gameController;
    [SerializeField] private float hazardCheckRadius = 0.9f;
    [SerializeField] private LayerMask hazardLayer;

    private void Update()
    {
        if (Physics.CheckSphere(transform.position, hazardCheckRadius, hazardLayer, QueryTriggerInteraction.Collide))
        {
            gameController.Die();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<SpermRunnerHazard>() != null)
        {
            gameController.Die();
            return;
        }

        if (other.GetComponent<CenterPoint>() != null)
        {
            other.gameObject.SetActive(false);
            gameController.RevealGoals();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.GetComponentInParent<SpermRunnerHazard>() != null)
        {
            gameController.Die();
            return;
        }

        if (hit.collider.TryGetComponent(out SpermRunnerGoal goal))
        {
            gameController.ReachGoal(goal);
            return;
        }
    }
}