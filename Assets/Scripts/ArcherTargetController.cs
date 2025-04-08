using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

public class ArcherTargetController : MonoBehaviour {
    
    // ───── Serialized Fields ─────
    [Header("Prefabs & References")]
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    [SerializeField] private Transform target;
    
    [Header("Settings")]
    [SerializeField] private float minAngle = -90f;
    [SerializeField] private float maxAngle = 90f;

    // ───── Constants ─────
    private const string GUN_BONE_NAME = ("gun");

    // ───── Private Fields ─────
    private Bone GunBone { get; set; }

    // ───── Unity Methods ─────
    void Awake() {
        GunBone = skeletonAnimation.Skeleton.FindBone(GUN_BONE_NAME); // Название кости в Spine
    }
    void OnEnable() {
        skeletonAnimation.UpdateLocal += OnSpineUpdate;
    }

    void OnDisable() {
        skeletonAnimation.UpdateLocal -= OnSpineUpdate;
    }

    // ───── Private Methods ─────
    private void OnSpineUpdate(ISkeletonAnimation animated) {
        if (GunBone == null || target == null) return;

        Vector3 direction = target.position - skeletonAnimation.transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        float desiredLocalAngle = (targetAngle - GunBone.Parent.WorldRotationX) * 3;
        
        float clampedAngle = Mathf.Clamp(desiredLocalAngle, minAngle, maxAngle);
        
        GunBone.Rotation = clampedAngle;
    }
}