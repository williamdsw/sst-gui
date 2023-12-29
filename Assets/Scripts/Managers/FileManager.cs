using System;
using System.IO;

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
    }
}