using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using Godot.Collections;
using FileAccess = Godot.FileAccess;

namespace BloonsTD5Rewritten.NewFramework.Scripts.Assets;

public partial class SpriteInfo : Node
{
    private string _name;
    private SpriteInfo? _parent = null;
    private readonly string _texturesDirPath;
    private readonly TextureQuality _firstQuality;
    private readonly List<SpriteInfo> _children = new();
    private readonly List<FrameInfo> _frames = new();

    public readonly string Path;

    public SpriteInfo(string name, string texturesDirPath, string filePath,
        TextureQuality firstQuality = TextureQuality.Ultra, SpriteInfo? parent = null)
    {
        _name = name;
        _parent = parent;
        Path = filePath;
        if (!FileAccess.FileExists(Path))
            throw new FileNotFoundException();
        _texturesDirPath = texturesDirPath;
        _firstQuality = firstQuality;

        Load();
    }

    public CellEntry? FindCell(string name) => FindCell(name, "");

    public CellEntry? FindCell(string name, string texture)
    {
        foreach (var result in _children.Select(info => info.FindCell(name, texture)).Where(result => result != null))
        {
            return result;
        }

        return (from frame in _frames where texture == "" || frame.FrameName == texture select frame.FindCell(name))
            .FirstOrDefault(result => result != null);
    }

    public FrameInfo? FindFrame(string name)
    {
        foreach (var result in _children.Select(info => info.FindFrame(name)).Where(result => result != null))
        {
            return result;
        }

        return _frames.FirstOrDefault(frame => frame.FrameName == name);
    }

    private void Load()
    {
        var parser = new XmlParser();
        parser.Open(Path);
        SpriteInfo? currentInfo = null;
        FrameInfo? currentFrame = null;
        AnimationEntry? currentAnimation = null;

        while (parser.Read() != Error.FileEof)
        {
            string? nodeName;
            if (parser.GetNodeType() != XmlParser.NodeType.Text)
                nodeName = parser.GetNodeName();
            else
                nodeName = "";

            if (parser.GetNodeType() == XmlParser.NodeType.ElementEnd)
            {
                switch (nodeName)
                {
                    case "SpriteInformation":
                        currentInfo = null;
                        break;
                    case "FrameInformation":
                        Debug.Assert(currentFrame != null);
                        _frames.Add(currentFrame!);
                        currentFrame = null;
                        break;
                    case "Animation":
                        Debug.Assert(currentAnimation != null);
                        currentFrame?.AddAnimation(currentAnimation!);
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
                            currentAnimation = new AnimationEntry(currentFrame, _texturesDirPath, this.Path,
                                _firstQuality, animationName);
                            break;
                        }
                        case "Cell":
                        {
                            var cellName = attributesDict["name"].AsString();
                            var cellX = attributesDict["x"].AsInt32();
                            var cellY = attributesDict["y"].AsInt32();
                            var cellW = attributesDict["w"].AsInt32();
                            var cellH = attributesDict["h"].AsInt32();
                            var cellAx = attributesDict.ContainsKey("ax") ? attributesDict["ax"].AsInt32() : 0;
                            var cellAy = attributesDict.ContainsKey("ay") ? attributesDict["ay"].AsInt32() : 0;
                            var cellAw = attributesDict.ContainsKey("aw") ? attributesDict["aw"].AsInt32() : 0;
                            var cellAh = attributesDict.ContainsKey("ah") ? attributesDict["ah"].AsInt32() : 0;

                            var theCell = new CellEntry(null, _texturesDirPath, Path, _firstQuality, cellName,
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
                            if (attributesDict.ContainsKey("type"))
                            {
                                var sheetType = attributesDict["type"].AsString();
                                Debug.Assert(Enum.TryParse(sheetType.ToUpper(), out TextureType _));
                            }
                            for (var quality = _firstQuality; quality > TextureQuality.Invalid; quality--)
                            {
                                var filePath = _texturesDirPath + "/" + quality + "/" + sheetName + ".xml";
                                if (!FileAccess.FileExists(filePath))
                                    continue;

                                GD.Print("Found sprite info for " + filePath);

                                _children.Add(new SpriteInfo(sheetName, _texturesDirPath, filePath, quality, this));
                                break;
                            }

                            break;
                        }
                        case "FrameInformation":
                        {
                            var frameName = attributesDict["name"].AsString();
                            var texW = attributesDict["texw"].AsInt32();
                            var texH = attributesDict["texh"].AsInt32();
                            var type = attributesDict["type"].AsString();
                            Debug.Assert(Enum.TryParse(type.ToUpper(), out TextureType actualType));
                            currentFrame = new FrameInfo(this, _texturesDirPath, Path, _firstQuality, frameName, texW,
                                texH, actualType);
                            break;
                        }
                    }
                }
            }
        }
    }
}