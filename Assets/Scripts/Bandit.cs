using UnityEngine;
using System.Collections;
using Pathfinding;
using System;

public class Bandit : MonoBehaviour {

    private enum AnimState
    {
        Idle,
        Move,
        Attack,
        Die
    }

    [SerializeField] float speed = 200f;

    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRange = 0.5f;
    [SerializeField] LayerMask enemyLayers;

    [SerializeField] float seeingRange = 15f;
    [SerializeField] float nextWaypointDistance = 3f;

    private Animator _animator;
    private Rigidbody2D _body2d;

    private bool _canAttack = true;
    private bool _isDead = false;

    private Transform _target;
    private Seeker _seeker;
    private Path _path;
    private int _currentWaypoint = 0;

    public Action Die;

    private AnimState _animState = AnimState.Idle;

    void Awake () {
        _animator = GetComponent<Animator>();
        _body2d = GetComponent<Rigidbody2D>();
        _seeker = GetComponent<Seeker>();
    }
	
	void FixedUpdate () {
        if (_isDead) return;
        if ((_target != null && _target.GetComponent<Bandit>().isDead()))
        {
            _target = null;
            _animState = AnimState.Idle;
        }

        if (_target == null || _animState == AnimState.Idle)
        {
            Collider2D[] foundEnemies = DetectEnemy(_body2d.position, seeingRange);
            if (foundEnemies.Length <= 0) return;
            int rand = UnityEngine.Random.Range(0, foundEnemies.Length);
            _target = foundEnemies[rand].GetComponent<Transform>();
            _seeker.StartPath(_body2d.position, _target.position, OnPathComplete);
        }

        Move();
        Collider2D[] enemies = DetectEnemy(attackPoint.position, attackRange);
        if (enemies.Length > 0) Attack(enemies);
        PlayAnim();
    }


    #region Public Method

    public void Live()
    {
        float rand = UnityEngine.Random.Range(0.55f, 0.85f);
        attackRange = rand;

        rand = UnityEngine.Random.Range(150f, 200f);
        speed = rand;

        rand = UnityEngine.Random.Range(12f, 20f);
        seeingRange = rand;

        _animState = AnimState.Idle;
        _canAttack = true;
        _isDead = false;
        gameObject.SetActive(true);

        StartCoroutine(StartLifeCycle());
    }

    public void Attacked()
    {
        _animState = AnimState.Die;
        _animator.SetTrigger("Death");
        _isDead = true;
        StartCoroutine(DeathCoroutine());
    }

    public bool isDead()
    {
        return _isDead;
    }

    #endregion

    #region Private Method

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            _path = p;
            _currentWaypoint = 0;
        }
    }

    private void Move()
    {
        if (_animState == AnimState.Die) return;
        if (_path == null) return;
        if (_target == null) return;

        if (_currentWaypoint >= _path.vectorPath.Count)
        {
            _target = null;
            return;
        }

        Vector2 endpoint = (Vector2)_path.vectorPath[_path.vectorPath.Count - 1] - _body2d.position;
        Vector2 direction = ((Vector2)_path.vectorPath[_currentWaypoint] - _body2d.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        _body2d.AddForce(force);

        float distance = Vector2.Distance(_body2d.position, _path.vectorPath[_currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            _currentWaypoint++;
        }

        if (Vector2.Angle(Vector2.right, endpoint) <= 90f)
        {
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }
        else
        {
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }

        if (force.magnitude > 0f && _animState != AnimState.Die)
            _animState = AnimState.Move;
    }

    private Collider2D[] DetectEnemy(Vector2 position, float range)
    {
        return Physics2D.OverlapCircleAll(position, range, enemyLayers);
    }

    private void Attack(Collider2D[] enemies)
    {
        if (!_canAttack) return;

        foreach (Collider2D enemy in enemies)
        {
            enemy.GetComponent<Bandit>().Attacked();
        }

        _animState = AnimState.Attack;
        _canAttack = false;

        StartCoroutine(WaitNextAttackCoroutine());
    }

    private IEnumerator WaitNextAttackCoroutine()
    {
        int rand = UnityEngine.Random.Range(1, 4);
        yield return new WaitForSeconds(rand);
        _canAttack = true;
    }

    private IEnumerator StartLifeCycle()
    {
        yield return new WaitForSeconds(10);
        Attacked();
    }

    private IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(2);
        Die();
    }

    private void PlayAnim()
    {
        if (_animState == AnimState.Attack)
        {
            _animator.SetTrigger("Attack");
            _animState = AnimState.Idle;
        }
        else if (_animState == AnimState.Move)
        {
            _animator.SetInteger("AnimState", 2);
        }
        else
            _animator.SetInteger("AnimState", 0);
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.DrawWireSphere(_body2d.position, seeingRange);
    //}

    #endregion
}
