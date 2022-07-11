using System.Security.Cryptography;
using System.Text;
using Utility.Other.Enums;
using HashAlgorithm = Utility.Other.Enums.HashAlgorithm;

namespace Utility.Other.Extensions;

public static class ByteArrayExtension
{
    /// <summary>
    /// Validates if a byte array is an image
    /// </summary>
    /// <param name="fileBytes"></param>
    /// <returns></returns>
    public static bool IsImage(this byte[] fileBytes, ImageType allowedImages = ImageType.All)
    {
        var headers = new List<byte[]>();
        
        if(allowedImages.HasFlag(ImageType.Bmp)) headers.Add(Encoding.ASCII.GetBytes("BM"));
        if(allowedImages.HasFlag(ImageType.Gif)) headers.Add(Encoding.ASCII.GetBytes("GIF"));
        if(allowedImages.HasFlag(ImageType.Png)) headers.Add(new byte[] { 137, 80, 78, 71 });
        if (allowedImages.HasFlag(ImageType.Tiff))
        {
            headers.Add(new byte[] { 73, 73, 42 });
            headers.Add(new byte[] { 77, 77, 42 });
        }
        if(allowedImages.HasFlag(ImageType.Jpeg)) headers.Add(new byte[] { 255, 216, 255, 224 });
        if(allowedImages.HasFlag(ImageType.JpegCanon)) headers.Add(new byte[] { 255, 216, 255, 225 });

        return headers.Any(x => x.SequenceEqual(fileBytes.Take(x.Length)));
    }

    /// <summary>
    /// gets the MD5 Checksum from a byte array
    /// </summary>
    /// <param name="inputData"></param>
    /// <param name="algorithm"></param>
    /// <returns></returns>
    public static string GetChecksum(this byte[] inputData, HashAlgorithm algorithm)
    {
        //convert byte array to stream
        var stream = new MemoryStream();
        stream.Write(inputData, 0, inputData.Length);

        //important: get back to start of stream
        stream.Seek(0, SeekOrigin.Begin);

        System.Security.Cryptography.HashAlgorithm instance = null;
        
        switch (algorithm)
        {
            case HashAlgorithm.HmacSha256:
                break;
            case HashAlgorithm.HmacSha384:
                break;
            case HashAlgorithm.HmacSha512:
                break;
            case HashAlgorithm.HmacMd5:
                break;
            case HashAlgorithm.Md5:
                instance = System.Security.Cryptography.MD5.Create();
                break;
            case HashAlgorithm.Sha1:
                instance = System.Security.Cryptography.SHA1.Create();
                break;
            case HashAlgorithm.Sha256:
                instance = System.Security.Cryptography.SHA256.Create();
                break;
            case HashAlgorithm.Sha384:
                instance = System.Security.Cryptography.SHA384.Create();
                break;
            case HashAlgorithm.Sha512:
                instance = System.Security.Cryptography.SHA512.Create();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, null);
        }
        
        var hashResult = instance!.ComputeHash(stream);
        instance.Dispose();

        //***I did some formatting here, you may not want to remove the dashes, or use lower case depending on your application
        return BitConverter.ToString(hashResult).Replace("-", "").ToLower();
    }
}