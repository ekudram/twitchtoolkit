using ToolkitCore;
using TwitchLib.Client.Models;
using TwitchToolkitDev;

namespace TwitchToolkit.Commands.ModCommands;

public class RefreshViewers : CommandDriver
{
	public override void RunCommand(ChatMessage ChatMessage)
	{
		WebRequest_BeginGetResponse.Main("https://tmi.twitch.tv/group/user/" + ToolkitCoreSettings.channel_username.ToLower() + "/chatters", Viewers.SaveUsernamesFromJsonResponse);
		TwitchWrapper.SendChatMessage("@" + ChatMessage.Username + " viewers have been refreshed.");
	}
}
