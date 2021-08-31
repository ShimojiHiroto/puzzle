using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    //鳥のプレハブ格納配置
    public GameObject[] birdPrefabs;

    //連鎖判定用の鳥の距離
    [SerializeField]public float birdDistance = 1.4f;

    //最低連鎖数
    const int MinChain = 3;

    //クリックされた鳥を格納
    private GameObject firstBird;
    private GameObject lastBird;
    private string currentName;
    List<GameObject> removabirdList = new List<GameObject>();
    public GameObject lineObj;
    List<GameObject> lineBirdList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        TouchManager.Began += (info) =>
        {
            //クリック地点でヒットしているオブジェクトを取得
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(info.screenPoint),
                Vector2.zero);
            if (hit.collider != null)
            {
                GameObject hitObj = hit.collider.gameObject;
                //ヒットしたオブジェクトのtagがbirdだったら初期化し、hitobjを登録
                if (hitObj.tag == "bird")
                {
                    firstBird = hitObj;
                    lastBird = hitObj;
                    currentName = hitObj.name;
                    removabirdList = new List<GameObject>();
                    lineBirdList = new List<GameObject>();
                    PushToBirdList(hitObj);
                }
            }
        };
        TouchManager.Moved += (info) =>
        {
            //クリック地点でヒットしているオブジェクトを取得
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(info.screenPoint),
                Vector2.zero);
            if (hit.collider != null)
            {
                GameObject hitObj = hit.collider.gameObject;
                //ヒットオブジェクトのtagがbird尚且つ、名前が一緒
                //死後にhitしたオブジェクトと違う、リストには入ってない
                if (hitObj.tag == "bird" && hitObj.name == currentName
                  && lastBird != hitObj && 0 > removabirdList.IndexOf(hitObj))
                {
                    float distance = Vector2.Distance(hitObj.transform.position, lastBird.transform.position);
                    if (distance > birdDistance)
                    {
                        return;
                    }
                    PushToLineList(hitObj, lastBird);
                    lastBird = hitObj;
                    PushToBirdList(hitObj);
                }
            }
        };
        TouchManager.Ended += (info) =>
        {
            int count = removabirdList.Count;

            if (count >= MinChain)
            {
                //リストに格納されている鳥を削除
                foreach (GameObject obj in removabirdList)
                {
                    FindObjectOfType<Score>().AddPoint(10);
                    Destroy(obj, 0.01f);
                }
                StartCoroutine(DropBird(count));
            }


            foreach (GameObject obj in removabirdList)
            {
                ChangeColor(obj, 1.0f);
            }
            foreach (GameObject obj in lineBirdList)
            {
                Destroy(obj);
            }
            removabirdList = new List<GameObject>();
            lineBirdList = new List<GameObject>();
            firstBird = null;
            lastBird = null;
        };
        StartCoroutine(DropBird(70));
    }


    private void PushToBirdList(GameObject obj)
    {
        removabirdList.Add(obj);
        ChangeColor(obj, 0.5f);
    }

    private void PushToLineList(GameObject lastObj, GameObject hitObj)
    {
        GameObject line = (GameObject)Instantiate(lineObj);
        LineRenderer renderer = line.GetComponent<LineRenderer>();
        //線の太さ
        renderer.startWidth = 0.1f;
        renderer.endWidth = 0.1f;
        //頂点の数
        renderer.positionCount = 2;
        //頂点を設定
        renderer.SetPosition(0, new Vector3(lastObj.transform.position.x,
            lastObj.transform.position.y, -1.0f));
        renderer.SetPosition(1, new Vector3(hitObj.transform.position.x,
            hitObj.transform.position.y, -1.0f));
        lineBirdList.Add(line);
    }




    private void ChangeColor(GameObject obj, float transparency)
    {
        SpriteRenderer birdTexture = obj.GetComponent<SpriteRenderer>();
        birdTexture.color = new Color(birdTexture.color.r, birdTexture.color.g, birdTexture.color.b, transparency);
    }

    // Update is called once per frame

    IEnumerator DropBird(int count)
    {
        for (int i = 0; i < count; i++)
        {
            //ランダムで出現位置を作成
            Vector2 pos = new Vector2(Random.Range(-3f, 3f), 15f);
            //出現する鳥のIDを作成
            int id = Random.Range(0, birdPrefabs.Length);

            //鳥を発生させる
            GameObject bird = (GameObject)Instantiate(birdPrefabs[id],
                pos,
                Quaternion.AngleAxis(Random.Range(-40, 40), Vector3.forward));
            //作成した鳥の名前をidを使ってつけなおす
            bird.name = "Bird" + id;
            yield return new WaitForSeconds(0.05f);


        }
    }
}