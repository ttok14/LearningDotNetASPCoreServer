//*** Auto Generation Code ***

using System;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;

namespace GameDB
{
	public class GameDBContainer
	{
		public Dictionary<uint, ActionPointTable> ActionPointTable_data = new Dictionary<uint, ActionPointTable>();
		public Dictionary<uint, AIProfileTable> AIProfileTable_data = new Dictionary<uint, AIProfileTable>();
		public Dictionary<uint, AnimationTable> AnimationTable_data = new Dictionary<uint, AnimationTable>();
		public Dictionary<string, AssetMetaTable> AssetMetaTable_data = new Dictionary<string, AssetMetaTable>();
		public Dictionary<uint, AudioTable> AudioTable_data = new Dictionary<uint, AudioTable>();
		public Dictionary<uint, CharacterTable> CharacterTable_data = new Dictionary<uint, CharacterTable>();
		public Dictionary<uint, CurrencyTable> CurrencyTable_data = new Dictionary<uint, CurrencyTable>();
		public Dictionary<uint, EntityTable> EntityTable_data = new Dictionary<uint, EntityTable>();
		public Dictionary<uint, FieldItemTable> FieldItemTable_data = new Dictionary<uint, FieldItemTable>();
		public Dictionary<uint, ItemTable> ItemTable_data = new Dictionary<uint, ItemTable>();
		public Dictionary<uint, KillStreakTable> KillStreakTable_data = new Dictionary<uint, KillStreakTable>();
		public Dictionary<uint, PetTable> PetTable_data = new Dictionary<uint, PetTable>();
		public Dictionary<uint, ProjectileTable> ProjectileTable_data = new Dictionary<uint, ProjectileTable>();
		public Dictionary<uint, PurchaseCostTable> PurchaseCostTable_data = new Dictionary<uint, PurchaseCostTable>();
		public Dictionary<uint, ResourceTable> ResourceTable_data = new Dictionary<uint, ResourceTable>();
		public Dictionary<uint, SkillTable> SkillTable_data = new Dictionary<uint, SkillTable>();
		public Dictionary<uint, StatTable> StatTable_data = new Dictionary<uint, StatTable>();
		public Dictionary<uint, StructureTable> StructureTable_data = new Dictionary<uint, StructureTable>();
		public Dictionary<uint, WaveSequenceTable> WaveSequenceTable_data = new Dictionary<uint, WaveSequenceTable>();
		public Dictionary<uint, WaveTable> WaveTable_data = new Dictionary<uint, WaveTable>();
	}

	[MessagePackObject]
	public class ActionPointTable
	{
		[Key(0)]
		public uint ID;
		[Key(1)]
		public uint GroupID;
		[Key(2)]
		public E_ActionPointType Type;
		[Key(3)]
		public Vector2Int[] Positions;
		[Key(4)]
		public E_EntityFlags Flags;

		public static Dictionary<uint, ActionPointTable> Deserialize(ref byte[] _readBytes)
		{
			Dictionary<uint, ActionPointTable> dicTables = new Dictionary<uint, ActionPointTable>();
			MessagePackReader reader = new MessagePackReader(new System.ReadOnlyMemory<byte>(_readBytes));
			int tableCount = MessagePackSerializer.Deserialize<int>(ref reader);
			for (int i = 0; i < tableCount; i++)
			{
				var table = MessagePackSerializer.Deserialize<ActionPointTable>(ref reader);
				dicTables.Add(table.ID, table);
			}
			return dicTables;
		}
	}

	[MessagePackObject]
	public class AIProfileTable
	{
		[Key(0)]
		public uint ID;
		[Key(1)]
		public E_AITargetingRuleType RuleType;
		[Key(2)]
		public int MaxScore_Dist;
		[Key(3)]
		public int MaxScore_Aggro;
		[Key(4)]
		public int CharacterScore;
		[Key(5)]
		public int StructureScore;
		[Key(6)]
		public int DefenseStructureScore;
		[Key(7)]
		public int ObstacleStructureScore;
		[Key(8)]
		public float RescanInterval;
		[Key(9)]
		public float AdditionalTargetSwitchMargin;

