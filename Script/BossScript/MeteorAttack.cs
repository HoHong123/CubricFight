using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorAttack : MonoBehaviour {

    private ChessFightManage m_FightManager;
    
    [SerializeField] private GameObject m_LandingPosition;
    [SerializeField] private ParticleSystem m_Meteor;   
    [SerializeField] private ParticleSystem m_CircleTimer;

    [Space(10), Header("운석 등장 대기시간")]
    [SerializeField] private float m_MinDelay;
    [SerializeField] private float m_MaxDelay;
    private float m_MeteorDelay;


    // Use this for initialization
    void Start ()
    {
        m_FightManager = GameObject.Find("GameSystem").GetComponent<ChessFightManage>();

        if(m_MinDelay > m_MaxDelay)
        {
            m_MeteorDelay = m_MinDelay;
        } else
        {
            m_MeteorDelay = Random.Range(m_MinDelay, m_MaxDelay);
        }

        m_CircleTimer.gameObject.SetActive(true);
        m_CircleTimer.startLifetime = m_MeteorDelay;
    }
	
	// Update is called once per frame
	void Update ()
    {
		if(m_MeteorDelay > 0)
        {
            m_MeteorDelay -= Time.deltaTime;
            return;
        }

        m_Meteor.gameObject.SetActive(true);

        if (m_Meteor.isStopped)
        {
            m_LandingPosition.SetActive(false);
            SphereRangeCheck();

            Destroy(gameObject, 1);
            enabled = false;
        }
	}

    private void SphereRangeCheck()
    {
        Collider[] chess = Physics.OverlapSphere(transform.position, 4);
        m_FightManager.soundManager.PlaySkillSoundCilp(1, "FireBall", 0.3f);

        if (chess != null)
        {
            for (int i = 0; i < chess.Length; i++)
            {
                if (chess[i].tag == "Chess" && !(chess[i].name == "model"))
                {
                    m_FightManager.soundManager.PlaySkillSoundCilp(3, "FireBall_Dectect", 0.4f);

                    // 데미지 입히기
                    m_FightManager.ImpulseToChess(null, chess[i].GetComponent<ChessState>(), 15);

                    Vector3 dir = chess[i].transform.position - transform.position;

                    dir = (dir == Vector3.zero) ? Vector3.forward : dir.normalized;

                    chess[i].GetComponent<Rigidbody>().AddForce(dir * 40);

                    // Debug.Log(chess[i].name + ", 공격함");
                }
            }
        }
    }
}
