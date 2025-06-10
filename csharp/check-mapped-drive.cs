using System;
using System.IO;
using System.Management;

public static class PathInspector
{
    public static bool IsMappedDriveOrUNC(string path, out string mappedDriveLetterOrUNC)
    {
        mappedDriveLetterOrUNC = null;

        if (string.IsNullOrWhiteSpace(path))
            return false;

        // Case 1: UNC path (normalized)
        if (path.StartsWith(@"\\"))
        {
            mappedDriveLetterOrUNC = path;
            return true;
        }

        // Case 2: Mapped drive (e.g., Z:\...)
        string root = Path.GetPathRoot(path);  // e.g., "Z:\"
        if (string.IsNullOrEmpty(root) || root.Length < 2)
            return false;

        string letter = root.Substring(0, 2);  // e.g., "Z:"

        try
        {
            using (var searcher = new ManagementObjectSearcher(
                "SELECT ProviderName, Name FROM Win32_LogicalDisk WHERE DriveType = 4"))
            {
                foreach (ManagementObject drive in searcher.Get())
                {
                    string name = drive["Name"]?.ToString();           // e.g., "Z:"
                    string provider = drive["ProviderName"]?.ToString(); // e.g., "\\server\share"

                    if (string.Equals(name, letter, StringComparison.OrdinalIgnoreCase))
                    {
                        mappedDriveLetterOrUNC = provider;
                        return true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error checking mapped drives: " + ex.Message);
        }

        return false;
    }
}
