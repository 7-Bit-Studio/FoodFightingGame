using Assets.Globals;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float damageArea = 0.5f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float speed = 20;
    private VarTypes.Data data;
    private Vector3 AttackVel;
    private Transform AttackPos;
    private float deltaTime;
    private Vector2 startPos;
    public VarTypes.Data GetData() => data;
    private GameObject currentTarget;
    void Start()
    {
        AttackPos = GetComponent<Transform>();
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

        if (Mathf.Abs(AttackPos.position.x - startPos.x) > 10 || Mathf.Abs(AttackPos.position.y - startPos.y) > 10)
        {
            transform.gameObject.SetActive(false);
            AttackVel = Vector3.zero;
            AttackPos.position = startPos;
        }

        if (Vector3.Distance(AttackPos.position, currentTarget.transform.position) < 0.5f)
        {
            currentTarget.GetComponent<EnemyController>().OnHit(damage);
            transform.gameObject.SetActive(false);
        }
    }


    public void OnAttack(GameObject enemy, Vector3 start)
    {
        // if targeted enemy is not active, return
        if (AttackPos.gameObject.activeSelf) return;

        // set targeted enemy
        currentTarget = enemy;
        
        // find distance to targeted enemy
        float distance = Vector3.Distance(enemy.transform.position, start);

        //get general enemy data
        VarTypes.Data data = enemy.GetComponent<EnemyController>().GetData();

        /* distance / speed is equivilant to time
        *  time * velocity is equivilant to distance
        *  therefore, (distance / speed) * velocity is the updated relative position
        *  and we add the distance to the position to get the new position
        *  by subtracting the start position of the player's attack, we get the vector pointing at where the enemy will be from
        *  where the attack started, so the attack auto-aims at where the enemy will be, due to lack of fine control over direction
        *  of projectile from the player
        */
        Vector3 direction = ((distance/speed) * data.velocity) + data.position.position - start;
        Debug.Log($"Speed: {speed}");
        Debug.Log($"{distance / speed * data.velocity}");

        // set Velocity so that attack, y'know, moves
        AttackVel = direction.normalized * speed;
        // set position to the starting position
        startPos = start;

        // update the VarTypes.Data.position clusterfuck (why is VarTypes.Data.position a Transform?????)
        AttackPos.position = startPos;
        AttackPos.rotation = Functions.ToQuaternion(AttackVel.normalized);
        Debug.Log($"{Time.time}: {AttackVel}");

        // activate attack so it can move and be seen
        transform.gameObject.SetActive(true);

        // call UpdatePosition to update position
        UpdatePosition();
    }
}
