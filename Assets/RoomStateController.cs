using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RoomStateController : MonoBehaviour
{
    [SerializeField] private RoomStateSettings settings;
    [SerializeField] private GameObject regularLights;
    [SerializeField] private GameObject preparationLights;
    [SerializeField] private Animation alarmAnimation;
    [SerializeField] private float timeBetweenLights;

    private RoomState _currentState;
    private float _currentTime;
    private float _damageCooldown;
    private List<PlayerCore> _playersInRoom = new List<PlayerCore>();

    [SerializeField] private Collider _roomTrigger;

    private void Awake()
    {
        alarmAnimation = preparationLights.GetComponent<Animation>();

    }

    private void Start()
    {
        InitializeTime();
        UpdateInitialState();
    }

    private void Update()
    {
        _currentTime += Time.deltaTime;
        HandleStateTransitions();
        HandleIncinerationDamage();
    }

    private void InitializeTime()
    {
        _currentTime = -Random.Range(settings.minStartTime, settings.maxStartTime);
    }

    private void UpdateInitialState()
    {
        HandleStateTransitions(forceUpdate: true);
    }

    private void HandleStateTransitions(bool forceUpdate = false)
    {
        if (forceUpdate || _currentState == RoomState.Stable && _currentTime >= settings.preparationTime)
        {
            if (_currentTime >= settings.preparationTime)
            {
                SetState(RoomState.Preparation);
            }
        }

        if (forceUpdate || _currentState == RoomState.Preparation && _currentTime >= settings.incinerationTime)
        {
            if (_currentTime >= settings.incinerationTime)
            {
                SetState(RoomState.Incineration);
            }
        }
    }

    private void SetState(RoomState newState)
    {
        _currentState = newState;
        UpdateLights();
    }

    private void UpdateLights()
    {

        StartCoroutine(UpdateLightsWithPause());
    }

    IEnumerator UpdateLightsWithPause()
    {
        regularLights.SetActive(_currentState == RoomState.Stable);
        yield return new WaitForSeconds(timeBetweenLights);
        preparationLights.SetActive(_currentState != RoomState.Stable);
        alarmAnimation?.Play();
    }

    private void HandleIncinerationDamage()
    {
        if (_currentState != RoomState.Incineration) return;

        _damageCooldown -= Time.deltaTime;
        if (_damageCooldown <= 0)
        {
            ApplyDamage();
            ResetDamageCooldown();
        }
    }

    private void ApplyDamage()
    {
        foreach (var player in _playersInRoom)
        {
            player.TakeDamage();
        }
    }

    private void ResetDamageCooldown()
    {
        _damageCooldown = settings.damagePause;
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerCore>();
        if (player != null && !_playersInRoom.Contains(player))
        {
            _playersInRoom.Add(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<PlayerCore>();
        if (player != null)
        {
            _playersInRoom.Remove(player);
        }
    }
}