		public static Dictionary<uint, AIProfileTable> Deserialize(ref byte[] _readBytes)
		{
			Dictionary<uint, AIProfileTable> dicTables = new Dictionary<uint, AIProfileTable>();
			MessagePackReader reader = new MessagePackReader(new System.ReadOnlyMemory<byte>(_readBytes));
			int tableCount = MessagePackSerializer.Deserialize<int>(ref reader);
			for (int i = 0; i < tableCount; i++)
			{
				var table = MessagePackSerializer.Deserialize<AIProfileTable>(ref reader);
				dicTables.Add(table.ID, table);
			}
			return dicTables;
		}
	}

	[MessagePackObject]
	public class AnimationTable
	{
		[Key(0)]
		public uint ID;
		[Key(1)]
		public string ControllerName;
		[Key(2)]
		public string StateName;
		[Key(3)]
		public float TriggerAt;

		public static Dictionary<uint, AnimationTable> Deserialize(ref byte[] _readBytes)
		{
			Dictionary<uint, AnimationTable> dicTables = new Dictionary<uint, AnimationTable>();
			MessagePackReader reader = new MessagePackReader(new System.ReadOnlyMemory<byte>(_readBytes));
			int tableCount = MessagePackSerializer.Deserialize<int>(ref reader);
			for (int i = 0; i < tableCount; i++)
			{
				var table = MessagePackSerializer.Deserialize<AnimationTable>(ref reader);
				dicTables.Add(table.ID, table);
			}
			return dicTables;
		}
	}

	[MessagePackObject]
	public class AssetMetaTable
	{
		[Key(0)]
		public string Key;
		[Key(1)]
		public E_AssetType AssetType;
		[Key(2)]
		public E_LoaderType LoaderType;

		public static Dictionary<string, AssetMetaTable> Deserialize(ref byte[] _readBytes)
		{
			Dictionary<string, AssetMetaTable> dicTables = new Dictionary<string, AssetMetaTable>();
			MessagePackReader reader = new MessagePackReader(new System.ReadOnlyMemory<byte>(_readBytes));
			int tableCount = MessagePackSerializer.Deserialize<int>(ref reader);
			for (int i = 0; i < tableCount; i++)
			{
				var table = MessagePackSerializer.Deserialize<AssetMetaTable>(ref reader);
				dicTables.Add(table.Key, table);
			}
			return dicTables;
		}
	}

	[MessagePackObject]
	public class AudioTable
	{
		[Key(0)]
		public uint ID;
		[Key(1)]
		public E_AudioType AudioType;
		[Key(2)]
		public string ResourceKey;
		[Key(3)]
		public bool Is3D;
		[Key(4)]
		public E_Audio3D_DistanceType DistanceType;
		[Key(5)]
		public float Volume;
		[Key(6)]
		public float RandomPitchRange;

		public static Dictionary<uint, AudioTable> Deserialize(ref byte[] _readBytes)
		{
			Dictionary<uint, AudioTable> dicTables = new Dictionary<uint, AudioTable>();
			MessagePackReader reader = new MessagePackReader(new System.ReadOnlyMemory<byte>(_readBytes));
			int tableCount = MessagePackSerializer.Deserialize<int>(ref reader);
			for (int i = 0; i < tableCount; i++)
			{
				var table = MessagePackSerializer.Deserialize<AudioTable>(ref reader);
				dicTables.Add(table.ID, table);
			}
			return dicTables;
		}
	}

	[MessagePackObject]
	public class CharacterTable
	{
		[Key(0)]
		public uint ID;
		[Key(1)]
		public E_CharacterType CharacterType;
		[Key(2)]
		public E_ZoneType ZoneType;
		[Key(3)]
		public uint AIProfileID;
		[Key(4)]
		public E_SizeType CharacterSize;
		[Key(5)]
		public uint[] SkillSet;
		[Key(6)]
		public float ShadowScale;
		[Key(7)]
		public string ActiveFXKey;
		[Key(8)]
		public string MoveTrailFXKey;
		[Key(9)]
		public bool UseIK;
		[Key(10)]
		public uint DropItemID;
		[Key(11)]
		public string FootStepAudioKey;

