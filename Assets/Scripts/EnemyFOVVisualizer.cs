#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class EnemyFOVVisualizer : MonoBehaviour
{
    private EnemyController enemy;

    private void OnEnable() => enemy = GetComponent<EnemyController>();

    private void OnDrawGizmos()
    {
        if (enemy == null) return;

        float range = enemy.detectionRange;
        float half  = enemy.fieldOfViewAngle * 0.5f;

        Handles.color = enemy.CanSeePlayer()
            ? new Color(1f, 0.3f, 0.1f, 0.15f)
            : new Color(0.2f, 0.8f, 1f, 0.08f);

        Handles.DrawSolidArc(
            transform.position,
            Vector3.up,
            Quaternion.Euler(0, -half, 0) * transform.forward,
            enemy.fieldOfViewAngle,
            range
        );
    }
}
#endif