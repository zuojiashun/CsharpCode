﻿namespace Core.Components
{
    public class ComponentIdUtil
    {
        public static object GetType(int id)
        {
            if (id >= (int)ECoreComponentIds.End)
            {
                return (EComponentIds) id;
            }
            else
            {
                return (ECoreComponentIds) id;
            }
        }
    }

    public enum ECoreComponentIds
    {
        Begin,
        FlagCompensation,
        FlagSyncNonSelf,
        FlagSyncSelf,
        OwnerId,
        EntityId,
        Position,
        EntityAdapter,
        EntityKey,
        FlagDestroy,
        FlagSelf,
        LifeTime,
        Normal,
        Fake,
        GlobalFlag,
        PositionFilter,
        FlagPlayBackFilter,
        FlagImmutability,
        End,

      
    }
    public enum EComponentIds
    {
        Rotation= ECoreComponentIds.End,
        MoveOrientation,
        ViewOrientation,
        PlayerRotateLimit,
        UserCmd,
        VehicleCmd,
        VehicleSeat,
        SendUserCmd,
        Random,
        
        CameraState,
        CameraStateNew,
        CameraOutput,
        CameraStateUpload,

        BagPrimeWeapon,
        BagSubWeapon,
        BagMelee,
        BagPistol,
        BagGrenade,
        BagCurrentWeapon,
        BagBullet,
        BagTactic,
        BagGrenadeInventory,

        BulletData,
        PlayerTime,
        PlayerBasicInfo,
        PlayerMove,

		PlayerMoveUpdate,
        PlayerSkyMove,
        PlayerMoveByAnimUpdate,
        PlayerSkyMoveUpdate,
        PlayerSkyMoveInterVar,
		
        PlayerCameraMotorState,
        PlayerControlledEntity,
        PlayerFsm,
        PlayerFsmBefore,
        PlayerFsmOut,
        PlayerFsmOutBefore,
        PlayerFirePos,
        PlayerFirstPersonAppearance,
        PlayerThirdPersonAppearance,
        PlayerLatestAppearance,
        PlayerPredictedAppearance,
        PlayerFsmMotor,
        PlayerHitbox,
        PlayerGamePlay,
        PlayerWeapon,
        PlayerWeaponLogicInfo,
        PlayerInfo,
        PlayerMeleeAttacker,
        PlayerThrowing,
        PlayerThrowingUpdateData,
        PlayerThrowingSphere,
        PlayerStatisticsData,
        PlayerSound,
        PlayerRecycleableAsset,
        PlayerOxygenEnergy,
        PlayerMask,
        PlayerHitDiagnosis,
        PlayerCast,
        PlayerOverrideBag,
        GenericActionComponent,
        CharacterBone,

        ClientEffectType,
        ClientEffectSubType,
        ClientEffectRotation,
        ClientEffectAttachParent,
        DamageHint,

        VehicleAssetInfo,
        VehicleBrokenFlag,
        CarRewindData,
        CarFirstRewnWheel,
        CarSecondRewnWheel,
        CarThirdRewnWheel,
        CarFourthRewnWheel,
        CarEffect,
        CarFirstWheelEffect,
        CarSecondWheelEffect,
        CarThirdWheelEffect,
        CarFourthWheelEffect,
        CarGameData,    
        CarHitBox,
        ShipDynamicData,
        ShipFirstRudderDynamicData,
        ShipScondRudderDynamicData,
        ShipGameData,
        ShipHixBox,

        EquipmentData,
		AnimatorData,
        AnimatiorServerTime,
        FpAnimData,
        ClientEffectAssets,
        BulletGameObject,
        AppearanceGameObject,
        
        VehicleGameObject,
        DummyObject,

        SceneObjectGameObject,
        SceneObjectFlashGameObject,
        SceneObjectEquip,
        SceneObjectWeapon,
        SceneObjectCastTarget,
        SceneObjectThrowingWeapon,
        SceneObjectMultiGameObjects,
        SceneObjectTeam,
        SceneObjectCastFlag,
        SceneObjectTimeBomb,
        SceneTriggerObject,
        SceneCastTrigger,
        SceneDoorData,
        SceneDestructibleData,
        SceneDestructibleObjectFlag,
        SceneGlassyData,

        SoundUnityObj,

        FreeMoveKey,
        FreeMoveUnityObj,

        WeaponAnimation,

        ThrowingData,
        ThrowingGameObject,
        LocalEvents,
        RemoteEvents,
        Statistics,
        End,
       
    }
}