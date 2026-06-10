using Paperclip;

namespace Setting;

[GenerateSettingAPI]
public class PaperclipGlobal : SettingBase
{
    public bool ExampleGlobal;

    public override void OnCompiled()
    {
        PaperclipPlugin.Logger.LogDebug("PaperclipGlobal -> OnCompiled");
    }

    public override void Start()
    {
        PaperclipPlugin.Logger.LogDebug("PaperclipGlobal -> Start");
    }

    public override void Update()
    {
        // PaperclipPlugin.Logger.LogDebug("PaperclipGlobal -> Update");
    }

    public override void UpdateWhenDirty()
    {
        PaperclipPlugin.Logger.LogDebug("PaperclipGlobal -> UpdateWhenDirty");
    }

    public override void OnDestroy()
    {
        PaperclipPlugin.Logger.LogDebug("PaperclipGlobal -> OnDestroy");
    }
}