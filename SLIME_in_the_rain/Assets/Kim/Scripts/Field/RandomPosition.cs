/**
 * @brief ���� ������ ��ġ�� ��ȯ
 * @author ��̼�
 * @date 22-08-06
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class RandomPosition : MonoBehaviour
{

    /// <summary>
    /// �׺���̼� �޽� ���� �ȿ��� ������ ��ġ ��ȯ
    /// </summary>
    /// <param name="center">������</param>
    /// <param name="range">������ ����</param>
    /// <param name="result">��ȯ�� ���� ������</param>
    /// <returns>�������� ã���� �� true ��ȯ</returns>
    public static bool GetRandomNavPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;

        return false;
    }

    //private static Vector3 spawnPosition;
    //private static float randomX;
    //private static float randomZ;

    //// ���� �߿��� ������ ��ġ�� ������
    //public static Vector3 GetRandomPosition(float minX, float maxX, float minZ, float maxZ, float y)
    //{
    //    randomX = Random.Range(minX, maxX);
    //    randomZ = Random.Range(minZ, maxZ);

    //    spawnPosition = new Vector3(randomX, y, randomZ);

    //    return spawnPosition;
    //}
}
