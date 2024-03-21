using System.Collections.Generic;
using System.Text.Json;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Compound;
using BloonsTD5Rewritten.Godot.Scripts.MainMenu;
using Godot;

namespace BloonsTD5Rewritten.Godot.Screens;

public partial class MainMenuScreen : Node2D
{
    [Export] private float _introDuration = 10.0f;

    private Node2D _screenManager = null;
    private float _timePassed = 0.0f;
    private List<Building> _buildings = new();

    private static readonly Script CompoundSpriteScript =
        GD.Load<Script>("res://Godot/NewFramework/Scripts/Compound/CompoundSprite.cs");

    private static readonly Script BuildingScript = GD.Load<Script>("res://Godot/Scripts/MainMenu/Building.cs");

    private List<Building> GetBuildingSprites(JsonElement buildingJson)
    {
        var result = new List<Building>();
        var menuNode = FindChild("menu");
        foreach (var child in menuNode.GetChildren())
        {
            if (child.GetScript().As<Script>() != CompoundSpriteScript) continue;

            foreach (var building in buildingJson.EnumerateArray())
            {
                if (child is not CompoundSprite sprite ||
                    !sprite.SpriteDefinitionRes.EndsWith(building.GetProperty("SpriteFile").GetString() ?? ""))
                    continue;

                var name = sprite.Name;
                var index = sprite.GetIndex();
                sprite.Free();

                var buildingObj = new Building();
                buildingObj.SpriteDefinitionRes =
                    "Assets/JSON/Tablet/UILayout/" + building.GetProperty("SpriteFile").GetString();
                buildingObj.Screen = building.GetProperty("Screen").GetString();
                buildingObj.LocName = building.GetProperty("Name").GetString();
                buildingObj.Initialize();

                buildingObj.Name = name;
                menuNode.AddChild(buildingObj);
                menuNode.MoveChild(buildingObj, index);

                result.Add(buildingObj);
            }
        }

        return result;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _screenManager = FindParent("screen_manager") as Node2D;
        var buildingsJson = JetFileImporter.Instance()
            .GetJsonParsed("Assets/JSON/ScreenDefinitions/MainMenu/BuildingsNoSocial.json");
        _buildings = GetBuildingSprites(buildingsJson.GetProperty("Buildings"));
        Debug.Assert(_buildings.Count > 0);

        foreach (var building in _buildings)
        {
            var collider = new Area2D();
            var area = new CollisionShape2D();
            area.Name = "interaction_check";
            var circleShape = new CircleShape2D();
            circleShape.Radius = 250.0f;
            area.Shape = circleShape;
            collider.AddChild(area);
            collider.InputEvent += (viewport, @event, idx) => BuildingInputEvent(viewport, @event, idx, building);
            collider.MouseEntered += () => BuildingMouseEntered(building);
            collider.MouseExited += () => BuildingMouseExited(building);
            building.AddChild(collider);
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
        if ((!(buttonEvent?.IsPressed() ?? false)) || buttonEvent.ButtonIndex != MouseButton.Left) return;

        var popupScreen = GD.Load<PackedScene>("res://Godot/Screens/" + building.Screen + ".tscn");
        if (_screenManager.FindChild(building.Screen) == null)
            _screenManager.AddChild(popupScreen.Instantiate());
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