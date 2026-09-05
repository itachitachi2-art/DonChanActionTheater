using System.Collections.Generic;

namespace DonChanActionTheater
{
    public sealed class ActionTheaterConsoleCommand : ConsoleCmdAbstract
    {
        public override string[] getCommands() { return new[] { "dat", "donaction" }; }
        public override string getDescription() { return "Controls Don-chan Action Theater."; }
        public override string getHelp() { return "dat on | off | reload | test food|healing|craft|melee [type]|ranged [type]|vehicle [type]"; }

        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            string action = _params.Count > 0 ? string.Join(" ", _params.ToArray()) : "help";
            string result = ActionTheaterRuntime.Instance == null
                ? "Don-chan Action Theater: runtime is not ready."
                : ActionTheaterRuntime.Instance.RunCommand(action);
            SdtdConsole.Instance.Output(result);
        }
    }
}
