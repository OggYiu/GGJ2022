using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveDirection = Vector2.zero;
        const KeyCode moveUpKey = KeyCode.W;
        const KeyCode moveDownKey = KeyCode.S;
        const KeyCode moveLeftKey = KeyCode.A;
        const KeyCode moveRightKey = KeyCode.D;
        KeyCode[] moveKeys = { moveUpKey, moveDownKey, moveLeftKey, moveRightKey };
        foreach (var key in moveKeys)
        {
            if (Input.GetKey(key))
            {
                switch (key)
                {
                    case moveUpKey:
                        moveDirection.y = 1f;
                        break;
                    case moveDownKey:
                        moveDirection.y = -1f;
                        break;
                    case moveLeftKey:
                        moveDirection.x = -1f;
                        break;
                    case moveRightKey:
                        moveDirection.x = 1f;
                        break;
                    default:
                        break;
                }
            }
        }

        if (!Helper.IsZero(moveDirection))
        {
            moveDirection *= Time.deltaTime * speed;
            var currentPos = this.transform.position;
            var targetPosition = currentPos + new Vector3(moveDirection.x, moveDirection.y, 0f);
            targetPosition.z = Helper.BudColliderZ;

            //if (GameMgr.instance.CanMoveTo(targetPosition))
            {
                this.transform.position = targetPosition;
            }
        }
    }
}
