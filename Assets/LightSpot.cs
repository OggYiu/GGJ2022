using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LightSpot : MonoBehaviour
{
    [SerializeField]
    private float rotSpeed = 1f;

    private int rotDirection = 1;

    // Start is called before the first frame update
    void Start()
    {
        rotDirection = Random.Range(0, 2) == 0 ? 1 : -1;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(new Vector3(0f, 0f, Time.deltaTime * rotSpeed * (float)rotDirection));
    }
}
