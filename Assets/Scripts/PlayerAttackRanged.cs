using UnityEngine;
using Assets.Globals;
using UnityEngine.AdaptivePerformance;
using System.Collections;

public class PlayerAttackRanged : MonoBehaviour
{
    public int damage = 10;
    public float speed = 20;
    public float maxDistance = 10;
    public bool doesStun = true;
    private Vector3 AttackVel;
    private Transform AttackPos;
    private float deltaTime;
    private Vector2 startPos;
    private GameObject currentTarget;
    private Functions funcs = new();
    private FunctionsInstance funcInst = new();
    private Vector3 PrevPos;
    private Vector2 AttackDir;
    private Vector2 PrevFrameDir;
    private EnemyController enemy;
    void Awake()
    {
        AttackPos = GetComponent<Transform>();

        PrevPos = AttackPos.position;
        AttackPos.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime = Time.deltaTime;
        UpdatePosition();
    }
    void UpdatePosition()
    {

        AttackPos.position += deltaTime * AttackVel;
        AttackPos.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(-AttackVel.x, AttackVel.y)));

        transform.SetPositionAndRotation(AttackPos.position, AttackPos.rotation);
        
        enemy = currentTarget.GetComponent<EnemyController>();

        if (funcInst.DotProduct(AttackDir, PrevFrameDir) < 0)
        {
            enemy.OnHit(damage, doesStun);
            funcInst.Kill(transform.gameObject);
        }

        if (Vector3.Distance(AttackPos.position, startPos) > maxDistance)
        {
            funcInst.Kill(transform.gameObject);
        }

        if (Vector3.Distance(AttackPos.position, currentTarget.transform.position) < 0.5f)
        {
            enemy.OnHit(damage, doesStun);
            funcInst.Kill(transform.gameObject);
        }

        PrevPos = AttackPos.position;
        PrevFrameDir = AttackDir;
        AttackDir = (currentTarget.transform.position - AttackPos.position).normalized;
    }

    public void OnAttack(GameObject enemy, Vector3 start)
    {
        // if targeted enemy is not active, return
        if (!enemy.activeSelf)
        {
            funcInst.Kill(transform.gameObject);
            return;
        }

        // set targeted enemy
        currentTarget = enemy;

        // find distance to targeted enemy
        float distance = Vector3.Distance(enemy.transform.position, start);
        if(distance > maxDistance)
        {
            funcInst.Kill(transform.gameObject);
            return;
        }

        //get general enemy data
        VarTypes.Data data = enemy.GetComponent<EnemyController>().GetData();

        /* distance / speed is equivilant to time
        *  time * velocity is equivilant to distance
        *  therefore, (distance / speed) * velocity is the updated relative transform
        *  and we add the distance to the transform to get the new transform
        *  by subtracting the start transform of the player's attack, we get the vector
        *  pointing at where the enemy will be from where the attack started, so the
        *  attack auto-aims at where the enemy will be, due to lack of fine control
        *  over direction of projectile from the player
        */
        Vector3 direction = ((distance/speed) * data.velocity) + data.position - start;
        AttackDir = direction.normalized;
        PrevFrameDir = AttackDir;
        Debug.Log($"{data.transform.position} vs {data.position}");
        Debug.Log($"Speed: {speed}");
        Debug.Log($"{distance / speed * data.velocity}");

        // set Velocity so that attack, y'know, moves
        AttackVel = direction.normalized * speed;
        // set position to the player's position
        startPos = start;

        // update the VarTypes.Data.position clusterfuck (why is VarTypes.Data.position a Transform?????)
        //                                               fixed!!^ VarTypes.Data.position is now a Vector3
        //                                               also this never used VarTypes.Data.position lol
        AttackPos.position = startPos;
        AttackPos.rotation = funcInst.ToQuaternion(AttackVel.normalized);
        Debug.Log($"{Time.time}: {AttackVel}");

        // activate attack so it can move and be seen
        transform.gameObject.SetActive(true);

        // call UpdatePosition to update transform
        UpdatePosition();
    }
}
