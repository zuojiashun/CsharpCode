﻿using Core.Utils;
using App.Shared.Components.Bag;
using Core.Bag;
using XmlConfig;
using Utils.Configuration;
using Utils.Appearance;
using Assets.XmlConfig;
using Utils.Singleton;
using App.Shared.WeaponLogic;

namespace App.Shared
{
    public static class BagUtility
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(BagUtility));

        public static int GetRealAttachmentId(int attachId, int weaponId)
        {
            var cfg = SingletonManager.Get<WeaponPartSurvivalConfigManager>().GetConfigById(attachId);
            if (null == cfg)
            {
                return 0;
            }
            for (int i = 0; i < cfg.PartsList.Length; i++)
            {
                if (SingletonManager.Get<WeaponPartsConfigManager>().IsPartMatchWeapon(cfg.PartsList[i], weaponId))
                {
                    return cfg.PartsList[i];
                }
            }
            return 0;
        }

        /// <summary>
        /// 该槽位的武器是否可能有配件
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public static bool MayHasPart(this EWeaponSlotType slot)
        {
            switch(slot)
            {
                case EWeaponSlotType.PrimeWeapon1:
                case EWeaponSlotType.PrimeWeapon2:
                case EWeaponSlotType.SubWeapon:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 该槽位的武器使用的时候是否引起数据变化 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static bool IsSlotChangeByCost(this EWeaponSlotType slot)
        {
            switch(slot)
            {
                default:
                case EWeaponSlotType.PrimeWeapon1:
                case EWeaponSlotType.PrimeWeapon2:
                case EWeaponSlotType.SubWeapon:
                case EWeaponSlotType.TacticWeapon:
                case EWeaponSlotType.GrenadeWeapon:
                    return true;
                case EWeaponSlotType.MeleeWeapon:
                    return false;
            }
        }

        public static bool IsSlotWithBullet(this EWeaponSlotType slot)
        {
            switch (slot)
            {
                case EWeaponSlotType.PrimeWeapon1:
                case EWeaponSlotType.PrimeWeapon2:
                case EWeaponSlotType.SubWeapon:
                    return true;
                default:
                    return false;
            }
        }
        
        public static WeaponInPackage ToWeaponInPackage(this EWeaponSlotType slot)
        {
            switch(slot)
            {
                case EWeaponSlotType.PrimeWeapon1:
                    return WeaponInPackage.PrimaryWeaponOne;
                case EWeaponSlotType.PrimeWeapon2:
                    return WeaponInPackage.PrimaryWeaponTwo;
                case EWeaponSlotType.SubWeapon:
                    return WeaponInPackage.SideArm;
                case EWeaponSlotType.MeleeWeapon:
                    return WeaponInPackage.MeleeWeapon;
                case EWeaponSlotType.GrenadeWeapon:
                    return WeaponInPackage.ThrownWeapon;
                case EWeaponSlotType.TacticWeapon:
                    return WeaponInPackage.TacticWeapon;
                default:
                    Logger.ErrorFormat("slot {0} is illegal for weapon ", slot);
                    return WeaponInPackage.ThrownWeapon;
            }
        }

        public static EWeaponSlotType ToWeaponSlot(this EWeaponType weaponType)
        {
            switch(weaponType)
            {
                case EWeaponType.PrimeWeapon:
                    return EWeaponSlotType.PrimeWeapon1;
                case EWeaponType.SubWeapon:
                    return EWeaponSlotType.SubWeapon;
                case EWeaponType.MeleeWeapon:
                    return EWeaponSlotType.MeleeWeapon;
                case EWeaponType.ThrowWeapon:
                    return EWeaponSlotType.GrenadeWeapon;
                case EWeaponType.TacticWeapon:
                    return EWeaponSlotType.TacticWeapon;
                default:
                    return EWeaponSlotType.None;
            }
        }

        public static bool MayHasPart(this EWeaponType weaponType)
        {
            switch(weaponType)
            {
                case EWeaponType.PrimeWeapon:
                case EWeaponType.SubWeapon:
                    return true;
                default:
                    return false;
            }
        }

        public static bool CanAutoPick(this EWeaponType weaponType)
        {
            switch(weaponType)
            {
                case EWeaponType.MeleeWeapon:
                case EWeaponType.PrimeWeapon:
                case EWeaponType.SubWeapon:
                case EWeaponType.TacticWeapon:
                    return true;
                default:
                    return false;
            }
        }

        public static WeaponBagLogic GetBagLogicImp(this PlayerEntity enity)
        {
            return enity.bag.Bag as WeaponBagLogic;
        }
        public static WeaponComponent GetWeaponComponentBySlot(this PlayerEntity player, EWeaponSlotType slot)
        {
            switch (slot)
            {
                case EWeaponSlotType.PrimeWeapon1:
                    if(player.hasPrimeWeapon)
                    {
                        return player.primeWeapon;
                    }
                    break;
                case EWeaponSlotType.PrimeWeapon2:
                    if(player.hasSubWeapon)
                    {
                        return player.subWeapon;
                    }
                    break;
                case EWeaponSlotType.SubWeapon:
                    if(player.hasPistol)
                    {
                        return player.pistol;
                    }
                    break;
                case EWeaponSlotType.MeleeWeapon:
                    if(player.hasMelee)
                    {
                        return player.melee;
                    }
                    break;
                case EWeaponSlotType.GrenadeWeapon:
                    if(player.hasGrenade)
                    {
                        return player.grenade;
                    }
                    break;
                case EWeaponSlotType.TacticWeapon:
                    if(player.hasTacticWeapon)
                    {
                        return player.tacticWeapon;
                    }
                    break;
                case EWeaponSlotType.None:
                    return null;
                default:
                    Logger.WarnFormat("Illegal slot type {0} for weapon ", slot);
                    return null;
            }
            return null;
        }
    }
}
