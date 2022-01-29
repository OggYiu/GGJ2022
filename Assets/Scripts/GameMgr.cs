using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    private static GameMgr instance;

    [SerializeField]
    private Branch branchPrefab;

    [SerializeField]
    private List<Branch> branches = new List<Branch>();

    [SerializeField]
    private ParticleSystem seedTouchEffect;

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

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        var worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
        bool result = GameMgr.Instance.CanMoveTo(worldPoint);
        //Debug.Log($"result: {result}\n");
    }

    public bool CanMoveTo(Vector3 position)
    {
        foreach(var branch in branches)
        {
            if(branch.IsColliderContainsPoint(position))
            {
                return true;
            }
        }

        return false;
    }

    public Branch GetBranchPrefab()
    {
        return branchPrefab;
    }

    public void OnSeedTouched(Seed seed)
    {
        Destroy(seed.gameObject);

        var effect = Instantiate(seedTouchEffect);
        effect.transform.position = seed.transform.position;
        Destroy(effect.gameObject, 2f);
    }
}
