using UnityEngine;
using System.Collections.Generic;
using System;
using Tools;
using States;

public class GameManager : MonoBehaviour
{
    private enum MainState
    {
        Tuto,
        Part1,
        Part2,
        Part3,
        Part4
    }

    private enum Part1State
    {
        Intro,
        Aiguille,
        Protection,
        Carte_Mere,
        Coque
    }

    private enum Part2State
    {
        Intro,
        Phase1,
        Phase2,
        Phase3
    }

    private StateMachine<MainState> mainStateMachine;
    private EventsManager eventsManager;

    void Start()
    {
        eventsManager = gameObject.AddComponent<EventsManager>();

        InitializeStateMachine();
        RegisterEvents();

        LoggerTool.Log("Initialized.");
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

        var part1StateMachine = new StateMachine<Part1State>(Part1State.Intro, part1StateActions, part1TransitionsActions);
        var part2StateMachine = new StateMachine<Part2State>(Part2State.Intro, part2StateActions);

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
        if (mainStateMachine.IsLastSubState()) mainStateMachine.IncrementState();
        else mainStateMachine.IncrementSubState();
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
