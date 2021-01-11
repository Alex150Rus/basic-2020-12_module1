using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationEvents : MonoBehaviour
{
    private Character _character;

    void Start()
    {
        _character = GetComponentInParent<Character>();
    }

    // в анимации m_pistol_shoot, на определённом кадре, во вкладке Events, добавили событие и название этой функции
    private void ShootEnd()
    {
        _character.SetState(Character.State.Idle);
    }

    private void AttackEnd()
    {
        _character.SetState(Character.State.RunningFromEnemy);
    }
}
