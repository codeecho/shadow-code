using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [System.Serializable]
    public class BossPhase
    {
        public Vector3 position;
        public bool moveRandomly = false;
        public float speed = 1f;
        public float duration = 5f;
        public float damage = 10f;
    }

    public List<BossPhase> phases = new List<BossPhase>();
    public bool cycle = false;

    private int currentPhaseIndex = 0;
    private BossPhase currentPhase;
    private bool inPostion = false;
    private float phaseTimer = 0f;
    private bool finished = false;
    private MoveRandomly moveRandomly;
    private Health health;

    private float healthRemaining = 0;
    private float damageTaken = 0;

    void Start()
    {
        health = GetComponent<Health>();
        healthRemaining = health.health;
        health.onTakeDamage.AddListener(OnTakeDamage);
        moveRandomly = GetComponent<MoveRandomly>();
        currentPhase = phases[currentPhaseIndex];
    }

    void Update()
    {
        if (finished)
        {
            return;
        }
        if (!inPostion)
        {
            MoveToStartPosition();
            CheckReachedPosition();
        }
        phaseTimer += Time.deltaTime;
        if (phaseTimer >= currentPhase.duration)
        {
            MoveToNextPhase();
        }
    }

    private void MoveToNextPhase()
    {
        EventManager.Trigger("BossPhaseEnd");
        phaseTimer = 0f;
        damageTaken = 0;
        currentPhaseIndex++;
        if (currentPhaseIndex >= phases.Count)
        {
            if (cycle)
            {
                currentPhaseIndex = 0;
            }
            else
            {
                finished = true;
                return;
            }
        }
        currentPhase = phases[currentPhaseIndex];
        inPostion = false;
        if (moveRandomly) moveRandomly.enabled = false;
    }

    private void MoveToStartPosition()
    {
        transform.position = Vector3.MoveTowards(transform.position, currentPhase.position, currentPhase.speed * Time.deltaTime);
    }

    private void CheckReachedPosition()
    {
        if (Vector3.Distance(transform.position, currentPhase.position) < 0.1f)
        {
            inPostion = true;
            if (moveRandomly) moveRandomly.enabled = currentPhase.moveRandomly;
            EventManager.Trigger("BossPhase" + (currentPhaseIndex + 1) + "Start");
        }
    }

    void OnTakeDamage()
    {
        damageTaken += healthRemaining - health.health;
        healthRemaining = health.health;
        if (currentPhase.damage > 0 && damageTaken >= currentPhase.damage)
        {
            MoveToNextPhase();
        }
    }
}
