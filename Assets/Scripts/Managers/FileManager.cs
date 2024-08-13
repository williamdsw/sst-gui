using System;
using System.IO;
using System.Text;

namespace Managers
{
    /// <summary>
    /// Manager for File operations
    /// </summary>
    public class FileManager
    {
        public static string GetPathSubstring(string path)
        {
            try
            {
                return path[..path.LastIndexOf("\\")];
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetFolder(string path) => string.Concat("file:///", GetPathSubstring(path));


        public static string GetSlotFinalPath(string slotName) => Path.Combine(Directory.GetCurrentDirectory(), slotName);

        public static bool Exists(string path) => File.Exists(path);

        public static bool Copy(string source, string destination)
        {
            try
            {
                File.Copy(source, destination);
                return Exists(destination);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool Move(string source, string destination)
        {
            try
            {
                if (Exists(destination))
                {
                    File.Delete(destination);
                }

                File.Move(source, destination);
                return Exists(destination);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string ReadAllText(string path)
        {
            try
            {
                return File.ReadAllText(path);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void WriteAllText(string path, string contents)
        {
            try
            {
                File.WriteAllText(path, contents);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private static byte[] GetDataFromFileAtOffset(string path, int offset, int count)
        {
            try
            {
                byte[] data = new byte[16];

                using FileStream stream = new(path, FileMode.Open);
                using BinaryReader reader = new(stream);
                reader.BaseStream.Seek(offset, SeekOrigin.Begin);
                reader.Read(data, 0, count);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetSystemInFile(string path)
        {
            try
            {
                return Encoding.UTF8.GetString(GetDataFromFileAtOffset(path, 16, 16));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetIdVersionInFile(string path)
        {
            try
            {
                return Encoding.UTF8.GetString(GetDataFromFileAtOffset(path, 48, 16));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sarooPath"></param>
        /// <returns></returns>
        public static byte[] Test(string sarooPath)
        {
            try
            {
                using FileStream stream = new(sarooPath, FileMode.Open);
                using BinaryReader reader = new(stream);
                return reader.ReadBytes((int)stream.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sarooPath"></param>
        /// <returns></returns>
        public static void Test2(string sarooPath, int indexOfLastName, byte[] newGame)
        {
            try
            {
                UnityEngine.Debug.Log(indexOfLastName + 16);
                UnityEngine.Debug.Log(newGame.Length);
                using FileStream stream = new(sarooPath, FileMode.Open, FileAccess.ReadWrite);
                stream.Position = indexOfLastName + 16;
                stream.Write(newGame, 0, newGame.Length);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int Search(byte[] src, byte[] pattern)
        {
            int maxFirstCharSlot = src.Length - pattern.Length + 1;
            for (int i = 0; i < maxFirstCharSlot; i++)
            {
                if (src[i] != pattern[0]) // compare only first byte
                    continue;

                // found a match on first byte, now try to match rest of the pattern
                for (int j = pattern.Length - 1; j >= 1; j--)
                {
                    if (src[i + j] != pattern[j]) break;
                    if (j == 1) return i;
                }
            }
            return -1;
        }


    }
}