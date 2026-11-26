using Assets.Globals;
using JetBrains.Annotations;
using JetBrains.Rider.Unity.Editor;
using UnityEditor.Rendering;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerCollider : MonoBehaviour
{
    private CircleCollider2D playerCollider;
    private PlayerController player;
    [SerializeField] private LayerMask wallLayer;
    private float radius;
    private static readonly Vector2[] dirs = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
    private readonly RaycastHit2D[] currentHits = new RaycastHit2D[8];
    private readonly RaycastHit2D[] prevHits = new RaycastHit2D[8];
    private ContactFilter2D filter;
    private Vector2 prevPlayerPos;
    private CircleCollider2D previousCollider;

    void Start()
    {
        player = gameObject.GetComponent<PlayerController>();
        prevPlayerPos = transform.position;
        playerCollider = gameObject.GetComponent<CircleCollider2D>();
        previousCollider = playerCollider;
        radius = playerCollider.radius;
        filter = new ContactFilter2D();
        filter.SetLayerMask(wallLayer);
        filter.useTriggers = false;
    }

    void Update()
    {

        for (int d = 0; d < dirs.Length; d++)
        {
            int countCurrent = playerCollider.Raycast(dirs[d], filter, currentHits, radius);
            int countPrev = previousCollider.Raycast(dirs[d], filter, prevHits, radius);
            if (countCurrent == countPrev)
            {
                for (int i = 0; i < countCurrent; i++)
                {
                    player.playerPos -= (Vector3)dirs[d] * (radius - currentHits[i].distance);
                    if (d % 2 == 0)
                    {
                        player.playerVel = new Vector3(player.playerVel.x, 0);
                    }
                    if (d % 2 == 1)
                    {
                        player.playerVel = new Vector3(0, player.playerVel.y);
                    }
                }
            }
            else
            {
                player.playerPos = prevPlayerPos;
                for (int i = 0; i < countPrev; i++)
                {
                    player.playerPos -= (Vector3)dirs[d] * (radius - prevHits[i].distance);
                    if (d % 2 == 0)
                    {
                        player.playerVel = new Vector3(player.playerVel.x, 0);
                    }
                    if (d % 2 == 1)
                    {
                        player.playerVel = new Vector3(0, player.playerVel.y);
                    }
                }
            }
        }

        prevPlayerPos = player.playerPos;
        previousCollider = playerCollider;
    }
}
