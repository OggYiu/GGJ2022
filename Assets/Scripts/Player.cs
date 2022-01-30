using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private KeyCode moveUpKey;

    [SerializeField]
    private KeyCode moveDownKey;

    [SerializeField]
    private KeyCode moveLeftKey;

    [SerializeField]
    private KeyCode moveRightKey;

    [SerializeField]
    private KeyCode growBranchKey;

    [SerializeField]
    private int maxLive = 8;

    [SerializeField]
    private float live;

    [SerializeField]
    private float liveLoseSpeed = 0.5f;

    [SerializeField]
    private Bud currentBud;

    [SerializeField]
    private TextMeshProUGUI textLive;

    [SerializeField]
    private Image heartSpriteRenderer;

    [SerializeField]
    private GameObject[] playerAnims;

    [SerializeField]
    private GameObject lastPlayerAnim;

    [SerializeField]
    private AudioSource audioWalk;

    private bool isDied = false;

    [SerializeField]
    private ParticleSystem diedEffect;

    [SerializeField]
    private GameObject bubble;

    private SpriteRenderer spriteRenderer => GetComponent<SpriteRenderer>();

    public Bud CurrentBud
    {
        get
        {
            return currentBud;
        }
        set
        {
            currentBud = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        live = maxLive;
        textLive.text = live.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(isDied)
        {
            return;
        }

        Vector2 moveDirection = Vector2.zero;
        if (Input.GetKey(moveUpKey))
        {
            moveDirection.y = 1f;
        }
        if (Input.GetKey(moveDownKey))
        {
            moveDirection.y = -1f;
        }
        if (Input.GetKey(moveLeftKey))
        {
            moveDirection.x = -1f;
        }
        if (Input.GetKey(moveRightKey))
        {
            moveDirection.x = 1f;
        }
        if (growBranchKey != KeyCode.None && Input.GetKeyDown(growBranchKey))
        {
            GameMgr.Instance.GrowBranchOnPlayer1();
        }

        if (!Helper.IsZero(moveDirection))
        {
            if (audioWalk)
            {
                audioWalk.gameObject.SetActive(true);
            }

            if(!Helper.IsZero(moveDirection.x))
            {
                foreach (var anim in playerAnims)
                {
                    anim.transform.localScale = new Vector3(moveDirection.x, 1f, 1f);
                }
            }

            moveDirection *= Time.deltaTime * speed;
            var currentPos = this.transform.position;
            var targetPosition = currentPos + new Vector3(moveDirection.x, moveDirection.y, 0f);
            targetPosition.z = Helper.BudColliderZ;

            //if (GameMgr.instance.CanMoveTo(targetPosition))
            {
                this.transform.position = targetPosition;
            }

            if(playerAnims.Length > 0)
            {
                if (!Helper.IsZero(moveDirection.y))
                {
                    playerAnims[2].SetActive(true);
                    if(lastPlayerAnim != null && lastPlayerAnim != playerAnims[2])
                    {
                        lastPlayerAnim.SetActive(false);
                    }
                    lastPlayerAnim = playerAnims[2];
                }
                else if (!Helper.IsZero(moveDirection.x) && Helper.IsZero(moveDirection.y))
                {
                    playerAnims[1].SetActive(true);
                    if (lastPlayerAnim != null && lastPlayerAnim != playerAnims[1])
                    {
                        lastPlayerAnim.SetActive(false);
                    }
                    lastPlayerAnim = playerAnims[1];
                }
                else
                {
                    playerAnims[0].SetActive(true);
                    if (lastPlayerAnim != null && lastPlayerAnim != playerAnims[0])
                    {
                        lastPlayerAnim.SetActive(false);
                    }
                    lastPlayerAnim = playerAnims[0];
                }
            }
        }
        else
        {
            if(audioWalk)
            {
                audioWalk.gameObject.SetActive(false);
            }

            if (playerAnims.Length > 0)
            {
                playerAnims[0].SetActive(true);
                if (lastPlayerAnim != null && lastPlayerAnim != playerAnims[0])
                {
                    lastPlayerAnim.SetActive(false);
                }
                lastPlayerAnim = playerAnims[0];
            }
        }

        if (live > 0)
        {
            live -= Time.deltaTime * liveLoseSpeed;
            if(live <= 0)
            {
                live = 0;
                StartCoroutine(OnDied());
            }
            textLive.text = Mathf.Ceil(live).ToString();
        }
    }

    private IEnumerator OnDied()
    {
        isDied = true;

        var effect = Instantiate(diedEffect);
        effect.transform.position = this.transform.position;
        Destroy(effect.gameObject, 2.0f);

        foreach(var anim in playerAnims)
        {
            Destroy(anim);
        }
        playerAnims = new GameObject[] { };

        if(spriteRenderer)
        {
            spriteRenderer.enabled = false;
        }

        yield return new WaitForSeconds(5.0f);

        SceneManager.LoadScene("Tree");
    }

    public void Recover(int heartCount)
    {
        live += heartCount;
        if(live > maxLive)
        {
            live = maxLive;
        }
        textLive.text = Mathf.Ceil(live).ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player && !GameMgr.Instance.IsEnded)
        {
            StartCoroutine(GameMgr.Instance.OnEnd());
        }
    }

    public float GetDistancePercentage()
    {
        if (this == GameMgr.Instance.FirstPlayer)
        {
            return (transform.position.y / GameMgr.Instance.GetTotalMapHeight()) * 2f;
        }
        else
        {
            return ((GameMgr.Instance.GetTotalMapHeight() - transform.position.y) / GameMgr.Instance.GetTotalMapHeight()) * 2f;
        }
    }

    public void ShowBubble()
    {
        if(!bubble)
        {
            return;
        }

        IEnumerator _ShowBubble()
        {
            bubble.gameObject.SetActive(true);
            yield return new WaitForSeconds(2.0f);
            bubble.gameObject.SetActive(false);
        }

        StartCoroutine(_ShowBubble());
    }
}
