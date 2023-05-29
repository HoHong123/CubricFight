using UnityEngine;

public class OrcChif_DoubleAttack : State<OrcChif_AI>
{
    private static OrcChif_DoubleAttack m_Instance; // 해당 클래스 객체
    private OrcChif_DoubleAttack()
    {
        if (m_Instance != null)
        {
            return;
        }
        
        m_AxeAttack = OrcChif_AxeAttack.Instance; // 도끼 공격 스크립트를 메모리에 할당하며 스크립트 객체 생성
        m_LazerAttack = OrcChif_LazerAttack.Instance; // 레이저 공격 스크립트를 메모리에 할당하며 스크립트 객체 생성

        m_Instance = this;
    } // 생성자
    public static OrcChif_DoubleAttack Instance {
        get { // Instance변수를 호출시
            if (m_Instance == null)
            {
                new OrcChif_DoubleAttack(); // 해당 클래스의 객체가 비어있으면 생성자를 통해 생성
            }

            return m_Instance; // 해당 클래스 객체 반환
        }
    }
    
    private OrcChif_AxeAttack m_AxeAttack;
    private OrcChif_LazerAttack m_LazerAttack;

    private float m_PatternLifeTime;


    public override void EnterState(OrcChif_AI owner)
    {
        Debug.Log("Double Activated");
        m_PatternLifeTime = owner.m_NormalDuration;

        m_AxeAttack.EnterState(owner);
        m_LazerAttack.EnterState(owner);
    }

    public override void UpdateState(OrcChif_AI owner)
    {
        m_AxeAttack.UpdateState(owner);
        m_LazerAttack.UpdateState(owner);
    }
    
    public override void ExitState(OrcChif_AI owner)
    {
        Debug.Log("Double Exit");
    }
}