		public static Dictionary<uint, CharacterTable> Deserialize(ref byte[] _readBytes)
		{
			Dictionary<uint, CharacterTable> dicTables = new Dictionary<uint, CharacterTable>();
			MessagePackReader reader = new MessagePackReader(new System.ReadOnlyMemory<byte>(_readBytes));
			int tableCount = MessagePackSerializer.Deserialize<int>(ref reader);
			for (int i = 0; i < tableCount; i++)
			{
				var table = MessagePackSerializer.Deserialize<CharacterTable>(ref reader);
				dicTables.Add(table.ID, table);
			}
			return dicTables;
		}
	}

	[MessagePackObject]
	public class CurrencyTable
	{
		[Key(0)]
		public uint ID;
		[Key(1)]
		public E_CurrencyType Type;
		[Key(2)]
		public string Name;
		[Key(3)]
		public string Description;
		[Key(4)]
		public string SmallIconKey;
		[Key(5)]
		public string MediumIconKey;
		[Key(6)]
		public string BigIconKey;

		public static Dictionary<uint, CurrencyTable> Deserialize(ref byte[] _readBytes)
		{
			Dictionary<uint, CurrencyTable> dicTables = new Dictionary<uint, CurrencyTable>();
			MessagePackReader reader = new MessagePackReader(new System.ReadOnlyMemory<byte>(_readBytes));
			int tableCount = MessagePackSerializer.Deserialize<int>(ref reader);
			for (int i = 0; i < tableCount; i++)
			{
				var table = MessagePackSerializer.Deserialize<CurrencyTable>(ref reader);
				dicTables.Add(table.ID, table);
			}
			return dicTables;
		}
	}

	[MessagePackObject]
	public class EntityTable
	{
		[Key(0)]
		public uint ID;
		[Key(1)]
		public string Name;
		[Key(2)]
		public string ResourceKey;
		[Key(3)]
		public E_EntityType EntityType;
		[Key(4)]
		public E_EntityModelType ModelType;
		[Key(5)]
		public uint DetailTableID;
		[Key(6)]
		public uint StatTableID;
		[Key(7)]
		public string IconKey;
		[Key(8)]
		public Vector2Int[] OccupyOffsets;
		[Key(9)]
		public E_EntityFlags EntityFlags;

		public static Dictionary<uint, EntityTable> Deserialize(ref byte[] _readBytes)
		{
			Dictionary<uint, EntityTable> dicTables = new Dictionary<uint, EntityTable>();
			MessagePackReader reader = new MessagePackReader(new System.ReadOnlyMemory<byte>(_readBytes));
			int tableCount = MessagePackSerializer.Deserialize<int>(ref reader);
			for (int i = 0; i < tableCount; i++)
			{
				var table = MessagePackSerializer.Deserialize<EntityTable>(ref reader);
				dicTables.Add(table.ID, table);
			}
			return dicTables;
		}
	}

	[MessagePackObject]
	public class FieldItemTable
	{
		[Key(0)]
		public uint ID;
		[Key(1)]
		public E_FieldItemType ItemType;
		[Key(2)]
		public string Name;
		[Key(3)]
		public string ResourceKey;
		[Key(4)]
		public bool Is3D;
		[Key(5)]
		public uint DetailID;

		public static Dictionary<uint, FieldItemTable> Deserialize(ref byte[] _readBytes)
		{
			Dictionary<uint, FieldItemTable> dicTables = new Dictionary<uint, FieldItemTable>();
			MessagePackReader reader = new MessagePackReader(new System.ReadOnlyMemory<byte>(_readBytes));
			int tableCount = MessagePackSerializer.Deserialize<int>(ref reader);
			for (int i = 0; i < tableCount; i++)
			{
				var table = MessagePackSerializer.Deserialize<FieldItemTable>(ref reader);
				dicTables.Add(table.ID, table);
			}
			return dicTables;
		}
	}

