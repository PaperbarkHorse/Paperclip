using System.Collections.Generic;

namespace Paperclip;

public class ModMetadata
{

    public ulong ModGUID;
    public bool BundledByScriptMod = false;
    public List<ulong> RequiredDependencyGUIDs = new List<ulong>();
    public List<ulong> OptionalDependencyGUIDs = new List<ulong>();

    public bool IsPaperclipMod()
    {
        return BundledByScriptMod || RequiredDependencyGUIDs.Count > 0 || OptionalDependencyGUIDs.Count > 0;
    }

}