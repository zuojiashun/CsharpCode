﻿using Core.Compare;
using Core.Fsm;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.CharacterState;
using UnityEngine;
using Utils.Appearance;
using Utils.Utils;
using XmlConfig;
using App.Shared.Player;
using Core.CharacterState.Posture;

namespace App.Shared.GameModules.Player.CharacterState
{
    public class FsmInputCreator
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(FsmInputCreator));

        private static readonly Dictionary<FsmInput, List<PostureInConfig>> FilterFsmInputByStateDict = new Dictionary<FsmInput, List<PostureInConfig>>(FsmInputEqualityComparer.Instance)
        {
            {FsmInput.Up, new List<PostureInConfig>{ PostureInConfig.Dive} },
            {FsmInput.Down, new List<PostureInConfig>{PostureInConfig.Dive, PostureInConfig.Swim} },
            {FsmInput.DiveMove, new List<PostureInConfig>{PostureInConfig.Dive} },
        };

        private static readonly int InitCommandLen = 5;

        private List<IFsmInputFilter> _filters = new List<IFsmInputFilter>
        {
            new ProneStateFilter(), new DiveStateFilter()
        };

        private FsmInputContainer _commandsContainer = new FsmInputContainer(InitCommandLen);

        public FsmInputCreator()
        {

        }

        public IAdaptiveContainer<IFsmInputCommand> CommandsContainer { get { return _commandsContainer; } }

        public void CreateCommands(IUserCmd cmd, FilterState state, PlayerEntity player) //,int curLeanState,int leanTimeCount)
        {
            PretreatCmd(player, cmd);
            FromUserCmdToFsmInput(cmd, player);
            foreach (var v in _filters)
            {
                TryFilter(v, state);
            }

            BlockFilter(state);
        }

        /// <summary>
        /// 一些命令只有在特定的状态下使用
        /// </summary>
        /// <param name="state"></param>
        private void BlockFilter(FilterState state)
        {
            for (int i = 0; i < CommandsContainer.Length; ++i)
            {
                if (FilterFsmInputByStateDict.ContainsKey(CommandsContainer[i].Type) && !FilterFsmInputByStateDict[CommandsContainer[i].Type].Contains(state.Posture, CommonEnumEqualityComparer<PostureInConfig>.Instance))
                {
                    CommandsContainer[i].Reset();
                }
            }
        }

        public void Reset()
        {
            _commandsContainer.Reset();
        }

        private void PretreatCmd(PlayerEntity player, IUserCmd cmd)
        {
            var actionState = player.stateInterface.State.GetActionState();
            var curPostureState = player.stateInterface.State.GetCurrentPostureState();
            var nextPostureState = player.stateInterface.State.GetNextPostureState();

            if (cmd.MoveHorizontal != 0 ||
                cmd.MoveVertical != 0)
                player.playerMove.InterruptAutoRun();
            else if ((cmd.IsPeekRight || cmd.IsPeekLeft) &&
                     curPostureState != PostureInConfig.Prone &&
                     curPostureState != PostureInConfig.Swim &&
                     curPostureState != PostureInConfig.Dive)
                player.playerMove.InterruptAutoRun();
            else if (curPostureState != nextPostureState &&
                    nextPostureState != PostureInConfig.Dive &&
                    nextPostureState != PostureInConfig.Swim &&
                    nextPostureState != PostureInConfig.Land &&
                    nextPostureState != PostureInConfig.Jump &&
                    nextPostureState != PostureInConfig.Stand)
                player.playerMove.InterruptAutoRun();
            else
            {
                switch (actionState)
                {
                    case ActionInConfig.MeleeAttack:
                    case ActionInConfig.SwitchWeapon:
                    case ActionInConfig.PickUp:
                    case ActionInConfig.Reload:
                    case ActionInConfig.SpecialReload:
                        player.playerMove.InterruptAutoRun();
                        break;
                    default: break;
                }
            }

            if (cmd.IsSwitchAutoRun)
            {
                player.playerMove.IsAutoRun = !player.playerMove.IsAutoRun;
                if (player.playerMove.IsAutoRun)
                {
                    PlayerStateUtil.AddPlayerState(EPlayerGameState.InterruptItem, player.gamePlay);
                }
            }

        }

        private void FromUserCmdToFsmInput(IUserCmd cmd, PlayerEntity player)
        {
            // 根据WSAD生成FsmInput
            if (CompareUtility.IsApproximatelyEqual(cmd.MoveHorizontal, 0) 
                && CompareUtility.IsApproximatelyEqual(cmd.MoveVertical, 0)
                && !player.playerMove.IsAutoRun)
            {
                // WSAD均未按下
                SetCommand(FsmInput.Idle);
            }
            else
            {
                if (!CompareUtility.IsApproximatelyEqual(cmd.MoveHorizontal, 0))
                {
                    SetCommand(cmd.MoveHorizontal > 0 ? FsmInput.Right : FsmInput.Left, cmd.MoveHorizontal);
                }
                if (!CompareUtility.IsApproximatelyEqual(cmd.MoveVertical, 0))
                {
                    SetCommand(cmd.MoveVertical > 0 ? FsmInput.Forth : FsmInput.Back, cmd.MoveVertical);
                }
                if(player.playerMove.IsAutoRun)
                {
                    SetCommand(FsmInput.Forth, InputValueLimit.MaxAxisValue);
                }
                // 冲刺
                if ((cmd.IsRun || player.playerMove.IsAutoRun) && IsCanSprint(cmd))
                {
                    if(player.playerMove.IsAutoRun)
                    {
                        SetCommand(FsmInput.Sprint);
                    }
                    // 冲刺只有前向90度
                    else if (cmd.MoveVertical > 0 && cmd.MoveVertical >= Math.Abs(cmd.MoveHorizontal))
                    {
                        SetCommand(FsmInput.Sprint);
                    }
                    else
                    {
                        SetCommand(FsmInput.Run);
                    }
                }
                // 静走 静走不被限制
                else if (cmd.FilteredInput.IsInput(EPlayerInput.IsSlightWalk))
                {
                    SetCommand(FsmInput.Walk);
                }
                else if (IsCanRun(cmd))
                {
                    SetCommand(FsmInput.Run);
                }
                //不能冲刺跑步，就切换为静走
                else if(!cmd.FilteredInput.IsInputBlocked(EPlayerInput.IsSlightWalk))
                {
                    SetCommand(FsmInput.Walk);
                }
            }

            if (!CompareUtility.IsApproximatelyEqual(cmd.MoveUpDown, 0))
            {
                SetCommand(cmd.MoveUpDown > 0 ? FsmInput.Up : FsmInput.Down, cmd.MoveUpDown);
            }

            if (CompareUtility.IsApproximatelyEqual(cmd.MoveHorizontal, 0) &&
                CompareUtility.IsApproximatelyEqual(cmd.MoveVertical, 0) &&
                CompareUtility.IsApproximatelyEqual(cmd.MoveUpDown, 0))
            {
                SetCommand(FsmInput.DiveIdle);
            }
            else
            {
                SetCommand(FsmInput.DiveMove);

            }

            if (cmd.IsPeekLeft && !cmd.IsPeekRight && cmd.FilteredInput.IsInput(XmlConfig.EPlayerInput.IsPeekLeft))
            {
                CheckConditionAndSetCommand(cmd, XmlConfig.EPlayerInput.IsPeekLeft, FsmInput.PeekLeft);
            }
            else if (cmd.IsPeekRight && !cmd.IsPeekLeft && cmd.FilteredInput.IsInput(XmlConfig.EPlayerInput.IsPeekRight))
            {
                CheckConditionAndSetCommand(cmd, XmlConfig.EPlayerInput.IsPeekRight, FsmInput.PeekRight);
            }
            else
            {
                SetCommand(FsmInput.NoPeek);
            }

            if (cmd.IsJump)
            {
				CheckConditionAndSetCommand(cmd, XmlConfig.EPlayerInput.IsJump, FsmInput.Jump);
            }

            if (cmd.IsCrouch)
            {
                CheckConditionAndSetCommand(cmd, XmlConfig.EPlayerInput.IsCrouch, FsmInput.Crouch);
            }
            
            if (cmd.IsProne)
            {
                CheckConditionAndSetCommand(cmd, XmlConfig.EPlayerInput.IsProne, FsmInput.Prone);
            }
        }

        private bool IsCanSprint(IUserCmd cmd)
        {
            return !cmd.FilteredInput.IsInputBlocked(EPlayerInput.IsSprint);
        }

        private bool IsCanRun(IUserCmd cmd)
        {
            return !cmd.FilteredInput.IsInputBlocked(EPlayerInput.IsRun);
        }

        private void CheckConditionAndSetCommand(IUserCmd cmd, XmlConfig.EPlayerInput mappedInput, FsmInput fsmInput)
        {
            if (null != cmd.FilteredInput && cmd.FilteredInput.IsInput(mappedInput))
            {
                SetCommand(fsmInput);
            }
            else
            {
                if(null == cmd.FilteredInput)
                {
                    Logger.Error("FilteredInput in cmd should never be null !");
                }
            }
        }

        private void SetCommand(FsmInput type, float value = 0f)
        {
            var command = CommandsContainer.GetAvailableItem();
            command.Type = type;
            command.AdditioanlValue = value;
        }

        private void TryFilter(IFsmInputFilter filter, FilterState state)
        {
            filter.SetCurrentState(state);
            if (filter.Active)
            {
                filter.Filter(CommandsContainer);
            }
        }
    }
}
