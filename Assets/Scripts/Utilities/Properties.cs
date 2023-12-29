using System.IO;
using System.Security.Claims;
using UnityEngine;

namespace Utilities
{
    public class Properties
    {
        // /// <summary>
        // /// Configuration path
        // /// </summary>
        // public static string ConfigurationPath => Path.Combine(Application.persistentDataPath, "Configuration.json");

        // /// <summary>
        // /// Database file name
        // /// </summary>
        // public static string DatabaseName => "biozahard.s3db";

        /// <summary>
        /// Database file name
        /// </summary>
        public static string SstName => "sst.exe";

        // /// <summary>
        // /// Copied database path
        // /// </summary>
        // public static string DatabasePath => Path.Combine(Application.persistentDataPath, DatabaseName);

        /// <summary>
        /// Copied database path
        /// </summary>
        public static string SstPath => Path.Combine(Application.persistentDataPath, SstName);

        // /// <summary>
        // /// Local Streaming Assets folder path
        // /// </summary>
        // public static string DatabaseStreamingAssetsPath => Path.Combine(Application.streamingAssetsPath, DatabaseName);

        /// <summary>
        /// Local Streaming Assets folder path
        /// </summary>
        public static string SstStreamingAssetsPath => Path.Combine(Application.streamingAssetsPath, SstName);

        // /// <summary>
        // /// Save progress path
        // /// </summary>
        // public static string ProgressPath => Path.Combine(Application.persistentDataPath, "Progress{0}.dat");




        public class FixedStrings
        {
            public static string Bin => "bin";
            public static string Save => "Save_";
            public static string Slot => "Slot_";

        }
    }
}