using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerAttackRange : MonoBehaviour {

    private ChessFightManage m_FightManager;

    [Space(10)]
    [SerializeField] private GameObject m_LazerPrefab;
    [SerializeField] private SpriteRenderer m_SR;
    [SerializeField] private bool m_Vertical;

    private float m_YAxis;
    private float m_Timer = 2.0f;
    private Vector3 m_Pos;
    private Vector3 m_CheckRange;

    private Color color;
    private float lerp;
    private bool fade;


    private void Start()
    {
        if (m_Vertical)
        {
            m_YAxis = 180;
            m_Pos = new Vector3(0, 1, 16);
            m_CheckRange = new Vector3(1.5f, 3, 20);
        } else
        {
            m_YAxis = 90;
            m_Pos = new Vector3(-14, 1, 0);
            m_CheckRange = new Vector3(20, 3, 1.5f);
        }


        m_FightManager = GameObject.Find("GameSystem").GetComponent<ChessFightManage>();
        m_FightManager.soundManager.PlaySkillSoundCilp(2, "Thunder_Cast", 0.3f);
        color = m_SR.color;

        lerp = 0;
        fade = true;
    }

    // Update is called once per frame
    void Update ()
    {
		if(m_Timer < 0)
        {           
            // 1초뒤 제거되는 프리팹 생성, 해당 객체보다 y축 2 위에 생성
            Destroy(Instantiate(m_LazerPrefab, transform.position + m_Pos, Quaternion.Euler(0, m_YAxis, 0)), 1.0f);

            BoxRangeCheck();

            enabled = false;
        } else
        {
            m_Timer -= Time.deltaTime;

            if (m_SR.color.a < 1 && fade)
            {
                lerp += Time.deltaTime; // 1초간 색이 진해짐

                color.a = Mathf.Lerp(0, 1, lerp);

                m_SR.color = color;

                if (m_SR.color.a >= 1)
                {
                    fade = false;
                    lerp = 0;
                    color.a = 1;
                }
            }
            else if (!fade)
            {
                lerp += Time.deltaTime; // 1초간 색이 옅어짐

                color.a = Mathf.Lerp(1, 0, lerp);

                m_SR.color = color;
            }
        }
	}

    private void BoxRangeCheck()
    {
        Collider[] chess = Physics.OverlapBox(transform.position, m_CheckRange);
        m_FightManager.soundManager.PlaySkillSoundCilp(1, "Thunder", 0.3f);

        if (chess != null)
        {
            for (int i = 0; i < chess.Length; i++)
            {
                if (chess[i].tag == "Chess" && !(chess[i].name == "model"))
                {                   
                    // 데미지 입히기
                    m_FightManager.ImpulseToChess(null, chess[i].GetComponent<ChessState>(), 15);
                    m_FightManager.soundManager.PlaySkillSoundCilp(3, "Thunder_Dectect", 0.3f);

                    // Debug.Log(chess[i].name + ", 공격함");
                }
            }
        }
    }
}
