using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    //���̃v���n�u�i�[�z�u
    public GameObject[] birdPrefabs;

    //�A������p�̒��̋���
    [SerializeField]public float birdDistance = 1.4f;

    //�Œ�A����
    const int MinChain = 3;

    //�N���b�N���ꂽ�����i�[
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
            //�N���b�N�n�_�Ńq�b�g���Ă���I�u�W�F�N�g���擾
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(info.screenPoint),
                Vector2.zero);
            if (hit.collider != null)
            {
                GameObject hitObj = hit.collider.gameObject;
                //�q�b�g�����I�u�W�F�N�g��tag��bird�������珉�������Ahitobj��o�^
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
            //�N���b�N�n�_�Ńq�b�g���Ă���I�u�W�F�N�g���擾
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(info.screenPoint),
                Vector2.zero);
            if (hit.collider != null)
            {
                GameObject hitObj = hit.collider.gameObject;
                //�q�b�g�I�u�W�F�N�g��tag��bird�����A���O���ꏏ
                //�����hit�����I�u�W�F�N�g�ƈႤ�A���X�g�ɂ͓����ĂȂ�
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
                //���X�g�Ɋi�[����Ă��钹���폜
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
        //���̑���
        renderer.startWidth = 0.1f;
        renderer.endWidth = 0.1f;
        //���_�̐�
        renderer.positionCount = 2;
        //���_��ݒ�
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
            //�����_���ŏo���ʒu���쐬
            Vector2 pos = new Vector2(Random.Range(-3f, 3f), 15f);
            //�o�����钹��ID���쐬
            int id = Random.Range(0, birdPrefabs.Length);

            //���𔭐�������
            GameObject bird = (GameObject)Instantiate(birdPrefabs[id],
                pos,
                Quaternion.AngleAxis(Random.Range(-40, 40), Vector3.forward));
            //�쐬�������̖��O��id���g���Ă��Ȃ���
            bird.name = "Bird" + id;
            yield return new WaitForSeconds(0.05f);


        }
    }
}