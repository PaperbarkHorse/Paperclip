using System.Collections.Generic;

namespace Paperclip;

public class ModMetadata
{

    public ulong ModGUID;
    public int PaperclipVersion;
    public bool BundledByScriptMod = false;
    public List<ulong> DependencyGUIDs = new List<ulong>();

    public bool IsPaperclipMod()
    {
        return PaperclipVersion > 0 || BundledByScriptMod || DependencyGUIDs.Count > 0;
    }

    public void MarkAsPaperclipMod()
    {
        PaperclipVersion = PaperclipCore.META_VERSION;
    }

}