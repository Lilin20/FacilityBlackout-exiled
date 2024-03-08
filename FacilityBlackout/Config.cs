using Exiled.API.Enums;
using Exiled.API.Interfaces;
using System.Collections.Generic;

namespace FacilityBlackout
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        public int BlackoutAmount { get; set; } = 3;
        public int BlackoutDelayBetween { get; set; } = 120;
        public int BlackoutStartDelay { get; set; } = 60;
        public int BlackoutMinDelay { get; set; } = 120;
        public int BlackoutMaxDelay { get; set; } = 300;
        public int BlackoutTime { get; set; } = 20;
        public string BlackoutCassie { get; set; } = "LIGHT SYSTEM MALFUNCTION . REPAIRING SYSTEMS";
        public List<ZoneType> BlackoutZones { get; set; } = new List<ZoneType> { ZoneType.Entrance, ZoneType.LightContainment, ZoneType.HeavyContainment, ZoneType.Surface };
    }
}
