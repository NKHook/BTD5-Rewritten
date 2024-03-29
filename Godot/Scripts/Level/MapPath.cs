using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Assets;
using BloonsTD5Rewritten.Godot.NewFramework.Scripts.Sprites;
using Godot;

namespace BloonsTD5Rewritten.Godot.Scripts.Level;

public partial class MapPath : Node2D
{
    [Export] public string PathFile = "";

    [Flags]
    enum SegmentFlags
    {
        None = 0,
        Entrance = 1 << 0,
        Exit = 1 << 1,
        TunnelHide = 1 << 2,
        TunnelShow = 1 << 3
    }

    class PathSegment
    {
        public int Index;
        public SegmentFlags Flags;
        public List<Vector2> Points;

        public PathSegment(int index, SegmentFlags flags, List<Vector2> points)
        {
            Index = index;
            Flags = flags;
            Points = points;
        }
    }

    [Flags]
    enum LinkFlags
    {
        None = 0,
        Linear = 1 << 0,
        Tunnel = 1 << 1,
        Instant = 1 << 2,
    }

    class PathLink
    {
        public int Start { get; set; }
        public int End { get; set; }
        public int Flags { get; set; }

        public static PathLink FromJson(JsonWrapper elem)
        {
            var result = new PathLink();
            result.Start = elem["Start"];
            result.End = elem["End"];
            result.Flags = elem["Flags"];
            return result;
        }
    }

    private static PathSegment GeneratePathSegment(int index, JsonWrapper nodeJson)
    {
        var flags = nodeJson["Flags"].EnumValue<SegmentFlags>();
        var points = nodeJson["Points"].EnumerateArray().Select(pointJson => pointJson.GetVector2() * 4.0f).ToList();
        return new PathSegment(index, flags, points);
    }

    private static List<PathSegment> GenerateSegments(JsonWrapper document)
    {
        return document["Nodes"].EnumerateArray()
            .Select((segmentJson, i) => GeneratePathSegment(i, segmentJson)).ToList();
    }

    private static List<PathLink> GenerateLinks(JsonWrapper document)
    {
        return document["Links"].EnumerateArray().Select(linkJson => PathLink.FromJson(linkJson)).ToList();
    }

    private static List<List<PathSegment>> LinkPaths(List<PathLink> links, List<PathSegment> segmentTable)
    {
        List<List<PathSegment>> linkedPaths = new();

        //Search the links first to find the entrances
        foreach (var link in links)
        {
            var startSegment = segmentTable[link.Start];
            var endSegment = segmentTable[link.End];

            if ((startSegment.Flags & SegmentFlags.Entrance) != 0)
            {
                linkedPaths.Add(new List<PathSegment> { startSegment });
            }
        }

        //For each enterance, link segments until there are no longer any segments to be linked
        for (var i = 0; i < linkedPaths.Count; i++)
        {
            var linkedSegments = linkedPaths[i];
            var lastSegment = linkedSegments.Last();

            while ((lastSegment.Flags & SegmentFlags.Exit) == 0)
            {
                foreach (var link in links)
                {
                    if (link.Start == lastSegment.Index)
                    {
                        linkedSegments.Add(segmentTable[link.End]);
                    }
                }

                lastSegment = linkedSegments.Last();
            }

            linkedPaths[i] = linkedSegments;
        }

        return linkedPaths;
    }

    public void Initialize()
    {
        var jsonElem = JetFileImporter.Instance().GetJsonParsed(PathFile);
        var links = GenerateLinks(jsonElem);
        var segments = GenerateSegments(jsonElem);
        var linked = LinkPaths(links, segments);

        var i = 0;
        foreach (var link in linked)
        {
            var path = new Path2D();
            path.Curve = new Curve2D();

            foreach (var points in link.Select(segment => segment.Points))
            {
                foreach (var point in points)
                {
                    path.Curve.AddPoint(point);
                }
            }

            path.Name = "path_" + i;
            AddChild(path);
            i++;
        }
    }

    public override void _Ready()
    {
        if (PathFile == string.Empty)
            return;

        Initialize();
    }
}