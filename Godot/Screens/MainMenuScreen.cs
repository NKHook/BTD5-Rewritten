using System.Collections.Generic;
using System.Text.Json;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Compound;
using BloonsTD5Rewritten.Godot.Scripts.MainMenu;
using Godot;

namespace BloonsTD5Rewritten.Godot.Screens;

public partial class MainMenuScreen : Node2D
{
    [Export] private float _introDuration = 10.0f;

    private float _timePassed = 0.0f;
    private List<Building> _buildings = new();

    private static readonly Script CompoundSpriteScript =
        GD.Load<Script>("res://Godot/NewFramework/Scripts/Compound/CompoundSprite.cs");

    private static readonly Script BuildingScript = GD.Load<Script>("res://Godot/Scripts/MainMenu/Building.cs");

    private List<Building> GetBuildingSprites(JsonWrapper buildingJson)
    {
        var result = new List<Building>();
        var menuNode = FindChild("menu") as CompoundSprite;
        foreach (var (uid, child) in menuNode!.GetActors())
        {
            if (child.GetScript().As<Script>() != CompoundSpriteScript) continue;

            foreach (var building in buildingJson.EnumerateArray())
            {
                if (child is not CompoundSprite sprite ||
                    !sprite.SpriteDefinitionRes.EndsWith(building["SpriteFile"]))
                    continue;

                var buildingObj = new Building();
                buildingObj.Screen = building["Screen"];
                buildingObj.LocName = building["Name"];

                sprite.AddChild(buildingObj);

                result.Add(buildingObj);
            }
        }

        return result;
    }

    private Area2D? GetTriggerForScreen(string screenName)
    {
        var buildingTriggers = GetNode("building_triggers");
        foreach (var child in buildingTriggers.GetChildren())
        {
            if (child is Area2D area && area.Name == screenName)
                return area;
        }

        return null;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var buildingsJson = JetFileImporter.Instance()
            .GetJsonParsed("Assets/JSON/ScreenDefinitions/MainMenu/BuildingsNoSocial.json");
        _buildings = GetBuildingSprites(buildingsJson["Buildings"]);
        Debug.Assert(_buildings.Count > 0);

        foreach (var building in _buildings)
        {
            var trigger = GetTriggerForScreen(building.Screen);
            if (trigger is null) continue;
            
            trigger.InputEvent += (viewport, @event, idx) => BuildingInputEvent(viewport, @event, idx, building);
            trigger.MouseEntered += () => BuildingMouseEntered(building);
            trigger.MouseExited += () => BuildingMouseExited(building);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        _timePassed += (float)delta;
        if (_timePassed < _introDuration)
        {
            if (Input.IsMouseButtonPressed(MouseButton.Left))
                _timePassed = _introDuration;
            return;
        }


        var cameraFollow = GetNode<PathFollow2D>("camera_path/camera_follow");
        cameraFollow.ProgressRatio += (float)delta;
        if (!(cameraFollow.ProgressRatio > 1.0f)) return;

        GetNode("sky").Free();
        GetNode("intro").Free();
    }

    private void BuildingInputEvent(Node viewport, InputEvent @event, long shapeidx, Building building)
    {
        var buttonEvent = @event as InputEventMouseButton;
        if (!(buttonEvent?.IsPressed() ?? false) || buttonEvent.ButtonIndex != MouseButton.Left) return;

        ScreenManager.Instance().OpenPopup(building.Screen);
    }

    private void BuildingMouseExited(Building building)
    {
        building.Hovered = true;
    }

    private void BuildingMouseEntered(Building building)
    {
        building.Hovered = false;
    }
}