	[MessagePackObject]
	public class ItemTable
	{
		[Key(0)]
		public uint ID;
		[Key(1)]
		public E_ItemType Type;
		[Key(2)]
		public string Name;
		[Key(3)]
		public string Description;
		[Key(4)]
		public string IconKey;
		[Key(5)]
		public E_ItemGradeType GradeType;
		[Key(6)]
		public int SquadCount;
		[Key(7)]
		public int MaxQuantity;
		[Key(8)]
		public uint RefID;

		public static Dictionary<uint, ItemTable> Deserialize(ref byte[] _readBytes)
		{
			Dictionary<uint, ItemTable> dicTables = new Dictionary<uint, ItemTable>();
			MessagePackReader reader = new MessagePackReader(new System.ReadOnlyMemory<byte>(_readBytes));
			int tableCount = MessagePackSerializer.Deserialize<int>(ref reader);
			for (int i = 0; i < tableCount; i++)
			{
				var table = MessagePackSerializer.Deserialize<ItemTable>(ref reader);
				dicTables.Add(table.ID, table);
			}
			return dicTables;
		}
	}

	[MessagePackObject]
	public class KillStreakTable
	{
		[Key(0)]
		public uint ID;
		[Key(1)]
		public uint KillCount;
		[Key(2)]
		public float DisplayDuration;
		[Key(3)]
		public string NotificationText;
		[Key(4)]
		public string[] AudioKeys;
		[Key(5)]
		public string ColorHex;
		[Key(6)]
		public float ScalePunch;
		[Key(7)]
		public bool DoImpulse;

		public static Dictionary<uint, KillStreakTable> Deserialize(ref byte[] _readBytes)
		{
			Dictionary<uint, KillStreakTable> dicTables = new Dictionary<uint, KillStreakTable>();
			MessagePackReader reader = new MessagePackReader(new System.ReadOnlyMemory<byte>(_readBytes));
			int tableCount = MessagePackSerializer.Deserialize<int>(ref reader);
			for (int i = 0; i < tableCount; i++)
			{
				var table = MessagePackSerializer.Deserialize<KillStreakTable>(ref reader);
				dicTables.Add(table.ID, table);
			}
			return dicTables;
		}
	}

	[MessagePackObject]
	public class PetTable
	{
		[Key(0)]
		public uint ID;
		[Key(1)]
		public bool IsRidable;
		[Key(2)]
		public string MoveTrailFXKey;

		public static Dictionary<uint, PetTable> Deserialize(ref byte[] _readBytes)
		{
			Dictionary<uint, PetTable> dicTables = new Dictionary<uint, PetTable>();
			MessagePackReader reader = new MessagePackReader(new System.ReadOnlyMemory<byte>(_readBytes));
			int tableCount = MessagePackSerializer.Deserialize<int>(ref reader);
			for (int i = 0; i < tableCount; i++)
			{
				var table = MessagePackSerializer.Deserialize<PetTable>(ref reader);
				dicTables.Add(table.ID, table);
			}
			return dicTables;
		}
	}

