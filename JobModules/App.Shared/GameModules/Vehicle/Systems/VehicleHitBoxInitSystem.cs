﻿using System.Collections.Generic;
using App.Shared.GameModules.HitBox;
using Utils.AssetManager;
using Core.GameModule.System;
using Core.Utils;
using Entitas;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace App.Shared.GameModules.Vehicle
{
    public class VehicleHitBoxInitSystem : ReactiveResourceLoadSystem<VehicleEntity>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(VehicleHitBoxInitSystem));

        private Contexts _contexts;
        public VehicleHitBoxInitSystem(Contexts contexts) : base(contexts.vehicle)
        {
            _contexts = contexts; 
        }

        protected override ICollector<VehicleEntity> GetTrigger(IContext<VehicleEntity> context)
        {
            return context.CreateCollector(VehicleMatcher.EntityKey.Added(), VehicleMatcher.GameObject.Added(), VehicleMatcher.CarHitBox.Added(), VehicleMatcher.ShipHitBox.Added());
        }

        protected override bool Filter(VehicleEntity entity)
        {
            return !entity.hasHitBox && entity.hasGameObject && entity.HasHitBoxBuffer();
        }

        public override void SingleExecute(VehicleEntity vehicle)
        {
            var name = vehicle.vehicleAssetInfo.AssetBundleName;
            LoadRequestManager.AppendLoadRequest(vehicle, AssetConfig.GetVehicleHitboxAssetInfo(name), OnLoadSucc);
                
            _logger.DebugFormat("created client vehicle hitbox {0}", vehicle.entityKey.Value);
        }

      
       
            public static void OnLoadSucc(object source, UnityObjectWrapper<GameObject> obj)
            {
                VehicleEntity vehicle = (VehicleEntity)source;
                var hitboxConfig = (GameObject)obj;
                var hitboxImposter = HitBoxComponentUtility.InitHitBoxComponent(vehicle.entityKey.Value, vehicle, hitboxConfig);
                vehicle.ConfigHitBoxImposter(hitboxImposter);
            }
        
    }
}
