﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.App.Server.GameModules.GamePlay.Free;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using Free.framework;
using UnityEngine;

namespace App.Shared.FreeFramework.Free.player
{
    [Serializable]
    public class PlayerBattleDataResetAction : AbstractPlayerAction
    {
        public override void DoAction(IEventArgs args)
        {
            PlayerEntity playerEntity = GetPlayerEntity(args);
            if (null != playerEntity)
            {
                playerEntity.statisticsData.Battle.Reset();
                SimpleProto sp = new SimpleProto();
                sp.Key = FreeMessageConstant.ResetBattleData;
                FreeMessageSender.SendMessage(playerEntity, sp);
            }
        }
    }
}
