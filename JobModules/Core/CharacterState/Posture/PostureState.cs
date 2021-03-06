﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.CharacterState.Posture.States;
using Core.CharacterState.Posture.Transitions;
using Core.Fsm;
using UnityEngine;
using Core.Configuration;
using Core.Utils;
using XmlConfig;
using Utils.CharacterState;
using Utils.Configuration;
using Utils.Singleton;

namespace Core.CharacterState.Posture
{
    class PostureState : FsmState
    {
        
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PostureState));
        
        public static PostureState CreateStandState()
        {
            PostureState state = new CustomPostureState(PostureStateId.Stand,
                AnimatorParametersHash.FirstPersonStandCameraHeight,
                AnimatorParametersHash.FirstPersonStandCameraForwardOffset,
                SingletonManager.Get<CharacterStateConfigManager>().GetCharacterControllerCapsule(PostureInConfig.Stand).Height,
                SingletonManager.Get<CharacterStateConfigManager>().GetCharacterControllerCapsule(PostureInConfig.Stand).Radius);

            #region stand to crouch

            state.AddTransition(new PostureTransition(
                    state.AvailableTransitionId(),
                    (command, addOutput) => FsmTransition.SimpleCommandHandler(command, FsmInput.Crouch),
                    (command, addOutput) => FsmTransitionResponseType.NoResponse,
                    PostureStateId.Crouch,
                    SingletonManager.Get<CharacterStateConfigManager>().GetPostureTransitionTime(PostureInConfig.Stand,
                        PostureInConfig.Crouch),
                    AnimatorParametersHash.FirstPersonStandCameraHeight,
                    AnimatorParametersHash.FirstPersonCrouchCameraHeight,
                    AnimatorParametersHash.FirstPersonStandCameraForwardOffset,
                    AnimatorParametersHash.FirstPersonCrouchCameraForwardOffset,
                    AnimatorParametersHash.Instance.StandValue,
                    AnimatorParametersHash.Instance.CrouchValue,
                    SingletonManager.Get<CharacterStateConfigManager>().GetCharacterControllerCapsule(PostureInConfig.Stand),
                    SingletonManager.Get<CharacterStateConfigManager>().GetCharacterControllerCapsule(PostureInConfig.Crouch)),
                new[] {FsmInput.Crouch});

            #endregion

            #region stand to prone

            state.AddTransition(new PostureTransition(
                state.AvailableTransitionId(),
                (command, addOutput) =>
                {
                    var ret = command.IsMatch(FsmInput.Prone);

                    if (ret)
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ProneHash,
                                     AnimatorParametersHash.Instance.ProneName,
                                     AnimatorParametersHash.Instance.ProneEnable,
                                     CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.PostureHash,
                                                 AnimatorParametersHash.Instance.PostureName,
                                                 AnimatorParametersHash.Instance.ProneValue,
                                                 CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;
                    }

                    return ret;
                },
                (command, addOutput) => FsmTransitionResponseType.NoResponse,
                PostureStateId.ProneTransit,
                SingletonManager.Get<CharacterStateConfigManager>().GetPostureTransitionTime(PostureInConfig.Stand,
                        PostureInConfig.Prone)/*0*/,
                AnimatorParametersHash.FirstPersonStandCameraHeight,
                AnimatorParametersHash.FirstPersonProneCameraHeight,
                AnimatorParametersHash.FirstPersonStandCameraForwardOffset,
                AnimatorParametersHash.FirstPersonProneCameraForwardOffset,
                AnimatorParametersHash.Instance.StandValue,
                AnimatorParametersHash.Instance.ProneValue,
                SingletonManager.Get<CharacterStateConfigManager>().GetCharacterControllerCapsule(PostureInConfig.Stand),
                SingletonManager.Get<CharacterStateConfigManager>().GetCharacterControllerCapsule(PostureInConfig.Prone), false),
                new[] { FsmInput.Prone });

            #endregion

            #region stand to jumpstart

            state.AddTransition(
                (command, addOutput) =>
                {
                    var ret = command.IsMatch(FsmInput.Jump);

                    if (ret)
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.JumpStartHash,
                                                 AnimatorParametersHash.Instance.JumpStartName,
                                                 AnimatorParametersHash.Instance.JumpStartEnable,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;
                    }

                    return ret;
                },
                (command, addOutput) => FsmTransitionResponseType.NoResponse,
                (int) PostureStateId.JumpStart, null, 0, new[] { FsmInput.Jump });

            #endregion

            #region stand to freefall 

            state.AddTransition(
                (command, addOutput) =>
                {
                    var ret = command.IsMatch(FsmInput.Freefall);

                    if (ret)
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.FreeFallHash,
                                                 AnimatorParametersHash.Instance.FreeFallName,
                                                 AnimatorParametersHash.Instance.FreeFallEnable,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);
                        Logger.InfoFormat("stand to freefall transition, set jumploop to true!");
                        command.Handled = true;
                    }

                    return ret;
                },
                null, (int)PostureStateId.Freefall, null, 0, new[] { FsmInput.Freefall });

            #endregion

            #region stand to swim

            state.AddTransition(
                (command, addOutput) =>
                {
                    var ret = command.IsMatch(FsmInput.Swim);

                    if (ret)
                    {
                        FsmOutput.Cache.SetLayerWeight(AnimatorParametersHash.Instance.SwimLayer,
                                                       AnimatorParametersHash.Instance.SwimEnableValue,
                                                       CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.SwimStateHash,
                                                 AnimatorParametersHash.Instance.SwimStateName,
                                                 AnimatorParametersHash.Instance.SwimStateSwimValue,
                                                 CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;
                    }

                    return ret;
                },
                (command, addOutput) => FsmTransitionResponseType.NoResponse,
                (int)PostureStateId.Swim, null, 0, new[] { FsmInput.Swim });

            #endregion

            #region stand to dying

            AddTransitionToDying(state);

            #endregion

            #region stand to climb

            state.AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.Climb))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ClimbHash,
                                                 AnimatorParametersHash.Instance.ClimbName,
                                                 AnimatorParametersHash.Instance.ClimbEnableValue,
                                                 CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ClimbStateHash,
                                                 AnimatorParametersHash.Instance.ClimbStateName,
                                                 command.AdditioanlValue,
                                                 CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);
                        command.Handled = true;
                        return true;
                    }

                    return false;
                },
                (command, addOutput) => FsmTransitionResponseType.NoResponse,
                (int)PostureStateId.Climb, null, 0, new[] { FsmInput.Climb });

            #endregion

            return state;
        }

        public static PostureState CreateCrouchState()
        {
            PostureState state = new CustomPostureState(PostureStateId.Crouch,
                AnimatorParametersHash.FirstPersonCrouchCameraHeight,
                AnimatorParametersHash.FirstPersonCrouchCameraForwardOffset,
                SingletonManager.Get<CharacterStateConfigManager>().GetCharacterControllerCapsule(PostureInConfig.Crouch).Height,
                SingletonManager.Get<CharacterStateConfigManager>().GetCharacterControllerCapsule(PostureInConfig.Crouch).Radius);
            
            #region crouch to stand

            state.AddTransition(new PostureTransition(
                state.AvailableTransitionId(),
                (command, addOutput) => FsmTransition.SimpleCommandHandler(command, FsmInput.Crouch) || FsmTransition.SimpleCommandHandler(command, FsmInput.Jump),
                (command, addOutput) => FsmTransitionResponseType.NoResponse,
                PostureStateId.Stand,
                SingletonManager.Get<CharacterStateConfigManager>().GetPostureTransitionTime(PostureInConfig.Crouch, PostureInConfig.Stand),
                AnimatorParametersHash.FirstPersonCrouchCameraHeight,
                AnimatorParametersHash.FirstPersonStandCameraHeight,
                AnimatorParametersHash.FirstPersonCrouchCameraForwardOffset,
                AnimatorParametersHash.FirstPersonStandCameraForwardOffset,
                AnimatorParametersHash.Instance.CrouchValue,
                AnimatorParametersHash.Instance.StandValue,
                SingletonManager.Get<CharacterStateConfigManager>().GetCharacterControllerCapsule(PostureInConfig.Crouch),
                SingletonManager.Get<CharacterStateConfigManager>().GetCharacterControllerCapsule(PostureInConfig.Stand)),
                new[] { FsmInput.Crouch, FsmInput.Jump });

            #endregion

            #region crouch to prone

            state.AddTransition(new PostureTransition(
                state.AvailableTransitionId(),
                (command, addOutput) =>
                {
                    var ret = command.IsMatch(FsmInput.Prone);

                    if (ret)
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ProneHash,
                                     AnimatorParametersHash.Instance.ProneName,
                                     AnimatorParametersHash.Instance.ProneEnable,
                                     CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.PostureHash,
                                                 AnimatorParametersHash.Instance.PostureName,
                                                 AnimatorParametersHash.Instance.ProneValue,
                                                 CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;
                    }

                    return ret;
                },
                (command, addOutput) => FsmTransitionResponseType.NoResponse,
                PostureStateId.ProneTransit,
                SingletonManager.Get<CharacterStateConfigManager>().GetPostureTransitionTime(PostureInConfig.Crouch,
                        PostureInConfig.Prone)/*0*/,
                AnimatorParametersHash.FirstPersonCrouchCameraHeight,
                AnimatorParametersHash.FirstPersonProneCameraHeight,
                AnimatorParametersHash.FirstPersonCrouchCameraForwardOffset,
                AnimatorParametersHash.FirstPersonProneCameraForwardOffset,
                AnimatorParametersHash.Instance.CrouchValue,
                AnimatorParametersHash.Instance.ProneValue,
                SingletonManager.Get<CharacterStateConfigManager>().GetCharacterControllerCapsule(PostureInConfig.Crouch),
                SingletonManager.Get<CharacterStateConfigManager>().GetCharacterControllerCapsule(PostureInConfig.Prone), false),
                new[] { FsmInput.Prone });

            #endregion

            #region crouch to freefall

            state.AddTransition(
                (command, addOutput) =>
                {
                    var ret = command.IsMatch(FsmInput.Freefall);

                    if (ret)
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.FreeFallHash,
                                                 AnimatorParametersHash.Instance.FreeFallName,
                                                 AnimatorParametersHash.Instance.FreeFallEnable,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;
                    }

                    return ret;
                },
                null, (int)PostureStateId.Freefall, null, 0, new[] { FsmInput.Freefall });

            #endregion

            #region crouch to dying

            AddTransitionToDying(state);

            #endregion

            return state;
        }

        public static PostureState CreateProneState()
        {
            PostureState state = new CustomPostureState(PostureStateId.Prone,
                AnimatorParametersHash.FirstPersonProneCameraHeight,
                AnimatorParametersHash.FirstPersonProneCameraForwardOffset,
                SingletonManager.Get<CharacterStateConfigManager>().GetCharacterControllerCapsule(PostureInConfig.Prone).Height,
                SingletonManager.Get<CharacterStateConfigManager>().GetCharacterControllerCapsule(PostureInConfig.Prone).Radius);
            
            #region prone to crouch

            state.AddTransition(new PostureTransition(
                state.AvailableTransitionId(),
                (command, addOutput) =>
                {
                    var ret = command.IsMatch(FsmInput.Crouch);

                    if (ret)
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ProneHash,
                                     AnimatorParametersHash.Instance.ProneName,
                                     AnimatorParametersHash.Instance.ProneDisable,
                                     CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.PostureHash,
                                                 AnimatorParametersHash.Instance.PostureName,
                                                 AnimatorParametersHash.Instance.CrouchValue,
                                                 CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;
                    }

                    return ret;
                },
                (command, addOutput) => FsmTransitionResponseType.NoResponse,
                PostureStateId.ProneToCrouch,
                SingletonManager.Get<CharacterStateConfigManager>().GetPostureTransitionTime(PostureInConfig.Prone,
                        PostureInConfig.Crouch)/*0*/,
                AnimatorParametersHash.FirstPersonProneCameraHeight,
                AnimatorParametersHash.FirstPersonCrouchCameraHeight,
                AnimatorParametersHash.FirstPersonProneCameraForwardOffset,
                AnimatorParametersHash.FirstPersonCrouchCameraForwardOffset,
                AnimatorParametersHash.Instance.ProneValue,
                AnimatorParametersHash.Instance.CrouchValue,
                SingletonManager.Get<CharacterStateConfigManager>().GetCharacterControllerCapsule(PostureInConfig.Prone),
                SingletonManager.Get<CharacterStateConfigManager>().GetCharacterControllerCapsule(PostureInConfig.Crouch), false),
                new[] { FsmInput.Crouch });

            #endregion

            #region prone to stand

            state.AddTransition(new PostureTransition(
                state.AvailableTransitionId(),
                (command, addOutput) =>
                {
                    var ret = command.IsMatch(FsmInput.Jump) || command.IsMatch(FsmInput.Prone);

                    if (ret)
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ProneHash,
                                     AnimatorParametersHash.Instance.ProneName,
                                     AnimatorParametersHash.Instance.ProneDisable,
                                     CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.PostureHash,
                                                 AnimatorParametersHash.Instance.PostureName,
                                                 AnimatorParametersHash.Instance.StandValue,
                                                 CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;
                        
                        Logger.InfoFormat("prone to stand!!!! handle");
                    }

                    return ret;
                },
                (command, addOutput) => FsmTransitionResponseType.NoResponse,
                PostureStateId.ProneToStand,
                SingletonManager.Get<CharacterStateConfigManager>().GetPostureTransitionTime(PostureInConfig.Prone,
                        PostureInConfig.Stand)/*0*/,
                AnimatorParametersHash.FirstPersonProneCameraHeight,
                AnimatorParametersHash.FirstPersonStandCameraHeight,
                AnimatorParametersHash.FirstPersonProneCameraForwardOffset,
                AnimatorParametersHash.FirstPersonStandCameraForwardOffset,
                AnimatorParametersHash.Instance.ProneValue,
                AnimatorParametersHash.Instance.StandValue,
                SingletonManager.Get<CharacterStateConfigManager>().GetCharacterControllerCapsule(PostureInConfig.Prone),
                SingletonManager.Get<CharacterStateConfigManager>().GetCharacterControllerCapsule(PostureInConfig.Stand), false),
                new[] { FsmInput.Jump, FsmInput.Prone });

            #endregion

            #region prone to freefall

            state.AddTransition(
                (command, addOutput) =>
                {
                    var ret = command.IsMatch(FsmInput.Freefall);

                    if (ret)
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.FreeFallHash,
                                                 AnimatorParametersHash.Instance.FreeFallName,
                                                 AnimatorParametersHash.Instance.FreeFallEnable,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ProneHash,
                                                 AnimatorParametersHash.Instance.ProneName,
                                                 AnimatorParametersHash.Instance.ProneDisable,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;
                    }

                    return ret;
                },
                null, (int) PostureStateId.Freefall, null, 0, new[] { FsmInput.Freefall });

            #endregion

            #region prone to dying

            AddTransitionToDying(state);

            #endregion

            return state;
        }

        public static PostureState CreateSwimState()
        {
            PostureState state = new PostureState(PostureStateId.Swim);

            #region swim to stand

            AddTransitionFromWaterToStand(state);

            #endregion

            #region swim to dive

            state.AddTransition(new DiveTransition(
                state.AvailableTransitionId(), 
                (int)PostureStateId.Dive,
                SingletonManager.Get<CharacterStateConfigManager>().GetPostureTransitionTime(PostureInConfig.Swim, PostureInConfig.Dive), 
                AnimatorParametersHash.Instance.SwimStateSwimValue, 
                AnimatorParametersHash.Instance.SwimStateDiveValue,
                FsmInput.Dive), 
                new[] { FsmInput.Dive });

            #endregion

            return state;
        }

        public static PostureState CreateDiveState()
        {
            PostureState state = new DiveState(PostureStateId.Dive);

            #region dive to swim


            state.AddTransition(new DiveTransition(
                state.AvailableTransitionId(),
                (int)PostureStateId.Swim,
                SingletonManager.Get<CharacterStateConfigManager>().GetPostureTransitionTime(PostureInConfig.Dive, PostureInConfig.Swim),
                AnimatorParametersHash.Instance.SwimStateDiveValue, 
                AnimatorParametersHash.Instance.SwimStateSwimValue,
                FsmInput.Swim),
                new[] { FsmInput.Swim });

            #endregion

            #region dive to stand

            AddTransitionFromWaterToStand(state);

            #endregion

            return state;
        }

        public static PostureState CreateDyingState()
        {
            PostureState state = new PostureState(PostureStateId.Dying);

            #region dying to crouch

            state.AddTransition(
                (command, addOutput) =>
                {
                    var ret = command.IsMatch(FsmInput.Revive);

                    if (ret)
                    {
                        FsmOutput.Cache.SetLayerWeight(AnimatorParametersHash.Instance.DyingLayer,
                                                       AnimatorParametersHash.Instance.DyingDisableValue,
                                                       CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.PostureHash,
                                                 AnimatorParametersHash.Instance.PostureName,
                                                 AnimatorParametersHash.Instance.CrouchValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;
                    }

                    return ret;
                },
                (command, addOutput) => FsmTransitionResponseType.NoResponse,
                (int) PostureStateId.Crouch, null, 0, new[] { FsmInput.Revive });

            #endregion

            return state;
        }

        public static PostureState CreateJumpStartState()
        {
            PostureState state = new JumpStartState(PostureStateId.JumpStart);

            return state;
        }

        public static PostureState CreateClimbState()
        {
            return new ClimbState(PostureStateId.Climb);
        }

        public static PostureState CreateProneTransitState()
        {
            return new ProneTransitState(PostureStateId.ProneTransit);
        }

        public static PostureState CreateProneToStandState()
        {
            return new ProneToStandState(PostureStateId.ProneToStand);
        }

        public static PostureState CreateProneToCrouchState()
        {
            return new ProneToCrouchState(PostureStateId.ProneToCrouch);
        }

        public static PostureState CreateFreefallState()
        {
            return new FreefallState(PostureStateId.Freefall);
        }

        public static PostureState CreateJumpEndState()
        {
            return new JumpEndState(PostureStateId.JumpEnd);
        }

        public static PostureState CreateNoPeekState()
        {
            PostureState state = new PostureState(PostureStateId.NoPeek);

            #region NoPeek to PeekLeft

            state.AddTransition(new NoPeekToPeekLeftTransition(state.AvailableTransitionId(),
                                                               (int)PostureStateId.PeekLeft,
                                                               AnimatorParametersHash.PeekTime),
                                new[] { FsmInput.PeekLeft });

            #endregion

            #region NoPeek to PeekRight

            state.AddTransition(new NoPeekToPeekRightTransition(state.AvailableTransitionId(),
                                                                (int) PostureStateId.PeekRight,
                                                                AnimatorParametersHash.PeekTime),
                                new[] { FsmInput.PeekRight });

            #endregion

            return state;
        }

        public static PostureState CreatePeekLeftState()
        {
            PostureState state = new PeekLeftState(PostureStateId.PeekLeft);

            return state;
        }

        public static PostureState CreatePeekRightState()
        {
            PostureState state = new PeekRightState(PostureStateId.PeekRight);

            return state;
        }

        private static void AddTransitionToDying(PostureState state)
        {
            state.AddTransition(
                (command, addOutput) =>
                {
                    var ret = command.IsMatch(FsmInput.Dying);

                    if (ret)
                    {
                        FsmOutput.Cache.SetLayerWeight(AnimatorParametersHash.Instance.DyingLayer,
                                                       AnimatorParametersHash.Instance.DyingEnableValue,
                                                       CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ProneHash,
                                                 AnimatorParametersHash.Instance.ProneName,
                                                 AnimatorParametersHash.Instance.ProneDisable,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;
                    }

                    return ret;
                },
                null, (int)PostureStateId.Dying, null, 0, new[] { FsmInput.Dying });
        }

        private static void AddTransitionFromWaterToStand(PostureState state)
        {
            state.AddTransition(
                (command, addOutput) =>
                {
                    var ret = command.IsMatch(FsmInput.Ashore);

                    if (ret)
                    {
                        FsmOutput.Cache.SetLayerWeight(AnimatorParametersHash.Instance.SwimLayer,
                                                       AnimatorParametersHash.Instance.SwimDisableValue,
                                                       CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.PostureHash,
                                                 AnimatorParametersHash.Instance.PostureName,
                                                 AnimatorParametersHash.Instance.StandValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(FsmOutputType.FirstPersonHeight,
                                                 AnimatorParametersHash.FirstPersonStandCameraHeight);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(FsmOutputType.FirstPersonForwardOffset,
                            AnimatorParametersHash.FirstPersonStandCameraForwardOffset);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;
                    }

                    return ret;
                },
                (command, addOutput) => FsmTransitionResponseType.NoResponse,
                (int)PostureStateId.Stand, null, 0, new[] { FsmInput.Ashore });
        }

        protected static void AddTransitionFromJumpToDying(PostureState state)
        {
            state.AddTransition(
                (command, addOutput) =>
                {
                    var ret = command.IsMatch(FsmInput.Dying);

                    if (ret)
                    {
                        command.Handled = true;

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.FreeFallHash,
                                                 AnimatorParametersHash.Instance.FreeFallName,
                                                 AnimatorParametersHash.Instance.FreeFallDisable,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.JumpStartHash,
                                                 AnimatorParametersHash.Instance.JumpStartName,
                                                 AnimatorParametersHash.Instance.JumpStartDisable,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetLayerWeight(AnimatorParametersHash.Instance.DyingLayer,
                                                       AnimatorParametersHash.Instance.DyingEnableValue,
                                                       CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);
                    }

                    return ret;
                },
                null, (int)PostureStateId.Dying, null, 0, new[] { FsmInput.Dying });
        }

        public PostureState(PostureStateId id) : base((short)id)
        {
        }
    }
}
