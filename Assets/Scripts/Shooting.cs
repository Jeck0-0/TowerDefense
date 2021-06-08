using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class Shooting : MonoBehaviour
{
    public enum TargetChoiceCriterion { First, Last, Closest, Farther, Strongest, Weakest, Random, PreferredDirection }

    [Header("Stats")]
    public GameObject projectile;
    public TargetChoiceCriterion criterion; //
    public float rotationSpeed;
    public float range;
    public float attackSpeed;
    public bool resetRotationWhenNoTarget;

    [Header("Others")]
    public Transform target;
    public Transform bulletSpawnAt;
    public Transform barrelPivot;
    public float degShootingAngleThreshold = 5;
    public event EventHandler<Projectile> ShotProjectile;

    float currentAngle {
        get {
            return FixDegreeAngle(barrelPivot.rotation.eulerAngles.z - 90);
        }
    }

    float FixDegreeAngle(float angle)
    {
        if (angle < 0)
            return angle + 360;
        return angle;
    }

    private void Start()
    {
        if (bulletSpawnAt == null)
            bulletSpawnAt = transform;
        StartCoroutine(ShootCoroutine());
    }

    private void Update()
    {
        if (target)
        {
            float distanceFromTarget = Vector2.Distance(transform.position, target.position);
            if (distanceFromTarget > range)
            {
                if (UpdateTarget())
                    RotateTowardsTarget();
            }
            else
            {
                RotateTowardsTarget();
                return;
            }
        }
        else if (UpdateTarget())
            RotateTowardsTarget();
        else if (resetRotationWhenNoTarget)
            RotateTowardsAngle(transform.rotation);
        
    } 

    void RotateTowardsTarget()
    {
        Vector3 vectorToTarget = target.position - transform.position;
        float angle = FixDegreeAngle(Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90);
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        RotateTowardsAngle(q);
    }
    void RotateTowardsAngle(Quaternion q)
    {
        //transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * speed);
        barrelPivot.rotation = Quaternion.RotateTowards(barrelPivot.rotation, q, rotationSpeed * Time.deltaTime);
    }

    bool CanShoot()
    {   
        if (!target)
            return false;
        Vector2 directionToTarget = (barrelPivot.position - target.position).normalized;
        float angleToTarget = FixDegreeAngle(Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg);
        float angleDiff = Mathf.Abs(Mathf.Abs(angleToTarget) - Mathf.Abs(currentAngle));
        return angleDiff <= degShootingAngleThreshold || angleDiff >= 360 - degShootingAngleThreshold;
    }

    bool selected = false;
    private void OnSelect()
    {
        selected = true;
        //MyEvents.sium.Invoke();
    }
    void OnDeselect()
    {
        selected = false;
    }
    private void OnDrawGizmos()
    {
        if (!selected) 
            return;
        if(target)
            Gizmos.DrawLine(transform.position, target.position);
        Gizmos.DrawRay(transform.position, barrelPivot.TransformDirection(Vector3.up) * range);
        Gizmos.DrawWireSphere(transform.position, range);
        
    }

    IEnumerator ShootCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / attackSpeed);
            yield return new WaitUntil(CanShoot);
            if (projectile)
            {
                GameObject go = Instantiate(projectile, bulletSpawnAt.position, Quaternion.Euler(0, 0, currentAngle + 180)); //////////////////////////////
                Projectile proj = go.GetComponent<Projectile>();
                proj.target = target;
                ShotProjectile?.Invoke(this, proj);
            }
        }
    }

    bool UpdateTarget()
    {
        Enemy nearestEnemy = null;
        float nearestDistance = range;
        foreach (Enemy e in Enemy.enemies)
        {
            float dist = Vector2.Distance(transform.position, e.transform.position);
            //Debug.Log(dist);
            if (dist < nearestDistance)
            {
                nearestEnemy = e;
                nearestDistance = dist;
            }
        }
        target = nearestEnemy?.transform;
        return target;
    }

}
