namespace Utility.Other.Enums;

[Flags]
public enum ImageType
{
    None = 0,
    Bmp = 1 << 0,
    Gif = 1 << 1,
    Png = 1 << 2,
    Tiff = 1 << 3,
    Jpeg = 1 << 4,
    JpegCanon = 1 << 5,
    All = 63
}