using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMgr : MonoBehaviour
{
    private static GameMgr instance;

    [SerializeField]
    private Branch upperBranchPrefab;

    [SerializeField]
    private Branch lowerBranchPrefab;

    [SerializeField]
    private GameObject lightSpotPrefab;

    [SerializeField]
    private List<Branch> branches = new List<Branch>();

    [SerializeField]
    private ParticleSystem seedTouchEffect;

    [SerializeField]
    private float totalMapHeight;

    [SerializeField]
    private float lightSpotScaleRange;

    [SerializeField]
    private GameObject[] raceDots;

    [SerializeField]
    private float totalMeterLength;

    [SerializeField]
    private Player[] players;

    private bool isEnded = false;

    [SerializeField]
    private GameObject endGameBackground;

    [SerializeField]
    private AudioSource audioSeedGot;

    [SerializeField]
    private List<Branch> allBranches = new List<Branch>();

    [SerializeField]
    private AudioSource audioGrow;

    public Player FirstPlayer => players[0];

    public Player SecondPlayer => players[1];

    public Branch UpperBranch => branches[0];

    public Branch LowerBranch => branches[1];

    public bool IsEnded => isEnded;

    public float GetTotalMapHeight()
    {
        return totalMapHeight;
    }

    public static GameMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameMgr>();

                if (instance == null)
                {
                    GameObject go = new GameObject();
                    go.name = "GameMgr";
                    instance = go.AddComponent<GameMgr>();
                    DontDestroyOnLoad(go);
                }
            }

            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        allBranches.Add(LowerBranch);
    }

    // Update is called once per frame
    void Update()
    {
        //Vector2 mousePos = Input.mousePosition;
        //var worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
        //bool result = GameMgr.Instance.CanMoveTo(worldPoint);
        //Debug.Log($"result: {result}\n");

        UpdateRaceDots();
    }

    void UpdateRaceDots()
    {
        for (int i = 0; i < players.Length; ++i)
        {
            float percentage = players[i].GetDistancePercentage();
            percentage = Mathf.Min(percentage, 1.0f);
            percentage = Mathf.Max(percentage, 0.0f);
            //Debug.Log($"player[{i}] percentage: {percentage}");

            float circleRadius = 55f;
            var targetDot = raceDots[i];
            float rad = (percentage * 180f + 90f) * Mathf.Deg2Rad;
            float x = Mathf.Cos(rad) * circleRadius;
            float y = Mathf.Sin(rad) * circleRadius;
            if(i == 0)
            {
                x = Mathf.Abs(x);
            }
            targetDot.transform.localPosition = new Vector3(x, y, 0);
        }
    }

    //public bool CanMoveTo(Vector3 position)
    //{
    //    foreach(var branch in branches)
    //    {
    //        if(branch.IsColliderContainsPoint(position))
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    public Branch GetUpperBranchPrefab()
    {
        return upperBranchPrefab;
    }

    public Branch GetLowerBranchPrefab()
    {
        return lowerBranchPrefab;
    }

    public void OnSeedTouched(Seed seed, Player player)
    {
        audioSeedGot.Play();

        Destroy(seed.gameObject);

        var effect = Instantiate(seedTouchEffect);
        effect.transform.position = seed.transform.position;
        Destroy(effect.gameObject, 2f);

        GenerateLightSpot(seed.transform.position);

        player.Recover(100);
    }

    public bool GrowBranchOnPlayer1()
    {
        //Debug.Log($"GrowBranchOnPlayer1, FirstPlayer: {FirstPlayer}");
        //Debug.Log($"GrowBranchOnPlayer1, FirstPlayer.CurrentBud: {FirstPlayer?.CurrentBud}");
        if (FirstPlayer.CurrentBud == null)
        {
            Debug.LogWarning($"Branch couldn't grow because player 1 is not on any bud");
            return false;
        }
        if(!FirstPlayer.CurrentBud.CanGrowBranch())
        {
            Debug.LogWarning($"Branch couldn't grow because it just cannot");
            return false;
        }

        FirstPlayer.ShowBubble();

        audioGrow.Play();

        FirstPlayer.CurrentBud.GrowBranch();

        var lowestBranch01 = FindLowestAvailableBranch();
        var lowestBranch02 = FindLowestAvailableBranch(lowestBranch01);

        if (lowestBranch01)
        {
            allBranches.AddRange(lowestBranch01.Generate());
        }

        if (lowestBranch02)
        {
            allBranches.AddRange(lowestBranch02.Generate());
        }

        return true;
    }

    private Branch FindLowestAvailableBranch(Branch except = null)
    {
        float lowest = float.MaxValue;
        Branch lowestBranch = null;
        foreach(var branch in allBranches)
        {
            if(except == branch)
            {
                continue;
            }

            if(branch.CanGrowBranch())
            {
                GameObject targetBud = branch.GetLowestBud();
                if (lowest > targetBud.transform.position.y)
                {
                    lowest = targetBud.transform.position.y;
                    lowestBranch = branch;
                }
            }
        }
        return lowestBranch;
    }

    [ContextMenu("GenerateLightSpot")]
    public void GenerateLightSpot(Vector3 fromPos)
    {
        var pos = fromPos;
        pos.y = totalMapHeight - fromPos.y;
        var lightSpot = Instantiate(lightSpotPrefab);
        lightSpot.transform.position = pos;
        lightSpot.transform.localScale = Vector3.one * Random.Range(1f, lightSpotScaleRange);
    }

    public IEnumerator OnEnd()
    {
        if(isEnded)
        {
            yield break;
        }

        isEnded = true;

        yield return ShowGameEnd();

        SceneManager.LoadScene("Tree");
    }


    IEnumerator ShowGameEnd()
    {
        endGameBackground.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
    }
}
