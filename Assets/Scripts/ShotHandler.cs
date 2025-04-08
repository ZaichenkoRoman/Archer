using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class ShotHandler : MonoBehaviour
{
    // ───── Serialized Fields ─────
    [Header("Prefabs & References")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject trajectoryPointPrefab;
    [SerializeField] private GameObject target;
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private Transform targetingPoint;
    [SerializeField] private InputReader input;

    [Header("Settings")]
    [SerializeField] private float launchForce = 10f;
    [SerializeField] private float arrowSpawnOffset = 0.5f;
    [SerializeField] private int trajectoryPointCount = 20;

    // ───── Private Fields ─────
    private bool _isDragging;
    private float _distance;
    private Vector2 _startPoint, _endPoint, _direction, _force;
    private Vector2 _initialArrowSpawnPosition;
    private Vector2 _targetingPoint;
    private Camera _camera;
    private List<GameObject> _trajectoryPoints;

    // ───── Unity Methods ─────
    private void Start()
    {
        _camera = Camera.main;
        _trajectoryPoints = new List<GameObject>();
        _initialArrowSpawnPosition = arrowSpawnPoint.position;
        _targetingPoint = targetingPoint.position;

        GenerateTrajectoryPoints();

        SetAnimation("idle", true);
    }

    private void OnEnable()
    {
        input.OnTouchStart += HandleTouchStart;
        input.OnTouchEnd += HandleTouchEnd;
        input.OnTouchPositionChanged += HandleTouchPositionChanged;
    }

    private void OnDisable()
    {
        input.OnTouchStart -= HandleTouchStart;
        input.OnTouchEnd -= HandleTouchEnd;
        input.OnTouchPositionChanged -= HandleTouchPositionChanged;
    }

    private void Update()
    {
        if (_isDragging)
        {
            ShowTrajectory();
        }
        else
        {
            HideTrajectory();
            arrowSpawnPoint.position = _initialArrowSpawnPosition;
        }
    }

    // ───── Input Handlers ─────
    private void HandleTouchStart()
    {
        _startPoint = _targetingPoint;
        _isDragging = true;
        SetAnimation("attack_start", true);
    }

    private void HandleTouchEnd()
    {
        _isDragging = false;

        Vector2 spawnPosition = arrowSpawnPoint.position;
        Arrow arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity).GetComponent<Arrow>();
        arrow.Shoot(target.transform.position, launchForce * _distance);

        SetAnimation("attack_target", false);
        SetAnimation("idle", true);

        arrowSpawnPoint.position = _initialArrowSpawnPosition;
    }

    private void HandleTouchPositionChanged(Vector2 screenPos)
    {
        if (!_isDragging) return;

        _endPoint = _camera.ScreenToWorldPoint(screenPos);
        _distance = Vector2.Distance(_startPoint, _endPoint);
        _direction = (_startPoint - _endPoint).normalized;
        _force = _direction * (_distance * launchForce);

        target.transform.position = _endPoint + _force;
        arrowSpawnPoint.position = _targetingPoint + _direction * arrowSpawnOffset;

        Debug.DrawLine(_startPoint, _endPoint, Color.green);

        SetAnimation("attack_start", false);
        SetAnimation("attack_target", true);
    }

    // ───── Trajectory ─────
    private void GenerateTrajectoryPoints()
    {
        for (int i = trajectoryPointCount; i >= 0; i--)
        {
            GameObject point = Instantiate(trajectoryPointPrefab, transform.position, Quaternion.identity);
            float scale = i / (float)trajectoryPointCount;
            point.transform.localScale = Vector3.one * scale;
            point.SetActive(false);
            _trajectoryPoints.Add(point);
        }
    }

    private void ShowTrajectory()
    {
        Vector2 startPosition = arrowSpawnPoint.position;
        Vector2 velocity = _force;
        float timeStep = 0.1f;
        float gravity = Physics.gravity.y;

        for (int i = 1; i < trajectoryPointCount; i++)
        {
            float t = i * timeStep;
            float x = startPosition.x + velocity.x * t;
            float y = startPosition.y + velocity.y * t + 0.5f * gravity * t * t;

            _trajectoryPoints[i].transform.position = new Vector2(x, y);
            _trajectoryPoints[i].SetActive(true);
        }
    }

    private void HideTrajectory()
    {
        foreach (var point in _trajectoryPoints)
        {
            point.SetActive(false);
        }
    }

    // ───── Animation ─────
    private void SetAnimation(string name, bool loop)
    {
        if (skeletonAnimation != null)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, name, loop);
        }
    }
}
