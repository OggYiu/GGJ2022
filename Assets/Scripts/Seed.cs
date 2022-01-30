using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Seed : MonoBehaviour
{
    private SpriteRenderer spriteRenderer => GetComponent<SpriteRenderer>();

    // Start is called before the first frame update
    void Start()
    {
        if (spriteRenderer)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
            DOTweenModuleSprite.DOColor(spriteRenderer, new Color(1f, 1f, 1f, 1f), 1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();
        if(player)
        {
            GameMgr.Instance.OnSeedTouched(this, player);
        }
    }
}
