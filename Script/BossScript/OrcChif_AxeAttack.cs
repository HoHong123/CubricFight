using UnityEngine;
using System.Collections.Generic;

public class OrcChif_AxeAttack : State<OrcChif_AI>
{
    private static OrcChif_AxeAttack m_Instance; // 해당 클래스 객체
    private OrcChif_AxeAttack()
    {
        if (m_Instance != null)
        {
            return;
        }

        m_Instance = this;
    } // 생성자
    public static OrcChif_AxeAttack Instance {
        get { // Instance변수를 호출시
            if (m_Instance == null)
            {
                new OrcChif_AxeAttack(); // 해당 클래스의 객체가 비어있으면 생성자를 통해 생성
            }

            return m_Instance; // 해당 클래스 객체 반환
        }
    }


    private struct AttackDirection
    {
        public int s_SelectedDirection;
        public bool s_Foward, s_Right, s_Left, s_Back;
        public List<string> list;

        public void reset()
        {
            s_SelectedDirection = 0;
            s_Foward = s_Right = s_Left = s_Back = false;
            list.Clear();
        }
    };
    private AttackDirection m_AttackDirection;
    
    private float m_AxeCoolDown;
    private float m_CurrentAttackDirection;
    

    public override void EnterState(OrcChif_AI owner)
    {
        m_AxeCoolDown = Random.Range(owner.m_MeleeCoolTime, owner.m_MeleeCoolTime + 3); // 최초 쿨타임 정의
        
        m_AttackDirection.list = new List<string>(); // AttackDirection구조체의 리스트를 메모리 할당하여 존재하는 변수로 만듬
        m_AttackDirection.reset(); // 구조체의 모든 변수를 초기화하는 작업
    }

    public override void UpdateState(OrcChif_AI owner)
    {
        if (m_AxeCoolDown < 0) // 도끼공격 쿨타임이 다 되면 실행
        {
            m_AxeCoolDown = Random.Range(owner.m_MeleeCoolTime, owner.m_MeleeCoolTime + 2); // 도끼공격 쿨타임 갱신, 4~6초 사이
            
            for (int i = 0; i < owner.m_Chesses.Count; i++)
            {
                if(owner.m_Chesses[i] != null)
                {
                    if (Vector3.Distance(owner.transform.position, owner.m_Chesses[i].transform.position) < 10)
                    {
                        float angle = Vector3.Angle(-owner.transform.forward, owner.transform.position - owner.m_Chesses[i].transform.position); // 정면이면 0, 우측 90, 좌측 90을 생성
                        angle *= (owner.m_Chesses[i].transform.position.x - owner.transform.position.x > 0) ? 1 : -1; // 체스가 좌측에 있으면 -1을 곱하여 -90으로 변환
                        
                        TargetLocate(angle);
                    }
                }
            }

            if (m_AttackDirection.s_SelectedDirection > 0) // 한 곳이라도 공격가능한 영역이 있다면 공격
            {
                owner.m_AC.AnimationController(OrcChifState.Axe); // 공격 애니메이션 실행

                m_CurrentAttackDirection = RandomAttackDirection();
                owner.m_CharacterModel.transform.rotation = Quaternion.Euler(0, m_CurrentAttackDirection, 0); // 공격 각도 실행
                owner.m_MeleePrefab.transform.rotation = Quaternion.Euler(90, 0, 360 - m_CurrentAttackDirection);
                owner.m_MeleePrefab.transform.position = owner.transform.position;

                Destroy(Instantiate(owner.m_MeleePrefab), 3f);

                m_AttackDirection.reset(); // 범위 안에 있는 체스를 확인하는 모든 변수 초기화
            }

            return;
        }

        if (!owner.m_AC.AnimationIsPlaying("OrcChif-Mace-Attack-R1"))
        {
            m_AxeCoolDown -= Time.deltaTime;
        }
    }

    public override void ExitState(OrcChif_AI owner)
    {
        
    }
    
    // 각 체스들이 공격 범위에 들어오면 (전/우/좌) 중 어디에 있는지 각도로 알아내는 함수
    private void TargetLocate(float angle)
    {
        /* 
         * 전방에 체스가 2개가 존재한다면
         * 첫 체스를 인식할때, 전방을 공격해야한다고 리스트에 "Foward"가 적용된다.
         * 그러므로 두 번째 체스가 전방에 있어도 더이상 리스트에 "Foward"가 적용될 필요가 없다.
         * 이를 방지하기위해 s_Foward를 통해 전방에 적을 이미 인식했는지 확인한다.
         * 다른 각도도 동일한 조건문을 통한다.
         */

        if (angle > -45 && angle < 45) // 정면
        {
            if (!m_AttackDirection.s_Foward)
            {
                m_AttackDirection.s_Foward = true;
                m_AttackDirection.list.Add("Foward");
                m_AttackDirection.s_SelectedDirection++;
            }
        }
        else if (angle > 45 && angle < 135) // 우측
        {
            if (!m_AttackDirection.s_Right)
            {
                m_AttackDirection.s_Right = true;
                m_AttackDirection.list.Add("Right");
                m_AttackDirection.s_SelectedDirection++;
            }
        }
        else if (angle < -45 && angle > -135) // 좌측
        {
            if (!m_AttackDirection.s_Left)
            {
                m_AttackDirection.s_Left = true;
                m_AttackDirection.list.Add("Left");
                m_AttackDirection.s_SelectedDirection++;
            }
        } else
        {
            if (!m_AttackDirection.s_Back)
            {
                m_AttackDirection.s_Back = true;
                m_AttackDirection.list.Add("Back");
                m_AttackDirection.s_SelectedDirection++;
            }
        }
    }

    // 각 체스들이 존재하는 방향을 알고 있는 상태에서 어떤 방향을 공격할지 랜덤으로 정하여 공격하는 함수
    private float RandomAttackDirection()
    {
        int selection = 0;
        
        // 리스트에 저장된 체스가 존재하는 방향 중 랜덤한 방향 선택
        switch (m_AttackDirection.s_SelectedDirection)
        {
            case 1: // 공격 가능 방향이 1개 일때
                selection = 1; // 체스가 있는 1개의 방향 공격
                break;
            case 2: // 공격 가능 방향이 2개 일때
                selection = Random.Range(1, 2); // 1과 2중에 한 방향 공격
                break;
            case 3: // 공격 가능 방향이 3개 일때
                selection = Random.Range(1, 3); // 1~3 중 한 방향 공격
                break;
            case 4:
                selection = Random.Range(1, 4); // 전 방향 중 한곳 공격
                break;
        }

        // 해당 방향이 어떤 방향인지 확인 후 공격할 각도를 정함
        switch (m_AttackDirection.list[selection-1])
        {
            // 월드 좌표 기준
            case "Foward": // 정면 각도 반환
                selection = 180;
                break;
            case "Right": // 우측 각도 반환
                selection = 90;
                break;
            case "Left": // 좌측 각도 반환
                selection = 270;
                break;
            case "Back": // 후방 각도 반환
                selection = 0;
                break;
        }

        return selection;
    }
}
