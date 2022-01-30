using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bud : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;

    [SerializeField]
    private Branch branch;

    [SerializeField]
    private AudioSource[] audioSpawn;

    private static int lastCreatedIndex = -1;

    private SpriteRenderer spriteRenderer => GetComponent<SpriteRenderer>();

    public int RandomSpriteIndex => Random.Range(0, sprites.Length);

    public void Init(Branch parentBranch, int spriteIndex)
    {
        if(spriteRenderer)
        {
            spriteRenderer.sprite = sprites[spriteIndex];
        }
        lastCreatedIndex = spriteIndex  ;
        branch = parentBranch;
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
        this.transform.DORotate(endRotate, duration, RotateMode.Fast).SetLoops(-1, LoopType.Yoyo);
        if(spriteRenderer)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
            DOTweenModuleSprite.DOColor(spriteRenderer, new Color(1f, 1f, 1f, 1f), 1f);
        }

        if(audioSpawn.Length != 0)
        {
            bool play = Random.Range(0, 2) == 1 ? true : false;
            if(play)
            {
                audioSpawn[Random.Range(0, audioSpawn.Length)].Play();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log($"[Bud::OnTriggerEnter2D], collision: {collision}");
        Player player = collision.GetComponent<Player>();
        if(player)
        {
            //Debug.Log($"[Bud::OnTriggerEnter2D], player.CurrentBud");
            player.CurrentBud = this;
        }
    }

    public bool CanGrowBranch()
    {
        return branch.CanGrowBranch();
    }

    public void GrowBranch()
    {
        branch.Generate();
    }
}
