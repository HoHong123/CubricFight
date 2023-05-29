using UnityEngine;

public enum OrcChifState
{
    Axe,
    Lazer,
    Double,
    Meteor,
    Injured,
    Dead
}

public class OrcChif_AI : BossRaidGameMode_BossStat
{
    [HideInInspector] public BossMachine<OrcChif_AI> m_StateMachine;

    [Tooltip("애니메이션 컨트롤러가 존재하는 오브젝트의 애니메이션 컨트롤 스크립트")]
    public OrcChif_AnimationController m_AC;
    [Tooltip("애니메이션 컨트롤러가 있는 모델 오브젝트")]
    public GameObject m_CharacterModel;
    

    [Space(10), Header("스킬 쿨타임")]
    public float m_MeleeCoolTime;
    public float m_LazerCoolTime;


    [Space(10), Header("패턴 지속시간")]
    [Range( 5,30)] public float m_NormalDuration; // 근접, 원거리 공격 패턴 지속시간
    [Range(10,20)] public float m_MeteorDuration; // 운석 패턴 지속시간
    private float m_PatternLifeTime;


    [Header("스킬 프리팹")]
    public GameObject m_MeleePrefab;
    public GameObject m_HorizontalPrefab, m_VerticalPrefab;
    public GameObject m_MeteorPrefab;

    [HideInInspector] public bool m_MeteorPattern; // 현재 운석 패턴인가?
    [HideInInspector] public bool m_ChangeMode; // 지금 패턴을 변경할 수 있는가?
    [HideInInspector] public int m_QuaterHealth = m_MaxHealth / 4;

    private ChessState m_ChessState;

    private OrcChif_DoubleAttack m_DoublePattern;
    private OrcChif_RageMode m_RagePattern;

    

    // Use this for initialization
    protected override void Start()
    {
        base.Start(); // 상속받은 부모 스크립의 Start를 먼저 수행

        m_AC = GetComponentInChildren<OrcChif_AnimationController>();

        m_DoublePattern = OrcChif_DoubleAttack.Instance;
        m_RagePattern = OrcChif_RageMode.Instance;

        m_StateMachine = new BossMachine<OrcChif_AI>(this); // 현재 스크립트를 보스 행동 기계에 주인으로 입력
        m_StateMachine.ChangeState(m_DoublePattern); // 시작 패턴 선언

        m_MeteorPattern = true;
        m_ChangeMode = true;

        m_ChessState = GetComponent<ChessState>();
        m_ChessState.chessHp = SetHealth;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update(); // 상속받은 부모 스크립의 Update 먼저 수행

        if (m_QuaterHealth > SetHealth)
        {
            if (m_ChangeMode)
            {
                m_ChangeMode = false;

                if (m_MeteorPattern)
                {
                    // Debug.Log("Rage Called");
                    m_PatternLifeTime = m_MeteorDuration;
                    m_StateMachine.ChangeState(m_RagePattern); // 운석 모드로 변경
                }
                else
                {
                    // Debug.Log("Double Called");
                    m_PatternLifeTime = m_NormalDuration;
                    m_StateMachine.ChangeState(m_DoublePattern); // 일반 공격
                }

                m_MeteorPattern = !m_MeteorPattern;
            }

            m_PatternLifeTime -= Time.deltaTime;

            if (m_PatternLifeTime < 0) // 운석 패턴의 지속시간이 다 지나면
            {
                m_ChangeMode = true; // 패턴 변경 가능
            }
        }

        m_StateMachine.Update();

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Chess")
        {
            TakenDamage(0);
            m_FightManager.soundManager.PlaySkillSoundCilp(3, "Gaurd_Dectect", 0.2f);
            m_FightManager.ImpulseToChess(null, GetComponent<ChessState>(), 50);
            SetHealth -= 50;

            Vector3 I = collision.gameObject.GetComponent<ChessState>().m_velocity; //입사벡터	
            Vector3 N = (collision.gameObject.transform.position - new Vector3(transform.position.x, collision.gameObject.transform.position.y, transform.position.z)).normalized; //표면 노말 벡터
            Vector3 R = I + 2 * (Vector3.Dot(I, -N)) * N;

            collision.gameObject.GetComponent<Rigidbody>().velocity = R;
        }
    }

    public override void Death()
    {
        Destroy(this.gameObject);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 10);
    }
}
