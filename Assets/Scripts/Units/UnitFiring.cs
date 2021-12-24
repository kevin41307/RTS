using UnityEngine;
using Mirror;
public class UnitFiring : NetworkBehaviour 
{
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private GameObject projectilePrefab = null;
    [SerializeField] private Transform projectileSpawnPoint = null;
    [SerializeField] private float fireRange = 5f;
    public float GetFireRange() => fireRange;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float rotationSpeed = 20f;

    private Targetable target = null;
    private float lastFireTime;
    [ServerCallback]
    private void Update() 
    {
        //Update target info
        target = targeter.GetTarget();
        if (target == null) target = targeter.GetAutoTarget();
        if (target == null) return;
        //if isMoving stop firing
        if (unitMovement.isMoving) return;
        if (!CanFireAt(target)) return;
        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if(Time.time > (1 / fireRate) + lastFireTime)
        {
            Quaternion projectileRotation = Quaternion.LookRotation(
                target.GetAimPoint().position - projectileSpawnPoint.position);

            GameObject projectileInstance = Instantiate(
                projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
            NetworkServer.Spawn(projectileInstance, connectionToClient);
            lastFireTime = Time.time;
        }
    }
    [Server]
    private bool CanFireAt(Targetable target)
    {
        return (target.transform.position - transform.position).sqrMagnitude <= fireRange * fireRange;
    }

}