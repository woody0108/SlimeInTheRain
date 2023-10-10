using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//나중에 수정할때 카메라 좌표 대입식 말고 좌표를 이동하는 식으로 바꾸기
public class ICamera : MonoBehaviour
{
    #region 변수
    
    private Vector3 vec3;
    private GameObject shopCanvas;
    bool isSTD = false;

    Slime slime;
    #endregion

    private void OnEnable()
    {
        slime = Slime.Instance;

        if (SceneManager.GetActiveScene().buildIndex > 2)
        {
            isSTD = true;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            shopCanvas = GameObject.Find("VillageCanvas").transform.Find("Shop").gameObject;
            isSTD = true;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            isSTD = false;
            Title();
        }
        else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            isSTD = false;
        }
    }

    private void Update()
    {
        if (isSTD)
        {
            if (SceneManager.GetActiveScene().buildIndex != 1)
            {
                STD();
            }
            else
            {
                if(shopCanvas.activeSelf)
                {
                    StartCoroutine(ShopOpen());
                }
                else
                {
                    STD();
                }
            }
        }
        else
        {
            //DontDestroy 코루틴 끌수가 없어서 Update에서 실행
            if(SceneManager.GetActiveScene().buildIndex == 2)
            {
                Result();
            }

        }
    }
    IEnumerator ShopOpen()
    {
        Shop();
        slime.canMove = false;
        while (shopCanvas.activeSelf)
        {
            yield return null;
        }
        slime.canMove = true;
    }


    public void Title()
    {
        vec3.x = -3f;
        vec3.y = 6f;
        vec3.z = -14f;
        Camera.main.transform.SetPositionAndRotation(vec3, Quaternion.Euler(Vector3.right * 30));
        Camera.main.orthographicSize = 3f;
    }
    public void STD()
    {
        vec3.x = slime.transform.position.x;
        vec3.y = 13.0f;
        vec3.z = slime.transform.position.z - 19.0f;
        Camera.main.transform.SetPositionAndRotation(vec3, Quaternion.Euler(Vector3.right * 30));
        Camera.main.orthographicSize = 5f;
    }
    public void Shop()
    {
        slime.canMove = false;
        vec3.x = slime.transform.position.x - 7f;
        vec3.y = 9.1f;
        vec3.z = slime.transform.position.z - 10f;
        Camera.main.transform.position = vec3;
    }
    public void Result()
    {
        vec3.x = 960f;
        vec3.y = 540f;
        vec3.z = -500f;
        Camera.main.transform.SetPositionAndRotation(vec3, new Quaternion());
        Camera.main.orthographicSize = 583f;

    }
}
