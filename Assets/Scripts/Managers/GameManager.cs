using UnityEngine;
using System.Collections.Generic;
using System;
using Tools;
using States;
using System.Security.Cryptography;

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
        Carte_Mere,
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
            { MainState.Tuto, OnTuto },
            { MainState.Part1, OnPart1 },
            { MainState.Part2, OnPart2 },
            { MainState.Part3, OnPart3 },
            { MainState.Part4, OnPart4 },
        };

        var part1StateActions = new Dictionary<Part1State, Action>
        {
            { Part1State.Intro, null },
            { Part1State.Aiguille, null },
            { Part1State.Protection, null },
            { Part1State.Carte_Mere, null },
            { Part1State.Coque, null }
        };

        var part1TransitionsActions = new Dictionary<(Part1State, Part1State), Action>
        {
            { (Part1State.Intro, Part1State.Aiguille), OnIntroPart1 },
            { (Part1State.Aiguille, Part1State.Protection), OnAiguillePlacement },
            { (Part1State.Protection, Part1State.Carte_Mere), OnProtectionPlacement },
            { (Part1State.Carte_Mere, Part1State.Coque), OnMotherboardPlacement },
            { (Part1State.Coque, Part1State.Coque), OnCoquePlacement }
        };

        var part2StateActions = new Dictionary<Part2State, Action>
        {
            { Part2State.Intro, OnIntroPart2 },
            { Part2State.Phase1, OnPhase1 },
            { Part2State.Phase2, OnPhase2 },
            { Part2State.Phase3, OnPhase3 }
        };

        var part1StateMachine = new StateMachine<Part1State>(Part1State.Intro, part1StateActions, part1TransitionsActions, onStateExecute: false);
        var part2StateMachine = new StateMachine<Part2State>(Part2State.Intro, part2StateActions, onStateExecute : false);

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

        mainStateMachine = new StateMachine<MainState>(MainState.Tuto, mainStateActions, null, subStateMachines, subStateTypes);
    }

    private void RegisterEvents()
    {
        eventsManager.RegisterEvent("NextState", () => HandleNextState());
    }

    private void HandleNextState()
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

    private void HandleNextStep(string infos = "")
    {
        oldID = $"NEXT_STEP_{oldStepName};{newStepName}";
        oldStepName = GameManager.Instance.mainStateMachine.CurrentStringState();
        newStepName = mainStateMachine.IncrementState().ToString();
        currentID = $"NEXT_STEP_{oldStepName};{newStepName}";
        SaveNextStep(oldStepName, newStepName, oldID, currentID, infos);
    }

    private void HandleNextSubStep(string infos = "")
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
        GameManager.Instance.datas.EditGhostData($"SUBSTEP_{GameManager.Instance.mainStateMachine.GetCurrentSubState()}");
    }

    internal void SaveNextStep(string oldStepName, string newStepName, string oldID, string currentID, string infos = "")
    {
        if (oldStepName != newStepName)
        {
            GameManager.Instance.datas.AddData($"NEXT_STEP_{oldStepName};{newStepName}", "NEXT_STEP", "");
            string timeStamp2 = GameManager.Instance.datas.CalculateTotalTime(currentID).ToString();
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
            Debug.Log($"{oldSubStepName}, {newSubStepName}, {oldID}, {currentID}");
            GameManager.Instance.datas.AddData($"NEXT_SUBSTEP_{oldSubStepName};{newSubStepName}", "NEXT_SUBSTEP", "");
            string timeStamp2 = GameManager.Instance.datas.CalculateTotalTime(currentID).ToString();
            string timeStamp1;
            if (oldID == "NEXT_SUBSTEP_;")
                timeStamp1 = timeStamp2;
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
    }

    private void OnTuto()
    {
        LoggerTool.Log("OnTuto.", LoggerTool.Level.Temporised);
    }

    private void OnPart1()
    {
        LoggerTool.Log("OnPart1.", LoggerTool.Level.Temporised);
    }

    private void OnPart2()
    {
        LoggerTool.Log("OnPart2.", LoggerTool.Level.Temporised);
    }

    private void OnPart3()
    {
        LoggerTool.Log("OnPart3.", LoggerTool.Level.Temporised);
    }

    private void OnPart4()
    {
        LoggerTool.Log("OnPart4.", LoggerTool.Level.Temporised);
    }

    private void OnAiguillePlacement()
    {
        LoggerTool.Log("Aiguille placée.");
    }

    private void OnProtectionPlacement()
    {
        LoggerTool.Log("Protection placée.");
    }

    private void OnMotherboardPlacement()
    {
        LoggerTool.Log("Carte mère placée.");
    }

    private void OnCoquePlacement()
    {
        LoggerTool.Log("Coque placée.");
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
    }

    private void OnIntroPart1()
    {
        LoggerTool.Log("OnIntroPart1.", LoggerTool.Level.Temporised);
    }

    private void OnIntroPart2()
    {
        LoggerTool.Log("OnIntroPart2.", LoggerTool.Level.Temporised);
    }
}
