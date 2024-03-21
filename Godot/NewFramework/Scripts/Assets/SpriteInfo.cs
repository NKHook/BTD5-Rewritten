using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;

public partial class SpriteInfo : Node
{
    private string _name;
    private SpriteInfo _parent = null;
    private readonly string _texturesDirPath;
    private readonly TextureQuality _quality;
    private readonly List<SpriteInfo> _children = new();
    private readonly List<Assets.FrameInfo> _frames = new();

    public readonly string Path;
    
    public SpriteInfo(string name, string texturesDirPath, string filePath, TextureQuality quality = TextureQuality.Ultra, SpriteInfo parent = null)
    {
        _name = name;
        _parent = parent;
        Path = filePath;
        _texturesDirPath = texturesDirPath;
        _quality = quality;

        Load();
    }

    public Assets.CellEntry FindCell(string name) => FindCell(name, "");
    public Assets.CellEntry FindCell(string name, string texture)
    {
        foreach (var result in _children.Select(info => info.FindCell(name, texture)).Where(result => result != null))
        {
            return result;
        }

        return (from frame in _frames where texture == "" || frame.Name == texture select frame.FindCell(name)).FirstOrDefault(result => result != null);
    }

    public Assets.FrameInfo FindFrame(string name)
    {
        foreach (var result in _children.Select(info => info.FindFrame(name)).Where(result => result != null))
        {
            return result;
        }

        return _frames.FirstOrDefault(frame => frame.Name == name);
    }
    
    private void Load()
    {
        var parser = new XmlParser();
        //GD.Print("Loading sprite info: " + Path);
        parser.Open(Path);
        SpriteInfo currentInfo = null;
        Assets.FrameInfo currentFrame = null;
        Assets.AnimationEntry currentAnimation = null;

        var frameLoadTasks = new List<Action>();
        while (parser.Read() != Error.FileEof)
        {
            var nodeName = parser.GetNodeName();
            if (parser.GetNodeType() == XmlParser.NodeType.ElementEnd)
            {
                switch (nodeName)
                {
                    case "SpriteInformation":
                        currentInfo = null;
                        break;
                    case "FrameInformation":
                        Debug.Assert(currentFrame != null);
                        _frames.Add(currentFrame);
                        currentFrame = null;
                        break;
                    case "Animation":
                        Debug.Assert(currentAnimation != null);
                        currentFrame.AddAnimation(currentAnimation);
                        currentAnimation = null;
                        break;
                }
            }
            else if (parser.GetNodeType() == XmlParser.NodeType.Element)
            {
                if ("SpriteInformation" == nodeName)
                {
                    currentInfo = this;
                }

                var attributesDict = new Dictionary();
                for (var idx = 0; idx < parser.GetAttributeCount(); idx++)
                {
                    attributesDict[parser.GetAttributeName(idx)] = parser.GetAttributeValue(idx);
                }

                if (currentFrame != null)
                {
                    switch (nodeName)
                    {
                        case "Animation":
                        {
                            var animationName = attributesDict["name"].AsString();
                            currentAnimation = new Assets.AnimationEntry(currentFrame, _texturesDirPath, this.Path,
                                _quality, animationName);
                            break;
                        }
                        case "Cell":
                        {
                            var cellName = attributesDict["name"].AsString();
                            var cellX = attributesDict["x"].AsInt32();
                            var cellY = attributesDict["y"].AsInt32();
                            var cellW = attributesDict["w"].AsInt32();
                            var cellH = attributesDict["h"].AsInt32();
                            var cellAx = attributesDict["ax"].AsInt32();
                            var cellAy = attributesDict["ay"].AsInt32();
                            var cellAw = attributesDict["aw"].AsInt32();
                            var cellAh = attributesDict["ah"].AsInt32();

                            var theCell = new Assets.CellEntry(null, _texturesDirPath, Path, _quality, cellName,
                                cellX, cellY, cellW, cellH, cellAx, cellAy, cellAw, cellAh);
                            if (currentAnimation != null)
                            {
                                theCell.Parent = currentAnimation;
                                currentAnimation.AddCell(theCell);
                            }
                            else
                            {
                                theCell.Parent = currentFrame;
                                currentFrame.AddCell(theCell);
                            }

                            break;
                        }
                    }
                }
                else if (currentInfo != null)
                {
                    switch (nodeName)
                    {
                        case "SpriteInfoXml":
                        {
                            var sheetName = attributesDict["name"].AsString();
                            var sheetType = attributesDict["type"].AsString();
                            Debug.Assert(Enum.TryParse(sheetType.ToUpper(), out TextureType actualSheetType));
                            _children.Add(new SpriteInfo(sheetName, _texturesDirPath, 
                                _texturesDirPath + "/" + _quality + "/" + sheetName + ".xml", 
                                _quality, this));
                            break;
                        }
                        case "FrameInformation":
                        {
                            var frameName = attributesDict["name"].AsString();
                            var texW = attributesDict["texw"].AsInt32();
                            var texH = attributesDict["texh"].AsInt32();
                            var type = attributesDict["type"].AsString();
                            Debug.Assert(Enum.TryParse(type.ToUpper(), out TextureType actualType));
                            currentFrame = new Assets.FrameInfo(this, _texturesDirPath, Path, _quality, frameName, texW, texH, actualType);
                            break;
                        }
                    }
                }
            }
        }
    }
}