	[MessagePackObject]
	public class ProjectileTable
	{
		[Key(0)]
		public uint ID;
		[Key(1)]
		public string ResourceKey;
		[Key(2)]
		public float LifeTime;
		[Key(3)]
		public E_ProjectileTargetingType TargetingType;
		[Key(4)]
		public E_AimType AimType;
		[Key(5)]
		public bool EnableShowTargetingIndicator;
		[Key(6)]
		public E_DeliveryContextInheritType InheritType;
		[Key(7)]
		public E_ProjectileMovementType MovementType;
		[Key(8)]
		public float MovementValue01;
		[Key(9)]
		public float MovementValue02;
		[Key(10)]
		public float MovementValue03;
		[Key(11)]
		public float MaxDistance;
		[Key(12)]
		public E_ProjectileCollisionActivationType CollisionActivationType;
		[Key(13)]
		public E_CollisionRangeType CollisionRangeType;
		[Key(14)]
		public float CollisionAreaRange;
		[Key(15)]
		public float CollisionForce;
		[Key(16)]
		public uint PreferMaxTargetCount;
		[Key(17)]
		public float MoveSpeed;
		[Key(18)]
		public float StatReductionMinRatio;
		[Key(19)]
		public float StatReductionRatioPerHit;
		[Key(20)]
		public bool AllowMultiHit;
		[Key(21)]
		public string[] HitSFXKeys;
		[Key(22)]
		public bool AudioRandomPick;
		[Key(23)]
		public string[] HitFXKeys;
		[Key(24)]
		public bool HitDestroy;
		[Key(25)]
		public E_UpdateLogicType UpdateLogicType;
		[Key(26)]
		public float UpdateLogicValue;
		[Key(27)]
		public E_ActionType ProcessActionType;
		[Key(28)]
		public E_ZoneType ProcessActionTargetZoneType;
		[Key(29)]
		public E_RefDataType ProcessRefType;
		[Key(30)]
		public uint ProcessRefID;
		[Key(31)]
		public string ProcessRefKey;
		[Key(32)]
		public float ProcessValue01;
		[Key(33)]
		public float ProcessValue02;
		[Key(34)]
		public float ProcessValue03;
		[Key(35)]
		public bool ProcessDestroy;
		[Key(36)]
		public string[] ProcessSFXKeys;
		[Key(37)]
		public string[] ProcessFXKeys;
		[Key(38)]
		public E_ActionType EndActionType;
		[Key(39)]
		public E_ZoneType EndActionTargetZoneType;
		[Key(40)]
		public E_RefDataType EndRefType;
		[Key(41)]
		public uint EndRefID;
		[Key(42)]
		public string EndRefKey;
		[Key(43)]
		public float EndValue01;
		[Key(44)]
		public float EndValue02;
		[Key(45)]
		public float EndValue03;
		[Key(46)]
		public string[] EndSFXKeys;
		[Key(47)]
		public string[] EndFXKeys;

		public static Dictionary<uint, ProjectileTable> Deserialize(ref byte[] _readBytes)
		{
			Dictionary<uint, ProjectileTable> dicTables = new Dictionary<uint, ProjectileTable>();
			MessagePackReader reader = new MessagePackReader(new System.ReadOnlyMemory<byte>(_readBytes));
			int tableCount = MessagePackSerializer.Deserialize<int>(ref reader);
			for (int i = 0; i < tableCount; i++)
			{
				var table = MessagePackSerializer.Deserialize<ProjectileTable>(ref reader);
				dicTables.Add(table.ID, table);
			}
			return dicTables;
		}
	}

	[MessagePackObject]
	public class PurchaseCostTable
	{
		[Key(0)]
		public uint ID;
		[Key(1)]
		public E_PurchaseContentType ContentType;
		[Key(2)]
		public uint RefID;
		[Key(3)]
		public E_CurrencyType CostCurrencyType;
		[Key(4)]
		public uint CostPrice;

		public static Dictionary<uint, PurchaseCostTable> Deserialize(ref byte[] _readBytes)
		{
			Dictionary<uint, PurchaseCostTable> dicTables = new Dictionary<uint, PurchaseCostTable>();
			MessagePackReader reader = new MessagePackReader(new System.ReadOnlyMemory<byte>(_readBytes));
			int tableCount = MessagePackSerializer.Deserialize<int>(ref reader);
			for (int i = 0; i < tableCount; i++)
			{
				var table = MessagePackSerializer.Deserialize<PurchaseCostTable>(ref reader);
				dicTables.Add(table.ID, table);
			}
			return dicTables;
		}
	}

	[MessagePackObject]
	public class ResourceTable
	{
		[Key(0)]
		public uint ID;
		[Key(1)]
		public E_ResourceType ResourceType;

		public static Dictionary<uint, ResourceTable> Deserialize(ref byte[] _readBytes)
		{
			Dictionary<uint, ResourceTable> dicTables = new Dictionary<uint, ResourceTable>();
			MessagePackReader reader = new MessagePackReader(new System.ReadOnlyMemory<byte>(_readBytes));
			int tableCount = MessagePackSerializer.Deserialize<int>(ref reader);
			for (int i = 0; i < tableCount; i++)
			{
				var table = MessagePackSerializer.Deserialize<ResourceTable>(ref reader);
				dicTables.Add(table.ID, table);
			}
			return dicTables;
		}
	}

