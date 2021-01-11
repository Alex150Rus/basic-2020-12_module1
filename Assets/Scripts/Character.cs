using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum State
    {
        Idle,
        RunningToEnemy,
        RunningFromEnemy,
        BeginAttack,
        Attack,
        BeginShoot,
        Shoot,
    }

    public enum Weapon
    {
        Pistol,
        Bat,
    }

    private Animator _animator;
    private State _state;

    [SerializeField] private Weapon weapon;
    [SerializeField] private Transform target;
    [SerializeField] private float runSpeed;
    
    //нужно для хулигана для атаки битой
    [SerializeField] private float distanceFromEnemy;
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;

    private void Start()
    {
        //шаг1
        _animator = GetComponentInChildren<Animator>();
        _state = State.Idle;
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
    }

    public void SetState(State newState)
    {
        _state = newState;
    }

    //три точки компонента скрипт (сверху справа)
    [ContextMenu("Attack")]
    private void AttackEnemy()
    {
        switch (weapon) {
            case Weapon.Bat:
                _state = State.RunningToEnemy;
                break;
            case Weapon.Pistol:
                _state = State.BeginShoot;
                break;
        }
    }

    /// <summary>
    /// Перемещает хулигана от врага и обратно.
    /// </summary>
    /// <param name="targetPosition">Позиция врага. Устанавливается в редакторе</param>
    /// <param name="distanceFromTarget">Расстояние до врага. Устанавливается в редакторе</param>
    /// <returns></returns>
    private bool RunTowards(Vector3 targetPosition, float distanceFromTarget)
    {
        Vector3 distance = targetPosition - transform.position;
        if (distance.magnitude < 0.00001f) {
            transform.position = targetPosition;
            return true;
        }

        Vector3 direction = distance.normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        targetPosition -= direction * distanceFromTarget;
        distance = (targetPosition - transform.position);

        Vector3 step = direction * runSpeed;
        if (step.magnitude < distance.magnitude) {
            transform.position += step;
            return false;
        }

        transform.position = targetPosition;
        return true;
    }

    private void FixedUpdate()
    {
        // переходы состояний
        switch (_state) {
            // 1) первоначальное состояние
            case State.Idle:
                transform.rotation = _originalRotation;
                _animator.SetFloat("Speed", 0.0f);
                break;

            // 2) состояние после выбора биты в качестве оружия
            case State.RunningToEnemy:
                // анимация бега срабатывает при условии, что скорость больше нуля. Мы здесь устанавливаем эту скорость
                _animator.SetFloat("Speed", runSpeed);
                if (RunTowards(target.position, distanceFromEnemy))
                    _state = State.BeginAttack;
                break;

            // устанавливается при возникновении события ShootEnd в скрипте CharacterAnimationEvents
            case State.RunningFromEnemy:
                _animator.SetFloat("Speed", runSpeed);
                if (RunTowards(_originalPosition, 0.0f))
                    _state = State.Idle;
                break;

            case State.BeginAttack:
                _animator.SetTrigger("MeleeAttack");
                _state = State.Attack;
                break;

            case State.Attack:
                break;

            // 2) состояние после выбора пистолета в качестве оружия
            case State.BeginShoot:
                // отрабатывает анимация выстрела. Она у нас запускается по триггеру.
                _animator.SetTrigger("Shoot");
                _state = State.Shoot;
                break;

            case State.Shoot:
                break;
        }
    }
}
