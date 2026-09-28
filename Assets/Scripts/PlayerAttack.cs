using Assets.Globals;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private VarTypes.Data data;
    private Vector3 AttackVel;
    private Transform AttackPos;
    private float deltaTime;
    public VarTypes.Data GetData() => data;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AttackPos = GetComponent<Transform>();
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

        //if(AttackPos.position.x > )
    }


    public void OnAttack(Vector3 direction, Vector3 start, float speed)
    {
        AttackVel = direction * speed;
        AttackPos.position = start;
        AttackPos.rotation = Functions.ToQuaternion(AttackVel.normalized);
        Debug.Log($"{AttackVel}");
        UpdatePosition();
    }
}