	[MessagePackObject]
	public class SkillTable
	{
		[Key(0)]
		public uint ID;
		[Key(1)]
		public string Name;
		[Key(2)]
		public string IconKey;
		[Key(3)]
		public E_SkillCategoryType SkillCategory;
		[Key(4)]
		public E_SkillType SkillType;
		[Key(5)]
		public string Description;
		[Key(6)]
		public E_ZoneType TargetZoneType;
		[Key(7)]
		public E_SkillTriggerConditionType TriggerCondition;
		[Key(8)]
		public E_SkillTriggerType TriggerType;
		[Key(9)]
		public bool RequireFacingTarget;
		[Key(10)]
		public float CastingTime;
		[Key(11)]
		public string[] TriggerAudioKey;
		[Key(12)]
		public bool AudioRandomPick;
		[Key(13)]
		public string[] EffectKeys;
		[Key(14)]
		public bool KillExecutor;
		[Key(15)]
		public string ProjectileKey;
		[Key(16)]
		public uint ProjectileCount;
		[Key(17)]
		public float CooldownTime;
		[Key(18)]
		public uint Cost;
		[Key(19)]
		public uint BaseDamage;
		[Key(20)]
		public float Range;
		[Key(21)]
		public bool LookAtTarget;
		[Key(22)]
		public E_CollisionRangeType ImpactCollisionRangeType;
		[Key(23)]
		public float ImpactCollisionRange;
		[Key(24)]
		public uint PreferMaxTargetCount;
		[Key(25)]
		public float ImpactCollisionForce;
		[Key(26)]
		public string[] ImpactSFXHitKeys;
		[Key(27)]
		public string[] ImpactFXHitKeys;
		[Key(28)]
		public E_SpellPositionType SpellStartPositionType;
		[Key(29)]
		public Vector3 SpellStartOffset;
		[Key(30)]
		public bool SpellStartOffsetRelative;
		[Key(31)]
		public E_SpellPositionType SpellEndPositionType;
		[Key(32)]
		public Vector3 SpellEndOffset;
		[Key(33)]
		public bool SpellEndOffsetRelative;

		public static Dictionary<uint, SkillTable> Deserialize(ref byte[] _readBytes)
		{
			Dictionary<uint, SkillTable> dicTables = new Dictionary<uint, SkillTable>();
			MessagePackReader reader = new MessagePackReader(new System.ReadOnlyMemory<byte>(_readBytes));
			int tableCount = MessagePackSerializer.Deserialize<int>(ref reader);
			for (int i = 0; i < tableCount; i++)
			{
				var table = MessagePackSerializer.Deserialize<SkillTable>(ref reader);
				dicTables.Add(table.ID, table);
			}
			return dicTables;
		}
	}

	[MessagePackObject]
	public class StatTable
	{
		[Key(0)]
		public uint ID;
		[Key(1)]
		public string Description;
		[Key(2)]
		public uint Grade;
		[Key(3)]
		public uint Population;
		[Key(4)]
		public uint BaseHP;
		[Key(5)]
		public bool IsInvincible;
		[Key(6)]
		public uint BaseAttackPower;
		[Key(7)]
		public float AttackSpeed;
		[Key(8)]
		public float AttackSpeedGrowthPerLevel;
		[Key(9)]
		public uint HPGrowthPerLevel;
		[Key(10)]
		public uint AttackGrowthPerLevel;
		[Key(11)]
		public float MoveSpeed;
		[Key(12)]
		public float RotateSpeed;
		[Key(13)]
		public float ScanRange;

		public static Dictionary<uint, StatTable> Deserialize(ref byte[] _readBytes)
		{
			Dictionary<uint, StatTable> dicTables = new Dictionary<uint, StatTable>();
			MessagePackReader reader = new MessagePackReader(new System.ReadOnlyMemory<byte>(_readBytes));
			int tableCount = MessagePackSerializer.Deserialize<int>(ref reader);
			for (int i = 0; i < tableCount; i++)
			{
				var table = MessagePackSerializer.Deserialize<StatTable>(ref reader);
				dicTables.Add(table.ID, table);
			}
			return dicTables;
		}
	}

