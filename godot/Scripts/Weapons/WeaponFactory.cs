using System;
using System.Collections.Generic;
using System.Linq;
using BloonsTD5Rewritten.NewFramework.Scripts;
using BloonsTD5Rewritten.NewFramework.Scripts.Assets;
using BloonsTD5Rewritten.Scripts.Bloons;
using BloonsTD5Rewritten.Scripts.Towers;
using BloonsTD5Rewritten.Scripts.Weapons.Tasks;
using Godot;

namespace BloonsTD5Rewritten.Scripts.Weapons;

public partial class WeaponFactory : BaseFactory<WeaponType, WeaponInfo, Weapon>
{
	public static WeaponFactory? Instance { get; private set; }
	private Dictionary<TowerType, Dictionary<WeaponType, WeaponInfo>> _factoryData = new();
	
	public WeaponFactory() : base(WeaponInfo.Invalid)
	{
		Instance = this;
		DefinitionsDir = "Assets/JSON/WeaponDefinitions/";
	}

	public override WeaponInfo GetInfo(string factoryName)
	{
		throw new NotImplementedException();
	}

	public void AddInfo(TowerType towerFlag, WeaponType weaponFlag, WeaponInfo info)
	{
		if (_factoryData.TryGetValue(towerFlag, out var infoTable))
		{
			infoTable[weaponFlag] = info;
			_factoryData[towerFlag] = infoTable;
		}

		var table = new Dictionary<WeaponType, WeaponInfo>
		{
			[weaponFlag] = info
		};
		_factoryData[towerFlag] = table;
	}

	public WeaponInfo GetInfo(string towerName, string weaponName) =>
		GetInfo(TowerFactory.Instance.StringToFlag<TowerType>(towerName), StringToFlag<WeaponType>(weaponName));
	public WeaponInfo GetInfo(TowerType towerFlag, WeaponType weaponFlag)
	{
		if (_factoryData.TryGetValue(towerFlag, out var infoTable))
		{
			if (infoTable.TryGetValue(weaponFlag, out var weaponInfo))
			{
				return weaponInfo;
			}
		}
		var towerName = TowerFactory.Instance.FlagToString(towerFlag);
		var weaponName = FlagToString(weaponFlag);

		var importer = JetFileImporter.Instance();
		var path = DefinitionsDir + towerName + "/" + ToFileName(weaponName);
		var document = importer.GetJsonParsed(path);
		var info = GenerateInfo(document);
		AddInfo(towerFlag, weaponFlag, info);
		return _factoryData[towerFlag][weaponFlag];
	}

	protected override void InitializeFactory()
	{
	}

	protected override string ToFileName(string factoryName)
	{
		return factoryName + ".weapon";
	}

	protected override WeaponInfo GenerateInfo(JsonWrapper element)
	{
		var info = new WeaponInfo();
		info.Type = element["Type"]?.GetFlag<WeaponType>() ??
					throw new BTD5WouldCrashException("'Type' is not a valid flag");
		info.TargetRange = element["TargetRange"] ?? 0.0f;
		info.CooldownTime = element["CooldownTime"] ?? 0.0f;
		info.FireDelayTime = element["FireDelayTime"] ?? 0.0f;
		info.MaxShots = element["MaxShots"] ?? 0;
		info.Tasks = element["Tasks"]?.EnumerateArray().Select(GenerateTask).ToArray() ?? Array.Empty<WeaponTask>();
		return info;
	}

