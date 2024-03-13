using Exiled.API.Enums;
using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;

namespace FacilityBlackout
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;

        [Description("Number of total blackouts per round.")]
        public int BlackoutAmount { get; set; } = 5;

        [Description("Additional delay between each blackout.")]
        public int BlackoutDelayBetween { get; set; } = 150;

        [Description("Delay before the first blackout occurs.")]
        public int BlackoutStartDelay { get; set; } = 10;

        [Description("Minimum delay until next blackout events.")]
        public int BlackoutMinDelay { get; set; } = 30;

        [Description("Maximum delay until next blackout events.")]
        public int BlackoutMaxDelay { get; set; } = 1200;

        [Description("Duration of each blackout event.")]
        public int BlackoutTime { get; set; } = 60;

        [Description("Cassie announcement during blackout.")]
        public string BlackoutCassie { get; set; } = "LIGHT SYSTEM MALFUNCTION . REPAIRING SYSTEMS";

        [Description("Translated Cassie text on the bottom left.")]
        public string BlackoutCassieTranslation { get; set; } = "Light System Malfunction. Repairing Systems...";

        [Description("If true, a random zone will be picked from the list. If false, all zones from this list will blackout.")]
        public bool BlackoutRandomZones { get; set; } = true; // If true, all zones from BlackoutZones will be used for random selection. If false, all zones in list will blackout.

        [Description("Amount of random zones for each full zone blackout.")]
        public int BlackoutRandomZonesAmount { get; set; } = 1;

        [Description("Zones affected by blackout.")]
        public List<ZoneType> BlackoutZones { get; set; } = new List<ZoneType> { ZoneType.Entrance, ZoneType.LightContainment, ZoneType.HeavyContainment, ZoneType.Surface };

        public bool BlackoutRooms { get; set; } = false;

        public int BlackoutRoomsAmount { get; set; } = 3;

        public bool BlackoutRoomsRandomZones { get; set; } = true;

        public int BlackoutRoomsRandomZonesAmount { get; set; } = 1;


    }
}
