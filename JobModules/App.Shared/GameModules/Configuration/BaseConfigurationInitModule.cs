﻿using App.Shared.Configuration;
using Assets.Core.Configuration;
using Assets.Utils.Configuration;
using Core.Configuration;
using Core.Configuration.Sound;
using Core.Configuration.Terrains;
using Core.GameModule.Module;
using Core.SessionState;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared.GameModules.Configuration
{
    public class BaseConfigurationInitModule : GameModule
    {
        public bool _isServer;

        public BaseConfigurationInitModule(Contexts context, ISessionState sessionState, bool IsServer)
        {
            _isServer = IsServer;


            AddConfigSystem<AssetConfigManager>(sessionState, "svn.version");
            AddConfigSystem<CharacterStateConfigManager>(sessionState, "SpeedConfig", new CharacterSpeedSubResourceHandler(context.session.commonSession.LoadRequestManager)
                .LoadSubResources);
            AddConfigSystem<AvatarAssetConfigManager>(sessionState, "role_avator_res");

            AddConfigSystem<FirstPersonOffsetConfigManager>(sessionState, "FirstPersonOffset");
            AddConfigSystem<RoleConfigManager>(sessionState, "role");
            AddConfigSystem<KillFeedBackConfigManager>(sessionState, "killfeedback");
            AddConfigSystem<CameraConfigManager>(sessionState, "NewCamera");
            AddConfigSystem<SoundConfigManager>(sessionState, "Sound");
            AddConfigSystem<PlayerSoundConfigManager>(sessionState, "PlayerSound");
            AddConfigSystem<BulletDropConfigManager>(sessionState, "BulletDrop");
            AddConfigSystem<ClientEffectCommonConfigManager>(sessionState, "ClientEffectCommon");
            AddConfigSystem<WeaponDataConfigManager>(sessionState, "WeaponData");
            AddConfigSystem<WeaponConfigManager>(sessionState, "weapon");
            AddConfigSystem<ClipDropConfigManager>(sessionState, "ClipDrop");
            AddConfigSystem<WeaponPartsConfigManager>(sessionState, "weapon_parts");
            AddConfigSystem<MapPositionConfigManager>(sessionState, "temp");
            AddConfigSystem<WeaponPartSurvivalConfigManager>(sessionState, "weapon_parts_survival");
            AddConfigSystem<GameItemConfigManager>(sessionState, "gameitem");
            AddConfigSystem<RoleAvatarConfigManager>(sessionState, "role_avator");
            AddConfigSystem<CardConfigManager>(sessionState, "card");
            AddConfigSystem<TypeForDeathConfigManager>(sessionState, "TypeForDeath");
            AddConfigSystem<ChatConfigManager>(sessionState, "chat");

            AddConfigSystem<TerrainSoundConfigManager>(sessionState, "TerrainSound");
            AddConfigSystem<TerrainEffectConfigManager>(sessionState, "TerrainEffect");
            AddConfigSystem<TerrainMaterialConfigManager>(sessionState, "TerrainMaterial");
            AddConfigSystem<TerrainTextureConfigManager>(sessionState, "TerrainTexture");
            AddConfigSystem<TerrainVehicleFrictionConfigManager>(sessionState, "TerrainFriction");
            AddConfigSystem<TerrainTextureTypeConfigManager>(sessionState, "TerrainTextureType");

            AddConfigSystem<DynamicPredictionErrorCorrectionConfigManager>(sessionState,
                "DynamicPredictionErrorCorrectionConfig");
            AddConfigSystem<VehicleAssetConfigManager>(sessionState, "VehicleConfig");
            AddConfigSystem<VehicleSoundConfigManager>(sessionState, "VehicleSound");
            AddConfigSystem<StateTransitionConfigManager>(sessionState, "StateTransition");
            AddConfigSystem<RaycastActionConfigManager>(sessionState, "RaycastAction");
            AddConfigSystem<LadderRankConfigManager>(sessionState, "ladderrank");

            AddConfigSystem<WeaponPropertyConfigManager>(sessionState, "weapon_property");
            AddConfigSystem<PropConfigManager>(sessionState, "prop");
            AddConfigSystem<EnvironmentTypeConfigManager>(sessionState, "EnvironmentType");
            AddConfigSystem<ClientEffectConfigManager>(sessionState, "ClientEffect");
            AddConfigSystem<GameModeConfigManager>(sessionState, "gamemode");

            AddConfigSystem<WeaponAvatarConfigManager>(sessionState, "weapon_avator",
                new WeaponAvatarAnimSubResourceHandler(context.session.commonSession.LoadRequestManager)
                    .LoadSubResources);
            //AddConfigSystem<StreamingLevelStructure>(sessionState, "streaminglevel");
            AddConfigSystem<MapsDescription>(sessionState, "mapConfig");
        }

        private void AddConfigSystem<T>(ISessionState sessionState, string asset,
            SubReousourcesHandler subResourceHandler = null) where T : AbstractConfigManager<T>, IConfigParser, new()
        {
            SingletonManager.Get<T>().IsServer = _isServer;
            AddSystem(new DefaultConfigInitSystem<T>(sessionState, new AssetInfo("tables", asset),
                SingletonManager.Get<T>(), subResourceHandler));
        }
    }
}