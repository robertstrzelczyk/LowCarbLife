using Microsoft.AspNetCore.SignalR;

namespace LowCarbLife.Api.Features.Assistant;

public class AssistantHub : Hub
{
    public async Task SendMessage(string text, string language)
    {
        var reply = AssistantResponder.Reply(text, language);
        await Task.Delay(350);
        await Clients.Caller.SendAsync("ReceiveMessage", reply);
    }
}
