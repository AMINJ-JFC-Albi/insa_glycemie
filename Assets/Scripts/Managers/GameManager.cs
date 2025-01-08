using UnityEngine;
using System.Collections.Generic;
using System;
using Tools;
using States;

public class GameManager : MonoBehaviour
{
    internal enum MainState
    {
        Tuto,
        Part1,
        Part2,
        Part3,
        Part4
    }

    internal enum Part1State
    {
        Intro,
        Aiguille,
        Protection,
        Carte,
        Coque
    }

    internal enum Part2State
    {
        Intro,
        Phase1,
        Phase2,
        Phase3
    }

    internal StateMachine<MainState> mainStateMachine;
    internal DataCollectorManager datas;

    private string oldID, currentID, oldSubID, currentSubID;
    private string oldStepName, newStepName, oldSubStepName, newSubStepName;
    private EventsManager eventsManager;

    internal static GameManager Instance;

    //GameObjects & Components used:
    [SerializeField] private GameObject holder;
    [SerializeField] private GameObject partsFolder;
    [SerializeField] private GameObject explodedView;
    [SerializeField] private DialogueSystem dialogueSystemStep1;
    [SerializeField] private SciFiDoor sciFiDoor1, sciFiDoor2, sciFiDoor3;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        eventsManager = gameObject.AddComponent<EventsManager>();
        datas = new("Name;OldState;NewState;TimeStamp;TimeStampSinceStart;Infos", ';');
        datas.LoadData();

