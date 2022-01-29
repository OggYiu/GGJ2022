using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class Bud : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;

    public void Init()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
    }

    // Start is called before the first frame update
    void Start()
    {
        float duration = Random.Range(1f, 2f);
        float rangeDeg = Random.Range(5f, 35f);
        var startRotate = this.transform.rotation.eulerAngles;
        startRotate.z += rangeDeg / 2f;
        var endRotate = startRotate;
        startRotate.z -= rangeDeg;
        this.transform.DOLocalRotate(endRotate, duration, RotateMode.Fast).SetLoops(-1, LoopType.Yoyo);
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        DOTweenModuleSprite.DOColor(GetComponent<SpriteRenderer>(), new Color(1f, 1f, 1f, 1f), 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
