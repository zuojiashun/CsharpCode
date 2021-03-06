﻿using App.Shared.Components.Player;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Core.Animation;
using Utils.Appearance;
using Core.Appearance;
using App.Shared.GameModules.Player.CharacterState;
using Core.CharacterState;
using UnityEngine;
using XmlConfig;
using App.Shared.Player;
using App.Shared.GameModules.Player.CharacterBone;
using Core.CharacterBone;

namespace App.Shared.GameModules.Player.Appearance
{
    public class PlayerAppearanceUpdateSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerAppearanceUpdateSystem));

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity player = owner.OwnerEntity as PlayerEntity;
            if (player.gamePlay.IsLifeState(EPlayerLifeState.Dead))
            {
                return;
            }
            AppearanceUpdate(player);
        }

        private void AppearanceUpdate(PlayerEntity player)
        {
            var appearance = player.appearanceInterface.Appearance;

            appearance.SyncFrom(player.predictedAppearance);
            appearance.SyncFrom(player.latestAppearance);
            appearance.TryRewind();

            appearance.Execute();

            // first person only
            appearance.SyncTo(player.predictedAppearance);
            appearance.SyncTo(player.latestAppearance);
        }
    }
}