	protected TaskMovement GenerateMovement(JsonWrapper element)
	{
		var movement = new TaskMovement();
		movement.Type = element["Type"]?.GetFlag<MovementType>() ?? MovementType.None;
		switch (movement.Type)
		{
			case MovementType.None:
				break;
			case MovementType.Forward:
				movement.CutOffDistance = element["CutOffDistance"] ?? -1;
				break;
			case MovementType.RotateAroundTower:
				break;
			case MovementType.GoToTarget:
				break;
			case MovementType.Target:
				break;
			case MovementType.MotionCurve:
				movement.Curves = element["Curves"]?.ArrayAs<Vector2[]>() ?? Array.Empty<Vector2[]>();
				movement.AngleOffsets = element["AngleOffset"] ?? 0.0f;
				movement.TerminateAtEndOfCurve = element["TerminateAtEndOfCurve"] ?? true;
				movement.ScaleCurvesByDirection = element["ScaleCurvesByDirection"] ?? false;
				break;
			case MovementType.ReturnToSender:
				movement.ReturnSpeed = element["ReturnSpeed"]?.GetFloat() ?? 0.0f;
				movement.TargetShouldFaceWeapon = element["TargetShouldFaceWeapon"]?.GetBool() ?? true;
				movement.StartOnTarget = element["StartOnTarget"]?.GetBool() ?? true;
				break;
			case MovementType.MoveToTouch:
				break;
			case MovementType.BeeMovement:
				break;
			case MovementType.BeeSwarmMovement:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
		movement.Speed = element["Speed"] ?? 0.0f;
		return movement;
	}
	
	protected WeaponTask GenerateTask(JsonWrapper element)
	{
		var taskType = element["Type"]?.GetFlag<TaskType>() ?? TaskType.Invalid;
		switch (taskType)
		{
			case TaskType.Invalid:
				throw new BTD5WouldCrashException("Task type is invalid");
			case TaskType.Projectile:
			{
				var result = new ProjectileTask();
				result.Type = taskType;
				result.GraphicName = element["GraphicName"] ?? "";
				result.SpriteFile = element["SpriteFile"] ?? "";
				result.NumPersists = element["NumPersists"] ?? 0;
				result.TerminateOnZeroPersists = element["TerminateOnZeroPersists"] ?? true;
				result.CollisionType = element["CollisionType"]?.GetFlag<CollisionType>() ?? CollisionType.None;
				result.CollidesOnlyWithTarget = element["CollidesOnlyWithTarget"]?.GetBool() ?? false;
				result.Radius = element["Radius"]?.GetFloat() ?? 1.0f;
				result.SpinRate = element["SpinRate"] ?? 0;
				var movement = element["Movement"];
				result.Movement = movement != null ? GenerateMovement(movement) : null;
				result.DisabledTasks = element["DisabledTasks"]?.ArrayAs<int>() ?? Array.Empty<int>();
				result.TasksToProcessOnCollision =
					element["TasksToProcessOnCollision"]?.ArrayAs<int>() ?? Array.Empty<int>();
				result.TasksToProcessOnTerminate =
					element["TasksToProcessOnTerminate"]?.ArrayAs<int>() ?? Array.Empty<int>();
				result.Tasks = element["Tasks"]?.EnumerateArray().Select(GenerateTask).ToArray() ??
							   Array.Empty<WeaponTask>();
				return result;
			}
			case TaskType.MultiFire:
			{
				var result = new MultiFireTask();
				result.Type = TaskType.MultiFire;
				result.InitialOffset = element["InitialOffset"] ?? 0.0f;
				result.AngleIncrement = element["AngleIncrement"] ?? 0.0f;
				result.FireCount = element["FireCount"] ?? 0;
				result.AimAtTarget = element["AimAtTarget"] ?? true;
				result.Offsets = element["Offsets"]?.ArrayAs<Vector2>() ?? Array.Empty<Vector2>();
				result.Tasks = element["Tasks"]?.EnumerateArray().Select(GenerateTask).ToArray() ??
							   Array.Empty<WeaponTask>();
				return result;
			}
			case TaskType.Damage:
			{
				var result = new DamageTask();
				result.Type = taskType;
				result.DamageType = element["DamageType"]?.GetFlag<DamageType>() ?? DamageType.None;
				result.Amount = element["Amount"] ?? 0;
				return result;
			}
			case TaskType.StatusEffect:
			{
				var result = new StatusEffectTask();
				result.Type = TaskType.StatusEffect;
				result.Status = element["Status"]?.GetFlag<StatusFlag>() ?? StatusFlag.None;
				return result;
			}
			case TaskType.RemoveStatusEffect:
			{
				var result = new RemoveStatusEffectTask();
				result.Status = element["Status"]?.GetFlag<StatusFlag>() ?? StatusFlag.None;
				return result;
			}
			case TaskType.AreaOfEffect:
			{
				var result = new AreaOfEffectTask();
				result.Type = taskType;
				result.Range = element["Range"] ?? 0.0f;
				result.MaxTargets = element["MaxTargets"] ?? 0;
				result.Tasks = element["Tasks"]?.EnumerateArray().Select(GenerateTask).ToArray() ??
							   Array.Empty<WeaponTask>();
				return result;
			}
			case TaskType.RandomFire:
			{
				var result = new RandomFireTask();
				result.Type = taskType;
				result.Range = element["Range"] ?? 0.0f;
				result.FireFullRange = element["FireFullRange"] ?? false;
				result.OnlyTargetPlacementLocations = element["OnlyTargetPlacementLocations"] ?? false;
				result.PlacementTowerType = element["PlacementTowerType"]?.GetFlag<TowerType>() ?? TowerType.Invalid;
				result.Tasks = element["Tasks"]?.EnumerateArray().Select(GenerateTask).ToArray() ??
							   Array.Empty<WeaponTask>();
				return result;
			}
			case TaskType.TimerFire:
				break;
			case TaskType.Effect:
			{
				var result = new EffectTask();
				result.Type = taskType;
				result.SpriteFile = element["SpriteFile"] ?? "";
				result.EffectScale = element["Scale"] ?? 1.0f;
				result.DrawLayer = element["DrawLayer"]?.GetFlag<WeaponRenderLayer>() ?? WeaponRenderLayer.Ground;
				result.LoopForever = element["LoopForever"]?.GetBool() ?? false;
				result.Duration = element["Duration"] ?? -1.0f;
				return result;
			}
			case TaskType.TextEffect:
				break;
			case TaskType.BloonSpawnedEvent:
				break;
			case TaskType.RayIntersect:
				break;
			case TaskType.ChainTasks:
				break;
			case TaskType.Collectable:
				break;
			case TaskType.LaunchAircraft:
				break;
			case TaskType.TowerModifier:
			{
				var result = new TowerModifierTask();
				result.Type = taskType;
				result.Range = element["Range"] ?? 1.0f;
				result.PriorityLevel = element["PriorityLevel"] ?? 0;
				result.ApplyToUserTower = element["ApplyToUserTower"] ?? false;
				result.TerminateOnUserUpgrade = element["TerminateOnUserUpgrade"] ?? false;
				result.Duration = element["Duration"] ?? 1.0f;
				result.NumPersists = element["NumPersists"] ?? 1;
				//TODO: result.TargetingFiler = ...
				result.Modifier = new TowerModifier(); //TODO: Make modifier stuff
				return result;
			}
			case TaskType.Harpoon:
				break;
			case TaskType.Sacrifice:
				break;
			case TaskType.AddChildSprite:
				break;
			case TaskType.ChangeTerrain:
				break;
			case TaskType.CreateTower:
			{
				var result = new CreateTowerTask();
				result.Type = taskType;
				result.TowerType = element["TowerType"]?.GetFlag<TowerType>() ?? TowerType.Invalid;
				result.TowerColor = element["TowerColour"]?.GetColor() ?? Colors.White;
				result.TowerLifetime = element["TowerLifetime"]?.GetFloat() ?? 1.0f;
				result.UseParentTowerUpgradeLevel = element["UseParentTowerUpgradeLevel"] ?? false;
				return result;
			}
			case TaskType.ResetPopCount:
				break;
			case TaskType.EnableNextWeaponSlot:
				break;
			case TaskType.EnableWeaponSlot:
				break;
			case TaskType.ResetCooldown:
				break;
			case TaskType.RedeployTower:
				break;
			case TaskType.ChangeSubtaskEnabled:
				break;
			case TaskType.CollectCollectables:
			{
				var result = new CollectCollectablesTask();
				result.Range = element["Range"]?.GetFloat() ?? 0.0f;
				result.Speed = element["Speed"]?.GetFloat() ?? 0.0f;
				result.CollectionDelay = element["CollectionDelay"]?.GetFloat() ?? 0.0f;
				result.AnimateOnCollection = element["AnimateOnCollection"]?.GetBool() ?? false;
				return result;
			}
			case TaskType.RestoreAmmo:
				break;
			case TaskType.FireTaskAtLocation:
				break;
			case TaskType.OverclockTask:
				break;
			case TaskType.RBEProjectile:
				break;
			case TaskType.TrapBloon:
				break;
			case TaskType.DamageSpread:
				break;
			case TaskType.TowerBoost:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		throw new NotImplementedException();
	}
	
	public override Weapon? Instantiate(string factoryName)
	{
		throw new NotImplementedException();
	}
	public override Weapon? Instantiate(WeaponType flag)
	{
		throw new NotImplementedException();
	}
}
