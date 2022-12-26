using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollRootBone;

    public void Setup(Transform originalRootBone)
    {
        MatchAllChildTransforms(originalRootBone, ragdollRootBone);

        ApplyExplotionToRagdoll(
            ragdollRootBone,
            300f,
            transform.position,
            10f
        );

    }

    private void MatchAllChildTransforms(Transform root, Transform clone)
    {
        foreach (Transform child in root)
        {
            Transform cloneChild = clone.Find(child.name);
            if (cloneChild != null)
            {
                cloneChild.position = child.position;
                cloneChild.rotation = child.rotation;
                MatchAllChildTransforms(child, cloneChild);
            }
        }
    }

    private void ApplyExplotionToRagdoll(
        Transform root, 
        float explosionForce,
        Vector3 explosionPosition,
        float explosionRadius
        )
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(
                    explosionForce,
                    explosionPosition,
                    explosionRadius
                );
            }
            ApplyExplotionToRagdoll(child, explosionForce, explosionPosition, explosionRadius);
        }
    }

}
