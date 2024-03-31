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
        public override Version Version { get; } = new Version(1, 0, 3);
        public override Version RequiredExiledVersion => new Version(8, 8, 0);
        public override string Prefix { get; } = "FacilityBlackout";
        public override PluginPriority Priority { get; } = PluginPriority.Default;
        public static FacilityBlackout Singleton;



        public static FacilityBlackout Instance { get; } = new FacilityBlackout();
        private FacilityBlackout() { }
        public EventHandlers EventHandlers;

        public override void OnEnabled()
        {
            Singleton = this;
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
            Singleton = null;
        }
    }
}
