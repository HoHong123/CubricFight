using UnityEngine;

public class OrcChif_AnimationController : MonoBehaviour
{
    private Animator m_Ani;
    private ChessFightManage m_FightManager;
    
    [Space(10), Header("스파크 프리팹 & 시각 효과 위치"), Tooltip("Spark Prefab : Asset/PKH/Prefab/Effect/Spark/DAX_Shile_v2_1")]
    [SerializeField] private GameObject m_SparkPrefab;
    [Tooltip("Spark Position : GameObject name = 'Sparkle'")]
    [SerializeField] private Transform m_SparkPosition;

    [Space(10), Header("펀치 프리팹 & 시각 효과 위치"), Tooltip("Heavy Punch : Asset/PKH/Prefab/Effect/CartoonyPunchHeavy 1")]
    [SerializeField] private GameObject m_HeavyPunch;
    [Tooltip("Punch Position : GameObject name = 'PunchLocation'")]
    [SerializeField] private Transform m_PunchPosition;

    [Space(10), Header("운석 패턴 시각 효과 위치")]
    [Tooltip("Punch Position : GameObject name = 'MeteorGesture'")]
    [SerializeField] private Transform m_MeteorGesturePosition;


    public float m_AnimationSpeed {
        get { return m_AnimationSpeed; }
        set {
            if (value > 0 && value <= 2.0f)
            {
                m_Ani.speed = value;
            }

            if(value == 3)
            {
                Destroy(Instantiate(m_SparkPrefab, m_SparkPosition.transform.position, transform.rotation), 2f);
            } else if(value == 4)
            {
                Destroy(Instantiate(m_SparkPrefab, m_MeteorGesturePosition.transform.position, transform.rotation), 2f);
            }
        }
    }
    

    private void Start()
    {
        m_Ani = GetComponent<Animator>();
        m_FightManager = GameObject.Find("GameSystem").GetComponent<ChessFightManage>();
    }

    public void AnimationController(OrcChifState state)
    {
        switch (state)
        {
            case OrcChifState.Axe:
                m_FightManager.soundManager.PlaySkillSoundCilp(2, "Slash_Cast", 0.4f);
                m_Ani.SetTrigger("Attack");
                break;
            case OrcChifState.Dead:
                m_Ani.SetTrigger("Dead");
                break;
            case OrcChifState.Meteor:
                m_FightManager.soundManager.PlaySkillSoundCilp(2, "FireBall_Cast", 0.3f);
                m_Ani.SetTrigger("Meteor");
                break;
            case OrcChifState.Injured:
                m_Ani.SetBool("IsInjured", true);
                break;
        }
    }

    public bool AnimationIsPlaying(string whatIs)
    {
        return m_Ani.GetCurrentAnimatorStateInfo(0).IsName(whatIs);
    }
    
    private void Attack()
    {
        m_FightManager.soundManager.PlaySkillSoundCilp(1, "Slash", 0.5f);

        Destroy(Instantiate(m_HeavyPunch, m_PunchPosition), 1f);

        Collider[] chess = Physics.OverlapSphere(transform.position, 10);
        
        if (chess != null)
        {
            for (int i = 0; i < chess.Length; i++)
            {
                if (chess[i].tag == "Chess" && !(chess[i].name == "model"))
                {
                    float angle = Vector3.Angle(-transform.forward, transform.position - chess[i].gameObject.transform.position);
                    angle *= (chess[i].gameObject.transform.position.x - transform.position.x > 0) ? 1 : -1;
                    
                    if (angle > -45 && angle < 45) // 바라보는 방향 공격
                    {
                        // 데미지 입히기
                        m_FightManager.ImpulseToChess(null, chess[i].GetComponent<ChessState>(), 20);
                        m_FightManager.soundManager.PlaySkillSoundCilp(3, "Slash_Dectect", 0.5f);

                        Vector3 dir = chess[i].transform.position - transform.position;
                        dir = dir.normalized;

                        chess[i].GetComponent<Rigidbody>().AddForce(dir * 35);
                        // Debug.Log(chess[i].name + ", 공격함");
                    }
                }
            }
        }
    }
}
