using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BossRaidGameMode_BossStat : MonoBehaviour
{
    [Header("체력")]
    [HideInInspector] protected const int m_MaxHealth = 1000;
    [SerializeField] private int m_Health;
    public int SetHealth {
        get {
            return m_Health;
        }

        set {
            if (m_Health > 0)
            {
                m_Health = value;
            }
            else
            {
                Death();
            }
        }
    }


    [Space(10)]
    [Header("모델"), Tooltip("머테리얼이 존재하는 오브젝트를 넣어주어야함, \"Characters\" 오브젝트")]
    [SerializeField] private SkinnedMeshRenderer m_Model;
    private Material m_Skin;
    
    [HideInInspector] public List<GameObject> m_Chesses;
    [HideInInspector] public ChessFightManage m_FightManager;

    // Use this for initialization
    protected virtual void Start()
    {
        SetHealth = m_MaxHealth;

        if (m_Model != null)
        {
            m_Skin = m_Model.material;
        }

        GameObject[] game = GameObject.FindGameObjectsWithTag("Chess"); // 모든 Chess태그 오브젝트를 가져옴

        if (game != null)
        {
            m_Chesses = new List<GameObject>();

            for (int i = 0; i < game.Length; i++)
            {
                if(game[i].name != "model")
                {
                    m_Chesses.Add(game[i]);
                }
            }
        }

        m_FightManager = GameObject.Find("GameSystem").GetComponent<ChessFightManage>();
    }

    protected virtual void Update()
    {
        for (int i = 0; i < m_Chesses.Count; i++) // 매번 남은 체스의 수를 확인
        {
            if (!m_Chesses[i].GetComponent<ChessState>().IsAlive)
            {
                m_Chesses.RemoveAt(i);
            }
        }
    }

    public virtual void TakenDamage(int damageInput) // 데미지를 입을때 발동되는 가상함수, 재정의 가능
    {
        SetHealth -= damageInput;
        
        StartCoroutine("BossFadeToRed");
    }

    private IEnumerator BossFadeToRed()
    {
        Color color = new Color(1.0f, 0, 0);
        m_Skin.color = color;

        for (float i = 0; i < 1; i += 0.1f)
        {
            color.g = color.b = i;

            m_Skin.color = color;
            yield return new WaitForEndOfFrame();
        }

        m_Skin.color = new Color(1, 1, 1);
    } // 보스의 색상 붉게 변경 후 돌아오는 함수

    public abstract void Death(); // 죽을때 발동되는 추상 함수, 상속받는 자식 스크립에서 정의해야함
}


[System.Serializable]
public class BossMachine<T>
{
    public State<T> m_CurrentState { get; private set; }
    public T m_Owner;
    

    public BossMachine(T owner)
    {
        m_Owner = owner;
        m_CurrentState = null;
    }

    public void ChangeState(State<T> newstate)
    {
        if (m_CurrentState != null)
        {
            m_CurrentState.ExitState(m_Owner);
        }
        
        m_CurrentState = newstate;
        m_CurrentState.EnterState(m_Owner);
    }

    public void Update()
    {
        m_CurrentState.UpdateState(m_Owner);
    }
}

public abstract class State<T> : MonoBehaviour
{
    public abstract void EnterState(T owner); // 새로운 상태 입력
    public abstract void ExitState(T owner); // 상태 종료
    public abstract void UpdateState(T owner); // 상태 업데이트
}