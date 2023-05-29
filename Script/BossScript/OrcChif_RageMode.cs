using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcChif_RageMode : State<OrcChif_AI>
{
    private static OrcChif_RageMode m_Instance; // 해당 클래스 객체
    private OrcChif_RageMode()
    {
        if (m_Instance != null)
        {
            return;
        }

        m_MeteorPos = new List<Vector3>();

        m_Instance = this;
    } // 생성자
    public static OrcChif_RageMode Instance {
        get { // Instance변수를 호출시
            if (m_Instance == null)
            {
                new OrcChif_RageMode(); // 해당 클래스의 객체가 비어있으면 생성자를 통해 생성
            }

            return m_Instance; // 해당 클래스 객체 반환
        }
    }

    private Transform m_BorderPosition;
    private List<Vector3> m_MeteorPos; // 메테오 위치 값 저장

    private int m_NumberOfMeteor = 5;
    private float m_PatternLifeTime;


    public override void EnterState(OrcChif_AI owner)
    {
        Debug.Log("Rage Mode Activated");

        owner.m_AC.AnimationController(OrcChifState.Injured); // IsInjured를 true로 선언

        m_BorderPosition = GameObject.Find("Obj_Board").transform; // 보드를 찾음, x offset (-6, 6), y offset (-13, 13)

        owner.m_MeteorPattern = true;
        owner.m_AC.AnimationController(OrcChifState.Meteor); // 메테오 애니메이션 시작 (Staff-Boost1 0)

        m_PatternLifeTime = owner.m_MeteorDuration; // 메테오 패턴 지속시간만큼 패턴시간 설정
        
        m_MeteorPos.Add(owner.m_Chesses[Random.Range(0, owner.m_Chesses.Count-1)].transform.position); // 존재하는 체스 중 하나를 선택
        Instantiate(owner.m_MeteorPrefab, m_MeteorPos[0], Quaternion.Euler(90, 0, 0)); // 메테오를 체스 위치에 소환


        bool isOverlap; // 다른 원과 겹치는지 확인하는 bool 변수
        Vector3 pos = m_BorderPosition.position;
        pos.y = 1.28f;
        for (int i = 1; i < m_NumberOfMeteor; i++)
        {
            m_MeteorPos.Add(pos);

            // 이전 메테오 소환 위치들과 하나라도 겹치면 다시 위치 재조정
            while (true)
            {
                isOverlap = false;
                m_MeteorPos[i] = pos + new Vector3(Random.Range(-7, 7), 0, Random.Range(-14, 14));

                for (int j = 0; j < i; j++)
                {
                    // 메테오 공격 범위 반지름이 4이며, 각 공격이 겹치지 않도록 거리를 확인하여 위치시킴
                    if (Vector3.Distance(m_MeteorPos[j], m_MeteorPos[i]) < 7)
                    {
                        isOverlap = true; // 하나라도 겹친다면
                        break; // 반복문 break
                    }
                }

                if (isOverlap) // 겹치는게 존재하면
                {
                    continue; // while문 다시 반복
                } else // 그렇지 않으면
                {
                    // 해당 위치에 프리팹 소환
                    Instantiate(owner.m_MeteorPrefab, m_MeteorPos[i], Quaternion.Euler(90,0,0));
                }
                
                break;
            }
        }

        m_MeteorPos.Clear();
    }

    public override void UpdateState(OrcChif_AI owner)
    {

    }

    public override void ExitState(OrcChif_AI owner)
    {
        Debug.Log("Rage Mode Exit");

        owner.m_MeteorPattern = false;
    }
}
