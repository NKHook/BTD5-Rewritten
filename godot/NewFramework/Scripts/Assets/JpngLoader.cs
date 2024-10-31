using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace BloonsTD5Rewritten.NewFramework.Scripts.Assets;

public partial class JpngLoader : Node
{
	private static readonly byte[] PngSignature = { 0x89, 0x50, 0x4E, 0x47 };
	private const int JpngJfifPtr = 0x1C;
	private const int JpngPngPtr = 0x10;
	private const int JfifSizeOffset = 2;
	
	public static async Task<Image?> LoadJpngTexture(string path, int width, int height)
	{
		return await Task.Run(() =>
		{
			var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
			Debug.Assert(file.IsOpen(), "Couldn't open JPNG texture: " + path);
			var length = file.GetLength();
			var buffer = file.GetBuffer((long)length);

			if (length >= 4 && buffer.Take(4).Equals(PngSignature))
			{
				GD.PrintErr("Tried to read a JPNG image from a PNG file! (" + path + ")");
				return null;
			}

			var jfifOffset = BitConverter.ToInt32(buffer, (int)(length - JpngJfifPtr));
			var pngOffset = BitConverter.ToInt32(buffer, (int)(length - JpngPngPtr));

			//All this just to read big endian jfif size
			var jfifSize = pngOffset;
			var pngSize = (int)length - pngOffset - JpngPngPtr;

			var jfifData = new ArraySegment<byte>(buffer, jfifOffset, jfifSize).ToArray();
			var pngData = new ArraySegment<byte>(buffer, pngOffset, pngSize).ToArray();

			var pngImage = Image.CreateEmpty(width, height, false, Image.Format.R8);
			Debug.Assert(Error.Ok == pngImage.LoadPngFromBuffer(pngData));
			Debug.Assert(pngImage.GetWidth() > 0, "PNG image data doesnt match! Assuming JPNG load failed");
			Debug.Assert(pngImage.GetHeight() > 0, "PNG image data doesnt match! Assuming JPNG load failed");
			var jfifImage = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);
			
			Debug.Assert(Error.Ok == jfifImage.LoadJpgFromBuffer(jfifData));
			Debug.Assert(jfifImage.GetWidth() > 0, "JFIF image data doesnt match! Assuming JPNG load failed");
			Debug.Assert(jfifImage.GetHeight() > 0, "JFIF image data doesnt match! Assuming JPNG load failed");

			var jpngImage = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);
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
		});
	}
}