﻿using System;
using App.Shared.Components;
using Assets.XmlConfig;
using Core.Bag;
using Core.GameTime;
using Entitas;
using UnityEngine;

namespace App.Shared.EntityFactory
{
    public class ClientSceneObjectEntityFactory : ServerSceneObjectEntityFactory
    {
        public ClientSceneObjectEntityFactory(SceneObjectContext sceneObjectContext,
            IEntityIdGenerator entityIdGenerator, IEntityIdGenerator equipGenerator, ICurrentTime currentTime) : base(
            sceneObjectContext, entityIdGenerator, equipGenerator, currentTime)
        {

        }

        public override IEntity CreateSimpleEquipmentEntity(ECategory category, int id, int count, Vector3 position)
        {
            return null;
        }

        public override void DestroyEquipmentEntity(int key)
        {
        }

        public override IEntity CreateWeaponEntity(WeaponInfo weaponInfo, Vector3 position)
        {
            return null;
        }

        public override IEntity CreateDropWeaponEntity(WeaponInfo weaponInfo, Vector3 position, int lifeTime)
        {
            return null;
        }

        public override IEntity CreateCastEntity(Vector3 position, float size, int key, string tip)
        {
            return null;
        }
    }
}
