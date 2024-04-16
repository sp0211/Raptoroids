using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeLegBehavior : MonoBehaviour
{
    Transform parentTransform;

    [SerializeField] bool isLeftLeg = false;
    float legDirection = -1f;

    EnemyHealth centipedeHP;
    int legHP = 6;

    const float moveSpeed = 1f;
    const float attackMoveSpeed = 4f;
    [SerializeField] float bodySeparationDistance = 1f;
    Vector2 originalPosition;

    bool attackStarted = false;
    bool alreadyHitPlayer = false;

    float distanceTraveled = 0f;

    BezierCurve returnCurve = null;
    const float returnTime = 3f;
    float timeSinceReturnStart = 0;

    Action stateUpdate = null;

    void Awake()
    {
        parentTransform = transform.parent;
        legDirection = isLeftLeg ? 1f : -1f;
        originalPosition = transform.position;
        centipedeHP = GetComponentInParent<EnemyHealth>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (stateUpdate != null)
        {
            stateUpdate();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!alreadyHitPlayer && other.tag == "Player")
        {
            other.GetComponent<IBulletHittable>().OnBulletHit();
            alreadyHitPlayer = true;
        }
    }

    public void NotifyLegHit()
    {
        int damage = attackStarted ? 2 : 1;
        legHP -= damage;

        if (legHP <= 0)
        {
            centipedeHP.TakeDamage(1);
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    public void TryStartAttack()
    {
        if (attackStarted)
        {
            return;
        }
        
        StartCoroutine(AttackSequence());
    }

    IEnumerator AttackSequence()
    {
        attackStarted = true;

        transform.parent = null;
        stateUpdate = MoveFromBody;
        while (distanceTraveled < bodySeparationDistance)
        {
            yield return new WaitForEndOfFrame();
        }

        stateUpdate = TrackPlayer;
        yield return new WaitForSeconds(2);

        stateUpdate = Attack;
        while (transform.position.y > CombatStageManager.Instance.VerticalLowerBound)
        {
            yield return new WaitForEndOfFrame();
        }

        transform.parent = parentTransform;
        
        List<Vector2> path = new List<Vector2>() {  transform.position, originalPosition };

        Vector2 mid = ((Vector2)transform.position + originalPosition) / 2f;
        Vector2 perp = Vector2.Perpendicular((Vector2)transform.position - originalPosition) * legDirection * 0.5f;
        Vector2 curvePoint = mid + perp;
        path.Insert(1, curvePoint);

        returnCurve = new BezierCurve(path.ToArray());
        stateUpdate = ReturnToBody;
        
        while (timeSinceReturnStart <= returnTime)
        {
            yield return new WaitForEndOfFrame();
        }
        stateUpdate = null;
        transform.localEulerAngles = Vector3.zero;
        transform.position = originalPosition;

        // Cooldown
        yield return new WaitForSeconds(3);

        attackStarted = false;
        distanceTraveled = 0;
        timeSinceReturnStart = 0;
        returnCurve = null;
    }

    void MoveFromBody()
    {
        float distance = moveSpeed * Time.deltaTime;
        distanceTraveled += distance;

        Vector2 dir = Vector2.up * distance * legDirection;
        transform.Translate(dir);
    }

    void TrackPlayer()
    {
        LookAtTarget(CombatStageManager.Instance.PlayerTransform.position);
    }

    void Attack()
    {
        transform.Translate(Vector2.up * attackMoveSpeed * Time.deltaTime * legDirection);
    }

    void ReturnToBody()
    {
        timeSinceReturnStart += Time.deltaTime;
        transform.position = returnCurve.GetPosition(timeSinceReturnStart / returnTime);
    }

    void LookAtTarget(Vector3 target)
    {
        Vector2 direction = target - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x);
        angle += -legDirection * (Mathf.PI / 2f);
        angle *= Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}