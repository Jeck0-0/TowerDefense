using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class Projectile : MonoBehaviour
{
    public event EventHandler<Health.DamageArgs> OnHit;

    public Shooting shotBy; //
    public Transform target;
    public bool canFindNewTarget; 
    public float speed;
    public float turnSpeed; 
    public float damage; 
    public float explosionRange; //
    public float explosionDamagePerc; //
    public float explosionDamageFixed; //
    public float dist;
    public float curveHardness;
    public bool moveAround;
    public AnimationCurve curve;



    void MoveTowardsTarget()
    {
        Vector3 vectorToTarget = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, turnSpeed * Time.fixedDeltaTime);

        transform.position += transform.right * Time.fixedDeltaTime * speed;
    }

    void MoveAroundTarget()
    {
        Vector3 vectorToTarget = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;

        float distance = Vector2.Distance(transform.position, target.position);
        angle += curve.Evaluate((distance - dist) * curveHardness) * 180 + 90;

        //transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.fixedDeltaTime * turnSpeed);

        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, turnSpeed * Time.fixedDeltaTime);
        transform.position += transform.up * Time.fixedDeltaTime * speed;

        //transform.position += q.eulerAngles. * Time.fixedDeltaTime * speed;
        //transform.
        //transform.Translate(//new Vector3(UnityEngine.Random.Range(-.01f, .01f), UnityEngine.Random.Range(-.01f, .01f)));
        //new Vector3(
        //   Mathf.Cos((vectorToTarget.y + dist) * Mathf.Deg2Rad),
        //    Mathf.Sin((vectorToTarget.x + dist) * Mathf.Deg2Rad)) * speed * Time.fixedDeltaTime);
    }
    private void OnDrawGizmos()
    {
        if (target)
        {
            Color col = Gizmos.color;
            col.a = .1f;
            Gizmos.color = col;
            Gizmos.DrawLine(transform.position, target.position);
            Gizmos.color = Color.white;
        }
        Gizmos.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * explosionRange);

    }
    bool UpdateTarget()
    {
        Enemy nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;
        foreach (Enemy e in Enemy.enemies)
        {
            float dist = Vector2.Distance(transform.position, e.transform.position);
            if (dist < nearestDistance)
            {
                nearestEnemy = e;
                nearestDistance = dist;
            }
        }
        target = nearestEnemy?.transform;
        return target;
    }
    private void Start()
    {
    }

    void Update()
    {
        if (!target)
        {
            if (canFindNewTarget)
            {
                if (!UpdateTarget())
                {
                    Bonk();
                    return;
                }
            }
            else
            {
                Bonk();
                return;
            }
        }

        //Vector3 vectorToTarget = (target.position - transform.position).normalized;
        //float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        //Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

        //transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * turnSpeed);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, q, turnSpeed * Time.deltaTime);


        //transform.position += new Vector3(
        //    Mathf.Cos((transform.rotation.eulerAngles.z + sium) * Mathf.Deg2Rad),
        //    Mathf.Sin((transform.rotation.eulerAngles.z + sium) * Mathf.Deg2Rad)) * speed * Time.deltaTime;

    }

    void FixedUpdate()
    {
        if (!target)
            return;

        if (moveAround)
            MoveAroundTarget();
        else
            MoveTowardsTarget();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform != target)
            return;

        if (collision.TryGetComponent(out Health h))
            HitTarget(h);
    }


    void HitTarget(Health targetHealth)
    {
        Health.DamageArgs args = targetHealth.Damage(damage);
        OnHit?.Invoke(this, args);
        Bonk();
    }

    void Bonk()
    {
        Destroy(gameObject);
    }
}
