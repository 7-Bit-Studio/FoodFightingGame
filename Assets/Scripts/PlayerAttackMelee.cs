using UnityEngine;
using Assets.Globals;

public class PlayerAttackMelee : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float speed = 20;
    [SerializeField] public float range = 5;
    private float deltaTime;
    private Vector2 startPos;
    private GameObject currentTarget;
    void Awake()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
    }
    void UpdatePosition()
    {
    }


    public void OnAttack(GameObject enemy, Vector3 start)
    {
        // if targeted enemy is not active, return
        if (!enemy.activeSelf) return;

        // set targeted enemy
        currentTarget = enemy;

        // find distance to targeted enemy
        float distance = Vector3.Distance(enemy.transform.position, start);

        //get general enemy data
        VarTypes.Data data = enemy.GetComponent<EnemyController>().GetData();

        if(distance > range)
        {
            Debug.Log($"Enemy is out of range. Distance: {distance}, Range: {range}");
            return;
        }

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
        AttackPos.rotation = Functions.ToQuaternion(AttackVel.normalized);
        Debug.Log($"{Time.time}: {AttackVel}");

        // activate attack so it can move and be seen
        transform.gameObject.SetActive(true);

        // call UpdatePosition to update transform
        UpdatePosition();
    }
}
