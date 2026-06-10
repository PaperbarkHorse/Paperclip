using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Paperclip;

public class PaperclipUtils
{

    public static string SerializeGUIDList(List<ulong> guids)
    {
        if (guids.Count == 0) return "";
        return string.Join(",", guids.Select(guid => guid.ToString()));
    }

    public static List<ulong> DeserializeGUIDList(string value)
    {
        if (string.IsNullOrEmpty(value)) return new List<ulong>();
        return value.Split(',').Select(guid => ulong.Parse(guid)).ToList();
    }

}