        InitializeStateMachine();
        RegisterEvents();
        FirstActions();
    }

    void Update()
    {
        mainStateMachine.Update();

        if (Input.GetKeyDown(KeyCode.A))
        {
            eventsManager.TriggerEvent("NextState");
        }
    }

    private void InitializeStateMachine()
    {
        var mainStateActions = new Dictionary<MainState, Action>
        {
            { MainState.Tuto, null },
            { MainState.Part1, null },
            { MainState.Part2, null },
            { MainState.Part3, null },
            { MainState.Part4, null },
        };

        var mainTransitionsActions = new Dictionary<(MainState, MainState), Action>
        {
            { (MainState.Tuto, MainState.Part1), OnIntroPart1 },
            { (MainState.Part1, MainState.Part2), OnIntroPart2 },
            { (MainState.Part2, MainState.Part3), null },
            { (MainState.Part3, MainState.Part4), null },
            { (MainState.Part4, MainState.Part4), null }
        };

        var part1StateActions = new Dictionary<Part1State, Action>
        {
            { Part1State.Intro, null },
            { Part1State.Aiguille, null },
            { Part1State.Protection, null },
            { Part1State.Carte, null },
            { Part1State.Coque, null }
        };

        var part1TransitionsActions = new Dictionary<(Part1State, Part1State), Action>
        {
            { (Part1State.Intro, Part1State.Aiguille), OnAiguillePlacement },
            { (Part1State.Aiguille, Part1State.Protection), OnProtectionPlacement },
            { (Part1State.Protection, Part1State.Carte), OnMotherboardPlacement },
            { (Part1State.Carte, Part1State.Coque), OnCoquePlacement },
            { (Part1State.Coque, Part1State.Coque), null }
        };

        var part2StateActions = new Dictionary<Part2State, Action>
        {
            { Part2State.Intro, null },
            { Part2State.Phase1, null },
            { Part2State.Phase2, null },
            { Part2State.Phase3, null }
        };

        var part2TransitionsActions = new Dictionary<(Part2State, Part2State), Action>
        {
            { (Part2State.Intro, Part2State.Phase1), OnPhase1 },
            { (Part2State.Phase1, Part2State.Phase2), OnPhase2 },
            { (Part2State.Phase2, Part2State.Phase3), OnPhase3 },
            { (Part2State.Phase3, Part2State.Phase3), null }
        };

        var part1StateMachine = new StateMachine<Part1State>(Part1State.Intro, part1StateActions, part1TransitionsActions, onStateExecute: false);
        var part2StateMachine = new StateMachine<Part2State>(Part2State.Intro, part2StateActions, part2TransitionsActions, onStateExecute : false);

        var subStateMachines = new Dictionary<MainState, IStateMachine>
        {
            { MainState.Part1, part1StateMachine },
            { MainState.Part2, part2StateMachine }
        };

        var subStateTypes = new Dictionary<MainState, Type>
        {
            { MainState.Part1, typeof(Part1State) },
            { MainState.Part2, typeof(Part2State) }
        };

        mainStateMachine = new StateMachine<MainState>(MainState.Tuto, mainStateActions, mainTransitionsActions, subStateMachines, subStateTypes);
    }

    private void RegisterEvents()
    {
        eventsManager.RegisterEvent("NextState", () => HandleNextState());
    }

    private bool skipTutorial = false;
    public void SkipTutorial()
    {
        if (!skipTutorial)
        {
            skipTutorial = true;
            HandleNextState();
        }
    }

    internal void HandleNextState()
    {
        if (mainStateMachine.IsLastSubState())
        {
            HandleNextStep();
        }
        else
        {
            HandleNextSubStep();
        }
        datas.SaveData();
    }

    internal void HandleNextStep(string infos = "")
    {
        oldID = $"NEXT_STEP_{oldStepName};{newStepName}";
        oldStepName = GameManager.Instance.mainStateMachine.CurrentStringState();
        newStepName = mainStateMachine.IncrementState().ToString();
        currentID = $"NEXT_STEP_{oldStepName};{newStepName}";
        SaveNextStep(oldStepName, newStepName, oldID, currentID, infos);
    }

    internal void HandleNextSubStep(string infos = "")
    {
        oldSubID = $"NEXT_SUBSTEP_{oldSubStepName};{newSubStepName}";
        oldSubStepName = GameManager.Instance.mainStateMachine.GetCurrentSubState().ToString();
        mainStateMachine.IncrementSubState();
        newSubStepName = GameManager.Instance.mainStateMachine.GetCurrentSubState().ToString();
        currentSubID = $"NEXT_SUBSTEP_{oldSubStepName};{newSubStepName}";
        SaveNextSubStep(oldSubStepName, newSubStepName, oldSubID, currentSubID, infos);
    }

    // Save Datas : "Name;OldState;NewState;TimeStamp;TimeStampSinceStart;Infos"
    internal void SaveGhostSubStepTime()
    {
        SaveGhostSubStepTime(GameManager.Instance.mainStateMachine.GetCurrentSubState().ToString());
    }

    internal void SaveGhostSubStepTime(string subState)
    {
        GameManager.Instance.datas.EditGhostData($"SUBSTEP_{subState}");
    }

    private TimeSpan timeSpanTMP = TimeSpan.Zero;

    internal void SaveNextStep(string oldStepName, string newStepName, string oldID, string currentID, string infos = "")
    {
        if (oldStepName != newStepName)
        {
            GameManager.Instance.datas.AddData($"NEXT_STEP_{oldStepName};{newStepName}", "NEXT_STEP", "");
            timeSpanTMP = GameManager.Instance.datas.CalculateTotalTime(currentID);
            string timeStamp2 = timeSpanTMP.ToString();
            string timeStamp1;
            if (oldID == "NEXT_STEP_;")
                timeStamp1 = timeStamp2;
            else
                timeStamp1 = GameManager.Instance.datas.CalculateTimeDifference(oldID, currentID).ToString();
            GameManager.Instance.datas.EditData($"NEXT_STEP_{oldStepName};{newStepName}", "NEXT_STEP", $"{oldStepName};{newStepName};{timeStamp1};{timeStamp2};{infos}");
        }
    }

    internal void SaveNextSubStep(string oldSubStepName, string newSubStepName, string oldID, string currentID, string infos = "")
    {
        if (oldSubStepName != newSubStepName)
        {
            GameManager.Instance.datas.AddData($"NEXT_SUBSTEP_{oldSubStepName};{newSubStepName}", "NEXT_SUBSTEP", "");
            TimeSpan totalTime = GameManager.Instance.datas.CalculateTotalTime(currentID);
            string timeStamp2 = totalTime.ToString();
            string timeStamp1;
            if (oldID == "NEXT_SUBSTEP_;")
                timeStamp1 = (totalTime - timeSpanTMP).ToString();
            else
                timeStamp1 = GameManager.Instance.datas.CalculateTimeDifference(oldID, currentID).ToString();
            GameManager.Instance.datas.EditData($"NEXT_SUBSTEP_{oldSubStepName};{newSubStepName}", "NEXT_SUBSTEP", $"{oldSubStepName};{newSubStepName};{timeStamp1};{timeStamp2};{infos}");
            SaveGhostSubStepTime();
        }
    }

    // Actions :
    private void FirstActions()
    {
        LoggerTool.Log("First Actions.");
        datas.InitializeStartTime();
        SaveGhostSubStepTime("Intro");
        DisableUnusedGO();
    }

    private void DisableUnusedGO()
    {
        //holder?.SetActive(false);
        partsFolder?.SetActive(false);
        explodedView?.SetActive(false);
    }

    private void OnAiguillePlacement()
    {
        LoggerTool.Log("Aiguille placée.");
        dialogueSystemStep1?.IncrementStep();
    }

    private void OnProtectionPlacement()
    {
        LoggerTool.Log("Protection placée.");
        dialogueSystemStep1?.IncrementStep();
    }

    private void OnMotherboardPlacement()
    {
        LoggerTool.Log("Carte mère placée.");
        dialogueSystemStep1?.IncrementStep();
    }

    private void OnCoquePlacement()
    {
        LoggerTool.Log("Coque placée.");
        dialogueSystemStep1?.IncrementStep();
        holder?.SetActive(false);
        explodedView?.SetActive(true);
        sciFiDoor1?.TriggerOpen();
    }

    private void OnPhase1()
    {
        LoggerTool.Log("OnPhase1.", LoggerTool.Level.Temporised, 0.5);
    }

    private void OnPhase2()
    {
        LoggerTool.Log("OnPhase2.", LoggerTool.Level.Temporised, 0.5);
    }

    private void OnPhase3()
    {
        LoggerTool.Log("OnPhase3.", LoggerTool.Level.Temporised, 0.5);
        sciFiDoor2?.TriggerOpen();
        sciFiDoor3?.TriggerOpen(); //TODO Remove after part 3 implementation
    }

    private void OnIntroPart1()
    {
        LoggerTool.Log("OnIntroPart1.", LoggerTool.Level.Temporised);
        dialogueSystemStep1?.IncrementStep();
        holder?.SetActive(true);
        partsFolder?.SetActive(true);
    }

    private void OnIntroPart2()
    {
        LoggerTool.Log("OnIntroPart2.", LoggerTool.Level.Temporised);
    }
}
