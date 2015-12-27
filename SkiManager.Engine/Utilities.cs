using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace SkiManager.Engine
{
    public static class Utilities
    {
        /// <summary>
        /// Loads the file with the specified URI into a byte array.
        /// </summary>
        /// <param name="uri">File URI</param>
        /// <returns>Byte array</returns>
        public static async Task<byte[]> ReadBytesAsync(Uri uri)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            var buffer = await FileIO.ReadBufferAsync(file);
            var bytes = buffer.ToArray();
            return bytes;
        }

        /// <summary>
        /// Loads the specified file from the resource stream of the assembly.
        /// </summary>
        /// <param name="path">Path separated by dots</param>
        /// <returns>Byte array</returns>
        public static async Task<byte[]> ReadBytesFromEmbeddedResourceAsync(string path)
        {
            var assembly = typeof(Utilities).GetTypeInfo().Assembly;

            using (var fileStream = assembly.GetManifestResourceStream(path))
            {
                var bytes = new byte[fileStream.Length];
                await fileStream.ReadAsync(bytes, 0, (int)fileStream.Length);
                return bytes;
            }
        }

        public static Vector2 XZ(this Vector3 v)
            => new Vector2(v.X, v.Z);

        public static Vector2 Position(this Rect rect)
            => new Vector2((float)rect.X, (float)rect.Y);

        public static Vector2 Size(this Rect rect)
            => new Vector2((float)rect.Width, (float)rect.Height);
    }
}