	[MessagePackObject]
	public class StructureTable
	{
		[Key(0)]
		public uint ID;
		[Key(1)]
		public E_StructureType StructureType;
		[Key(2)]
		public bool IsRepairable;
		[Key(3)]
		public string Description;
		[Key(4)]
		public uint AIProfileID;
		[Key(5)]
		public string SoundKey;
		[Key(6)]
		public string DestroyEffectKey;
		[Key(7)]
		public uint[] SkillSet;
		[Key(8)]
		public uint ActionPointGroupID;
		[Key(9)]
		public E_CurrencyType UpgradeCostCurrencyType;
		[Key(10)]
		public uint UpgradeCost;
		[Key(11)]
		public uint MaxLevel;
		[Key(12)]
		public uint ResidentCapacity;
		[Key(13)]
		public E_ResourceType GenResourceType;
		[Key(14)]
		public uint GenResourceID;
		[Key(15)]
		public uint GenResourceBaseAmount;
		[Key(16)]
		public uint GenCurrencyGrowthPerLevel;
		[Key(17)]
		public float GenResourceInterval;
		[Key(18)]
		public uint SpawnEntityIDOnCombat;
		[Key(19)]
		public float SpawnIntervalSeconds;
		[Key(20)]
		public uint GarrisonCount;

		public static Dictionary<uint, StructureTable> Deserialize(ref byte[] _readBytes)
		{
			Dictionary<uint, StructureTable> dicTables = new Dictionary<uint, StructureTable>();
			MessagePackReader reader = new MessagePackReader(new System.ReadOnlyMemory<byte>(_readBytes));
			int tableCount = MessagePackSerializer.Deserialize<int>(ref reader);
			for (int i = 0; i < tableCount; i++)
			{
				var table = MessagePackSerializer.Deserialize<StructureTable>(ref reader);
				dicTables.Add(table.ID, table);
			}
			return dicTables;
		}
	}

	[MessagePackObject]
	public class WaveSequenceTable
	{
		[Key(0)]
		public uint ID;
		[Key(1)]
		public uint WaveID;
		[Key(2)]
		public uint Order;
		[Key(3)]
		public E_WaveCommandType CmdType;
		[Key(4)]
		public float ResumeDelay;
		[Key(5)]
		public uint Chance;
		[Key(6)]
		public uint IntValue01;
		[Key(7)]
		public uint IntValue02;
		[Key(8)]
		public uint IntValue03;
		[Key(9)]
		public float FloatValue01;
		[Key(10)]
		public float FloatValue02;
		[Key(11)]
		public string StringValue01;

		public static Dictionary<uint, WaveSequenceTable> Deserialize(ref byte[] _readBytes)
		{
			Dictionary<uint, WaveSequenceTable> dicTables = new Dictionary<uint, WaveSequenceTable>();
			MessagePackReader reader = new MessagePackReader(new System.ReadOnlyMemory<byte>(_readBytes));
			int tableCount = MessagePackSerializer.Deserialize<int>(ref reader);
			for (int i = 0; i < tableCount; i++)
			{
				var table = MessagePackSerializer.Deserialize<WaveSequenceTable>(ref reader);
				dicTables.Add(table.ID, table);
			}
			return dicTables;
		}
	}

	[MessagePackObject]
	public class WaveTable
	{
		[Key(0)]
		public uint ID;
		[Key(1)]
		public uint StageID;
		[Key(2)]
		public string Description;
		[Key(3)]
		public float NextWaveDelay;
		[Key(4)]
		public E_CurrencyType RewardCurrencyType;
		[Key(5)]
		public uint RewardAmount;

		public static Dictionary<uint, WaveTable> Deserialize(ref byte[] _readBytes)
		{
			Dictionary<uint, WaveTable> dicTables = new Dictionary<uint, WaveTable>();
			MessagePackReader reader = new MessagePackReader(new System.ReadOnlyMemory<byte>(_readBytes));
			int tableCount = MessagePackSerializer.Deserialize<int>(ref reader);
			for (int i = 0; i < tableCount; i++)
			{
				var table = MessagePackSerializer.Deserialize<WaveTable>(ref reader);
				dicTables.Add(table.ID, table);
			}
			return dicTables;
		}
	}

}
