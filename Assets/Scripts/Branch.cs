using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Branch : MonoBehaviour
{
    [SerializeField]
    private GameObject budPrefab;

    [SerializeField]
    private GameObject[] lightSeedPrefabs;

    [SerializeField]
    private float baseAngleRange;

    [SerializeField]
    private float randAngleRange;

    [SerializeField]
    private int minBudCount;

    [SerializeField]
    private int maxBudCount;

    [SerializeField]
    private float budSize;

    [SerializeField]
    private float generateSpeedChance;

    [SerializeField]
    private bool isUpper = true;

    [SerializeField]
    private List<GameObject> buds = new List<GameObject>();

    private int RandomBudCount => Random.Range(minBudCount, maxBudCount);

    private bool canGrowBranch = true;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    [ContextMenu("Generate")]
    public List<Branch> Generate()
    {
        List<Branch> branches = new List<Branch>();

        if (!canGrowBranch)
        {
            Debug.LogWarning($"Branch already generated");
            return branches;
        }

        canGrowBranch = false;

        if (buds.Count <= 1)
        {
            Debug.LogError($"Too List Bugs: {buds.Count}");
            return branches;
        }

        {
            int fromIndex = Random.Range(1, buds.Count - 1);
            var targetBud = buds[fromIndex];
            float angle = GetLeftRandomBudAngle(targetBud.transform.rotation.eulerAngles.z);
            var newBranch = CreateBranch(fromIndex, angle, RandomBudCount, isUpper);
            if(newBranch)
            {
                branches.Add(newBranch);
            }
        }

        {
            int fromIndex = Random.Range(1, buds.Count - 1);
            var targetBud = buds[fromIndex];
            float angle = GetRightRandomBudAngle(targetBud.transform.rotation.eulerAngles.z);
            var newBranch = CreateBranch(fromIndex, angle, RandomBudCount, isUpper);
            if (newBranch)
            {
                branches.Add(newBranch);
            }
        }
        return branches;
    }

    [ContextMenu("GenerateUpwardBranch")]
    void GenerateUpwardBranch()
    {
        StartCoroutine(Generate(Vector3.zero, 90f, 4, false));
        //Generate(Vector3.zero, 90f, 4, false);
    }

    //void Generate(Vector3 position, float angle, int count, bool skipFirst)
    IEnumerator Generate(Vector3 position, float angle, int count, bool skipFirst)
    {
        //Debug.Log($"[Branch::Generate], position: {position}, angle: {angle}, count: {count}");
        buds.Clear();

        Vector3 nextPos = position;

        Transform budParent = this.transform.Find("Buds");
        if (!budParent)
        {
            budParent = (new GameObject()).transform;
            budParent.name = "Buds";
            budParent.parent = this.transform;
            budParent.localPosition = Vector3.zero;
        }

        int lastSpriteIndex = -1;

        for (int i = 0; i < count; ++i)
        {
            if (skipFirst && i == 0)
            {
            }
            else
            {
                var bud = Instantiate(budPrefab).GetComponent<Bud>();
                int spriteIndex = -1;
                do
                {
                    spriteIndex = bud.RandomSpriteIndex;
                } while (lastSpriteIndex == spriteIndex);
                lastSpriteIndex = spriteIndex;
                bud.Init(this, spriteIndex);
                bud.transform.parent = budParent;
                bud.transform.localPosition = nextPos;
                bud.transform.rotation = Quaternion.Euler(0f, 0f, angle);
                bud.GetComponent<SpriteRenderer>().sortingOrder = count * -1 + i;
                buds.Add(bud.gameObject);

                yield return new WaitForSeconds(Random.Range(0.3f, 0.6f));
            }
            Vector3 addPos = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f) * budSize;
            nextPos += addPos;
        }

        bool generateSpeed = Random.Range(0f, 1f) <= generateSpeedChance;
        if(generateSpeed)
        {
            GameObject bud = Instantiate(lightSeedPrefabs[Random.Range(0, lightSeedPrefabs.Length)]);
            bud.transform.parent = budParent;
            bud.transform.localPosition = nextPos;
            bud.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            buds.Add(bud);
        }
    }

    public Branch CreateBranch(int fromIndex, float angle, int count, bool isUpper)
    {
        if(fromIndex < 0 || fromIndex >= buds.Count)
        {
            Debug.LogError($"Invalid fromIndex: {fromIndex}");
            return null;
        }

        var targetBud = buds[fromIndex];
        Vector3 fromPos = targetBud.transform.position;

        var branch = Instantiate<Branch>(isUpper? GameMgr.Instance.GetUpperBranchPrefab() : GameMgr.Instance.GetLowerBranchPrefab());
        branch.transform.position = targetBud.transform.position;
        StartCoroutine(branch.Generate(Vector3.zero, angle, count, true));
        //branch.Generate(Vector3.zero, angle, count, true);
        return branch;
    }

    private float GetRandomBudAngle(float angle)
    {
        Debug.Log($"[Branch::GetRandomBudAngle], angle: {angle}");
        while(angle > 360f)
        {
            angle -= 360f;
        }
        while(angle < 0)
        {
            angle += 360f;
        }
        if(Helper.Equals(angle, 90f))
        {
            Debug.Log($"[Branch::GetRandomBudAngle], case 1");
            return Random.Range(0, 2) == 1 ? GetLeftRandomBudAngle(angle) : GetRightRandomBudAngle(angle);
        }
        if(angle < 90f || angle >= 270f)
        {
            Debug.Log($"[Branch::GetRandomBudAngle], case 2");
            return GetLeftRandomBudAngle(angle);
        }
        else
        {
            Debug.Log($"[Branch::GetRandomBudAngle], case 3");
            return GetRightRandomBudAngle(angle);
        }
    }

    private float GetLeftRandomBudAngle(float angle)
    {
        return angle + (baseAngleRange + randAngleRange);
    }

    private float GetRightRandomBudAngle(float angle)
    {
        return angle - (baseAngleRange + randAngleRange);
    }

    public bool IsColliderContainsPoint(Vector3 position)
    {
        foreach (var bud in buds)
        {
            var collider = bud.GetComponent<Collider>();
            if (collider.bounds.Contains(position))
            {
                return true;
            }
        }
        return false;
    }

    public bool CanGrowBranch()
    {
        return canGrowBranch;
    }

    public GameObject GetLowestBud()
    {
        GameObject lowestBud = null;
        float lowestY = float.MaxValue;
        foreach (var bud in buds)
        {
            if(lowestY > bud.transform.position.y)
            {
                lowestY = bud.transform.position.y;
                lowestBud = bud;
            }
        }
        return lowestBud;
    }
}