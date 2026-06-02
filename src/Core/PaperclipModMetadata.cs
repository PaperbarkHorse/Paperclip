using System.Collections.Generic;

namespace Paperclip.Core;

public class PaperclipModMetadata
{

    public ulong ModGUID;
    public bool BundledByScriptMod = false;
    public List<ulong> RequiredDependencyGUIDs = new List<ulong>();
    public List<ulong> OptionalDependencyGUIDs = new List<ulong>();

}