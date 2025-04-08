using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Arrow : MonoBehaviour {
    // ───── Serialized Fields ─────
    [Header("Settings")]
    [SerializeField] private float arrowSelfDestructTimeout = 5f;
    
    // ───── Private Fields ─────
    private Rigidbody2D _rb;

    // ───── Unity Methods ─────
    void Start() {
        StartCoroutine(ArrowSelfDestructCoroutine());
    }

    void Update() {
        if (_rb.velocity.magnitude > .1f) {
            float angle = Mathf.Atan2(_rb.velocity.y, _rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    // ───── Public Methods ─────
    public void Shoot(Vector2 target, float launchForce) {
        _rb = GetComponent<Rigidbody2D>();
        
        Vector2 startPos = transform.position;
        Vector2 direction = (target - startPos).normalized;
        
        float initialAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, initialAngle);
        
        _rb.velocity = direction * launchForce;
    }

    // ───── Private Methods ─────
    private IEnumerator ArrowSelfDestructCoroutine() {
        yield return new WaitForSeconds(arrowSelfDestructTimeout);
        Destroy(gameObject);
    }
}