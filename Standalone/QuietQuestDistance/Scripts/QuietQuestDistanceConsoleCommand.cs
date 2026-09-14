using System.Collections.Generic;

namespace QuietQuestDistance
{
    public sealed class QuietQuestDistanceConsoleCommand : ConsoleCmdAbstract
    {
        public override string[] getCommands()
        {
            return new[] { "qqd", "quietquestdistance" };
        }

        public override string getDescription()
        {
            return "Controls quest destination distance labels.";
        }

        public override string getHelp()
        {
            return "qqd hide | show | toggle | status | reload";
        }

        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            string action = _params.Count > 0 ? _params[0] : "status";
            string result = QuietQuestDistanceRuntime.Instance == null
                ? "Quiet Quest Distance: runtime is not ready."
                : QuietQuestDistanceRuntime.Instance.RunCommand(action);
            SdtdConsole.Instance.Output(result);
        }
    }
}
