using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class Projectile : MonoBehaviour
{
    public event EventHandler<Health> OnHit;

    public Shooting shotBy; //
    public Transform target;
    public bool canFindNewTarget; 
    public float speed;
    public float turnSpeed; 
    public float damage; 
    public float explosionRange; //
    public float explosionDamagePerc; //
    public float explosionDamageFixed; //

    

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

        Vector3 vectorToTarget = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        //transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * turnSpeed);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, turnSpeed * Time.deltaTime);

        transform.position += transform.right * Time.deltaTime * speed;

        //transform.position += new Vector3(
        //    Mathf.Cos((transform.rotation.eulerAngles.z + sium) * Mathf.Deg2Rad),
        //    Mathf.Sin((transform.rotation.eulerAngles.z + sium) * Mathf.Deg2Rad)) * speed * Time.deltaTime;

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
        targetHealth.Damage(damage);
        Bonk();
        OnHit?.Invoke(this, targetHealth);
    }

    void Bonk()
    {
        Destroy(gameObject);
    }
}
