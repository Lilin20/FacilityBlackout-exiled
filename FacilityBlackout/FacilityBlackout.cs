using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled;

namespace FacilityBlackout
{
    public class FacilityBlackout : Plugin<Config>
    {
        public static FacilityBlackout Instance { get; } = new FacilityBlackout();
        private FacilityBlackout() { }
        public override PluginPriority Priority { get; } = PluginPriority.Default;
        public EventHandlers EventHandlers;

        public override void OnEnabled()
        {
            EventHandlers = new EventHandlers();

            Exiled.Events.Handlers.Server.RoundStarted += EventHandlers.OnRoundStarted;
            Exiled.Events.Handlers.Map.Decontaminating += EventHandlers.OnDecontamination;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= EventHandlers.OnRoundStarted;
            Exiled.Events.Handlers.Map.Decontaminating -= EventHandlers.OnDecontamination;

            EventHandlers = null;

            base.OnDisabled();
        }
    }
}
