using System;
using UnityEngine;

public class GamePhaseManager : MonoBehaviour
{
    //Manages phaes of game

    //e.g
    //phase 1 - drill phase: hud, player movement etc, open ui at the end and validate path
    //phase 2 - Train phase: 

  

    public static GamePhaseManager _instance;

    public static GamePhaseManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<GamePhaseManager>();
            }
            if (_instance == null)
            {
                Debug.LogError("GamePhaseManager has not been assigned");
            }
            return _instance;
        }
    }

    public void OnAwake()
    {
        tunnelPhase = new Phase();
        trainPhase = new Phase();
    }



    public Phase tunnelPhase;
    public Phase trainPhase;

    private void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }
    //all ran from game maanger
    public void Phase_StartTunnelPhase() => tunnelPhase.onPhaseStart();
    public void Phase_EndTunnelPhase() => tunnelPhase.onPhaseEnd();

    public void Phase_StartTrainPhase() => trainPhase.onPhaseStart();
    public void Phase_EndTrainPhase() => trainPhase.onPhaseEnd();
    public void Phase_FailTrainPhase() => trainPhase.onPhaseFail();
    public void Phase_FailTunnelPhase() => tunnelPhase.onPhaseFail();
}

[Serializable]
public class Phase
{
    public event Action onStart;
    public event Action onEnd;
    public event Action onFail;


    public void onPhaseStart()
    {

        onStart.Invoke();

    }

    public void onPhaseEnd()
    {
        onEnd.Invoke();
    }

    public void onPhaseFail()
    {
       onFail.Invoke();
    }
}