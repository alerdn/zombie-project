using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : NetworkBehaviour
{
    [field: SerializeField] public List<Transform> Waypoints { get; private set; }

    [SerializeField] private Transform _waypointsParent;
    private int _waypointIndex;
    private Coroutine _wanderingRoutine;
    private NavMeshAgent _agent;

    public void Init()
    {
        _agent = GetComponent<NavMeshAgent>();
        Waypoints = _waypointsParent.GetComponentsInChildren<Transform>().ToList();
        Waypoints.RemoveAt(0);
    }

    #region Wandering

    public void StartWandering()
    {
        _waypointIndex = 0;
        _agent.destination = Waypoints[_waypointIndex].position;
        StartCoroutine(WanderingRoutine());
    }

    public void HandleWandering()
    {
        if (_wanderingRoutine == null)
        {
            _wanderingRoutine = StartCoroutine(WanderingRoutine());
        }
    }

    public void StopWandering()
    {
        StopCoroutine(_wanderingRoutine);
        _wanderingRoutine = null;
    }

    private IEnumerator WanderingRoutine()
    {
        // Espera até o inimigo chegar no waypoint
        yield return new WaitUntil(() => Vector3.Distance(transform.position, _agent.destination) <= 1f);

        // Espera 2 segundos antes de ir pro próximo waypoint
        yield return new WaitForSeconds(2f);

        _waypointIndex++;
        _waypointIndex %= Waypoints.Count;

        _agent.destination = Waypoints[_waypointIndex].position;
        _wanderingRoutine = null;
    }

    #endregion

    #region Persuing

    public void StartPersuing()
    {

    }

    public void HandlePersuing()
    {

    }

    public void StopPersuing()
    {

    }

    #endregion
}
