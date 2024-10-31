using System.IO;
using BloonsTD5Rewritten.NewFramework.Scripts.Assets;
using Godot;

namespace BloonsTD5Rewritten.Scripts.Level;

public class MapMask
{
	private static byte BITMASK = 0b111;
	public static MaskBit FromByte(int data)
	{
		MaskBit result = 0;
		var bits = data & BITMASK;
		
		if ((bits & 0b100) != 0)
		{
			result |= MaskBit.Water;
		}

		if ((bits & 0b010) != 0)
		{
			result |= MaskBit.PathTower;
		}

		if ((bits & 0b001) != 0)
		{
			result |= MaskBit.BlockTower;
		}

		return result;
	}

	private readonly int _width;
	private readonly int _height;
	private readonly MaskBit[] _maskBuffer;

	public Vector2 Size => new(_width, _height);
	
	public MapMask(int width, int height)
	{
		_width = width;
		_height = height;
		_maskBuffer = new MaskBit[width * height];
	}

	public void SetPixel(int x, int y, MaskBit mask)
	{
		_maskBuffer[y * _width + x] = mask;
	}

	//This function expects a pixel at the base resolution, usually 480x320
	public MaskBit GetPixel(int x, int y)
	{
		return _maskBuffer[y * _width + x];
	}

	//A function to check if the mask has a pixel
	public bool HasPixel(int x, int y)
	{
		if (x < 0 || x > _width)
			return false;
		if (y < 0 || y > _height)
			return false;
		var index = y * _width + x;
		return _maskBuffer.Length > index;
	}

	//Ultra texture quality is the base quality for this project, so this function gets the pixel at ultra quality
	//This is usually a pixel at the resolution 1920x1280
	public MaskBit GetPixelUltra(int x, int y) => GetPixel(x / 4, y / 4);
	public bool HasPixelUltra(int x, int y) => HasPixel(x / 4, y / 4);
	

	public Image CreateImageForMask()
	{
		var image = Image.CreateEmpty(_width, _height, false, Image.Format.Rgbaf);
		var total = 0;
		foreach (var mask in _maskBuffer)
		{
			var color = Colors.Black;
			if ((mask & MaskBit.BlockTower) != 0)
			{
				color.R = 1.0f;
			}

			if ((mask & MaskBit.PathTower) != 0)
			{
				color.G = 1.0f;
			}

			if ((mask & MaskBit.Water) != 0)
			{
				color.B = 1.0f;
			}

			var x = total % _width;
			var y = total / _width;
			
			image.SetPixel(x, y, color);
			total += 1;
		}

		return image;
	}
	
	public static MapMask LoadFromFile(string filePath)
	{
		var maskFile = JetFileImporter.Instance().GetFileStream(filePath);
		var reader = new BinaryReader(maskFile);

		var width = reader.ReadInt32();
		var height = reader.ReadInt32();

		var resultMask = new MapMask(width, height);
		
		var total = 0;
		while (reader.BaseStream.Position != reader.BaseStream.Length)
		{
			var compressionNode = reader.ReadInt16();
			var length = compressionNode & 0xFF;
			var maskPair = (byte)((compressionNode >> 8) & 0xFF);

			var extractedPair = new []
			{
				FromByte(maskPair & BITMASK), // Get the first mask
				FromByte((maskPair >> 4) & BITMASK) // Get the second mask
			};

			//Repeat the masks in the pair for the specified length
			for (var i = 0; i < length; i++)
			{
				for (var j = 0; j < 2; j++)
				{
					var x = total % width;
					var y = total / width;
				
					resultMask.SetPixel(x, y, extractedPair[j]);
				
					total++;
				}
			}
		}

		return resultMask;
	}
}