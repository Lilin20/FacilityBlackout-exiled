using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled;
using System;
using Exiled.API.Features.Core.Generic;

namespace FacilityBlackout
{
    public class FacilityBlackout : Plugin<Config>
    {
        public override string Name { get; } = "Facility Blackout";
        public override string Author { get; } = "Lilin";
        public override Version Version { get; } = new Version(1, 0, 8);
        public override string Prefix { get; } = "FacilityBlackout";
        public override PluginPriority Priority { get; } = PluginPriority.High;
        public static FacilityBlackout Singleton;



        public static FacilityBlackout Instance { get; } = new FacilityBlackout();
        private FacilityBlackout() { }
        public EventHandlers EventHandlers;

        public override void OnEnabled()
        {
            Singleton = this;
            EventHandlers = new EventHandlers();

            Log.Debug("Loaded succesfully... Registering EventHandlers...");

            Exiled.Events.Handlers.Server.RoundStarted += EventHandlers.OnRoundStarted;
            Exiled.Events.Handlers.Map.Decontaminating += EventHandlers.OnDecontamination;
            Exiled.Events.Handlers.Server.RestartingRound += EventHandlers.OnRoundRestart;

            Log.Debug("Done.");

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= EventHandlers.OnRoundStarted;
            Exiled.Events.Handlers.Map.Decontaminating -= EventHandlers.OnDecontamination;
            Exiled.Events.Handlers.Server.RestartingRound -= EventHandlers.OnRoundRestart;

            EventHandlers = null;
            Singleton = null;

            base.OnDisabled();
        }
    }
}
