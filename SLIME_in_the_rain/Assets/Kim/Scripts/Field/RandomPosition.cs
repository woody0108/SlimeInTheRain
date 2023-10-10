/**
 * @brief 맵의 랜덤한 위치를 반환
 * @author 김미성
 * @date 22-08-06
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class RandomPosition : MonoBehaviour
{

    /// <summary>
    /// 네비게이션 메시 범위 안에서 랜덤한 위치 반환
    /// </summary>
    /// <param name="center">목적지</param>
    /// <param name="range">목적지 범위</param>
    /// <param name="result">반환할 랜덤 목적지</param>
    /// <returns>목적지를 찾았을 때 true 반환</returns>
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

    //// 범위 중에서 랜덤한 위치를 가져옴
    //public static Vector3 GetRandomPosition(float minX, float maxX, float minZ, float maxZ, float y)
    //{
    //    randomX = Random.Range(minX, maxX);
    //    randomZ = Random.Range(minZ, maxZ);

    //    spawnPosition = new Vector3(randomX, y, randomZ);

    //    return spawnPosition;
    //}
}
