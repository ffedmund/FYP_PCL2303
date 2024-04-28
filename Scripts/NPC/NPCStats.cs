using UnityEngine;
using FYP;

public class NPCStats : CharacterStats {
    public UIEnemyHealthBar npcHealthBar;
    NPCController npcController;
    NPCStateController npcStateController;

    void Awake()
    {
        npcController = GetComponent<NPCController>();
        npcStateController = GetComponent<NPCStateController>();
    }

    void OnDisable() {
        if(isDead)
        {
            isDead = false;
            Start();
        }
    }

    void Start()
    {
        maxHealth = SetMaxHealthFromHealthLevel();
        if(IsOwner)
        {
            currentHealth.Value = maxHealth;
        }
        npcHealthBar.SetMaxHealth(maxHealth);
    }
    

    public override void TakeDamage(int physicalDamage, CharacterManager enemyCharacterDamagingMe)
    {
        base.TakeDamage(physicalDamage, enemyCharacterDamagingMe);
        npcController.animator.Play("Hurt");
        npcStateController.attacker = enemyCharacterDamagingMe.transform;
        if(npcController.npc.isKillable && IsOwner){
            npcHealthBar.SetHealth(currentHealth.Value);

            if (currentHealth.Value <= 0)
            {
                currentHealth.Value = 0;
                npcController.animator.Play("Death");
                isDead = true;
            }
        }
    }
}