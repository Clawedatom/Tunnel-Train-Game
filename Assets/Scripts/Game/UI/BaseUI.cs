using UnityEngine;

public class BaseUI : MonoBehaviour
{
    [Header("Base UI Fields")]
    [SerializeField] private GameObject screenGO;
    [SerializeField] private UIState state;

    public UIState GetState => state;

    public virtual void OnEnableUI()
    {
        screenGO.SetActive(true);
    }

    public virtual void OnDisableUI()
    {
        screenGO.SetActive(false);
    }

    public virtual void OnAwake()
    {

    }

    public virtual void OnStart()
    {

    }
}

public enum UIState
{
    InGame,
    TunnelComplete,
    LevelComplete,
    LevelFail,
    Tutorial,
    Pause
}