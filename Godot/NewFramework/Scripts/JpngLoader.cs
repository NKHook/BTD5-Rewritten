using System;
using System.Linq;
using BloonsTD5Custom.Godot.Scripts;
using Godot;

namespace BloonsTD5Rewritten.Godot.NewFramework.Scripts;

public partial class JpngLoader : Node
{
	private static readonly byte[] PNG_SIGNATURE = { 0x89, 0x50, 0x4E, 0x47 };
	private const int JPNG_JFIF_PTR = 0x1C;
	private const int JPNG_PNG_PTR = 0x10;
	private const int JFIF_SIZE_OFFSET = 2;
	
	public static Image LoadJpngTexture(string path, int width, int height)
	{
		var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		Debug.Assert(file.IsOpen(), "Couldn't open JPNG texture: " + path);
		var length = file.GetLength();
		var buffer = file.GetBuffer((long)length);

		if (length >= 4 && buffer.Take(4).Equals(PNG_SIGNATURE))
		{
			GD.PrintErr("Tried to read a JPNG image from a PNG file! (" + path + ")");
			return null;
		}
		
		var jfifOffset = BitConverter.ToInt32(buffer, (int)(length - JPNG_JFIF_PTR));
		var pngOffset = BitConverter.ToInt32(buffer, (int)(length - JPNG_PNG_PTR));

		//All this just to read big endian jfif size
		var jfifSize = pngOffset;
		var pngSize = (int)length - pngOffset - JPNG_PNG_PTR;
		
		var jfifData = new ArraySegment<byte>(buffer, jfifOffset, jfifSize).ToArray();
		var pngData = new ArraySegment<byte>(buffer, pngOffset, pngSize).ToArray();

		var pngImage = Image.Create(width, height, false, Image.Format.R8);
		Debug.Assert(Error.Ok == pngImage.LoadPngFromBuffer(pngData));
		Debug.Assert(pngImage.GetWidth() > 0, "PNG image data doesnt match! Assuming JPNG load failed");
		Debug.Assert(pngImage.GetHeight() > 0, "PNG image data doesnt match! Assuming JPNG load failed");
		var jfifImage = Image.Create(width, height, false, Image.Format.Rgba8);;
		Debug.Assert(Error.Ok == jfifImage.LoadJpgFromBuffer(jfifData));
		Debug.Assert(jfifImage.GetWidth() > 0, "JFIF image data doesnt match! Assuming JPNG load failed");
		Debug.Assert(jfifImage.GetHeight() > 0, "JFIF image data doesnt match! Assuming JPNG load failed");
		
		var jpngImage = Image.Create(width, height, false, Image.Format.Rgba8);
		Debug.Assert(jpngImage.GetWidth() > 0, "JPNG image data doesnt match! Likely programmer skill issue");
		Debug.Assert(jpngImage.GetHeight() > 0, "JPNG image data doesnt match! Likely programmer skill issue");

		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				var jfifColor = jfifImage.GetPixel(x, y);
				var pngColor = pngImage.GetPixel(x, y);
				jpngImage.SetPixel(x, y, new Color(jfifColor.R, jfifColor.G, jfifColor.B, pngColor.R));
			}
		}

		return jpngImage;
	}
}