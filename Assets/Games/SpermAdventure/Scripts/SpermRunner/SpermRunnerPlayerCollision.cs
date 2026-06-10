using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SpermRunnerPlayerCollision : MonoBehaviour
{
    [SerializeField] private SpermRunnerGameController gameController;

    private void OnTriggerEnter(Collider other)
    {

        if (other.GetComponent<CenterPoint>() != null)
        {
            other.gameObject.SetActive(false);
            gameController.RevealGoals();
        }

    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.GetComponent<SpermRunnerHazard>() != null)
            gameController.Die();
        if (hit.collider.TryGetComponent(out SpermRunnerGoal goal))
        {
            gameController.ReachGoal(goal);
            return;
        }
    }
}
