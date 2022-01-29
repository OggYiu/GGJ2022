using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Branch : MonoBehaviour
{
    [SerializeField]
    private GameObject budPrefab;

    [SerializeField]
    private GameObject lightSeedPrefab;

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
    private List<GameObject> buds = new List<GameObject>();

    private int RandomBudCount => Random.Range(minBudCount, maxBudCount);

    private bool branchGenerated = false;

    // Start is called before the first frame update
    void Start()
    {
        //int counter = 1;
        //foreach(var bud in buds)
        //{
        //    bud.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        //    DOTweenModuleSprite.DOColor(bud.GetComponent<SpriteRenderer>(), new Color(1f, 1f, 1f, 1f), counter * 1f);
        //    //bud.GetComponent<SpriteRenderer>().color.DO

        //    ++counter;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        //Vector2 mousePos = Input.mousePosition;
        //var worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
        //if (Input.GetMouseButtonDown(0))
        //{
        //    int count = Random.Range(minBudCount, maxBudCount);
        //    float angle = Mathf.Deg2Rad * Random.Range(minBranchAngle, maxBranchAngle);
        //    Generate(worldPoint, angle, count);
        //}
    }

    [ContextMenu("Generate")]
    void Generate()
    {
        if(branchGenerated)
        {
            Debug.LogWarning($"Branch already generated");
            return;
        }

        branchGenerated = false;

        if (buds.Count <= 1)
        {
            Debug.LogError($"Too List Bugs: {buds.Count}");
            return;
        }

        {
            int fromIndex = Random.Range(1, buds.Count - 1);
            var targetBud = buds[fromIndex];
            float angle = GetLeftRandomBudAngle(targetBud.transform.rotation.eulerAngles.z);
            CreateBranch(fromIndex, angle, RandomBudCount);
        }

        {
            int fromIndex = Random.Range(1, buds.Count - 1);
            var targetBud = buds[fromIndex];
            float angle = GetRightRandomBudAngle(targetBud.transform.rotation.eulerAngles.z);
            CreateBranch(fromIndex, angle, RandomBudCount);
        }
    }

    [ContextMenu("GenerateUpwardBranch")]
    void GenerateUpwardBranch()
    {
        StartCoroutine(Generate(Vector3.zero, 90f, 4, false));
    }

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

        for (int i = 0; i < count; ++i)
        {
            if (skipFirst && i == 0)
            {
            }
            else
            {
                GameObject bud = Instantiate(budPrefab);
                bud.GetComponent<Bud>().Init();
                bud.transform.parent = budParent;
                bud.transform.localPosition = nextPos;
                bud.transform.rotation = Quaternion.Euler(0f, 0f, angle);
                bud.GetComponent<SpriteRenderer>().sortingOrder = count * -1 + i;
                buds.Add(bud);

                yield return new WaitForSeconds(0.5f);
            }
            Vector3 addPos = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f) * budSize;
            nextPos += addPos;
        }

        bool generateSpeed = Random.Range(0f, 1f) <= generateSpeedChance;
        if(generateSpeed)
        {
            GameObject bud = Instantiate(lightSeedPrefab);
            bud.transform.parent = budParent;
            bud.transform.localPosition = nextPos;
            bud.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            buds.Add(bud);
        }
    }

    public void CreateBranch(int fromIndex, float angle, int count)
    {
        if(fromIndex < 0 || fromIndex >= buds.Count)
        {
            Debug.LogError($"Invalid fromIndex: {fromIndex}");
            return;
        }

        var targetBud = buds[fromIndex];
        Vector3 fromPos = targetBud.transform.position;

        var branch = Instantiate<Branch>(GameMgr.Instance.GetBranchPrefab());
        branch.transform.position = targetBud.transform.position;
        StartCoroutine(branch.Generate(Vector3.zero, angle, count, true));
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
}