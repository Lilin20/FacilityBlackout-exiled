# FacilityBlackout-exiled
A plugin for EXILED to create timed blackouts. This is my very first real plugin creation (a plugin with an actual feature), designed to add an element of unpredictability and immersion. Pleaase bear with me as I continue learning and refining my skills. If you have any suggestions, whether it's for improving the plugin or enhancing overall functionality, don't hestitate to share them with me.

# Features
- Blackouts in random intervals. The interval is customizable.
- Wait x seconds when a round starts to delay the blackout mechanism (so it won't call a blackout after 2-3 minutes when the round starts).
- Blackouts in random zones (LCZ, HZC, Entrance, Surface).
- LCZ decontamination check (so no blackout will happen in LCZ when its decontaminated).
- Customizable amount of blackouts during a round.
- Customizable blackout duration.

# Config Variables
| Config | Type   | Default | Info | 
| :---------------- | :------: | :----: | -----: |
| IsEnabled | bool | true | Enable / Disable plugin. |
| Debug | bool | true | Enable debugging. |
| BlackoutAmount | integer | 3 | Number of total blackouts per round. |
| BlackoutDelayBetween | integer | 120 | Additional delay between each blackout. |
| BlackoutStartDelay | integer | 300 | Delay before the first blackout occurs. |
| BlackoutMinDelay | integer | 120 | Minimum delay between blackout events. |
| BlackoutMaxDelay | integer | 300 | Maximum delay between blackout events. |
| BlackoutTime | integer | 20 | Duration of each blackout event. |
| BlackoutCassie | string | "LIGHT SYSTEM MALFUNCTION . REPAIRING SYSTEMS" | Cassie announcement during blackout. |
| BlackoutRandomZones | bool | true | If true, a random zone will be picked from the list. If false, all zones from this list will blackout. |
| BlackoutRandomZonesAmount | int | 1 | Amount of random zones for each full zone blackout. |
| BlackoutZones | list | Entrance, LightContainment, HeavyContainment, Surface | Zones affected by blackout. |
| BlackoutRoom | bool | false | Enables the blackout functionality for rooms at round start. | 
| BlackoutRoomsAmount | int | 3 | Amount of rooms to be blacked out at round start. |
| BlackoutRoomsRandomZones | bool | false | Enables random zones for rooms. Takes BlackoutZones to get random zones to fetch rooms. |
| BlackoutRoomsRandomZonesAmount | int | 1 | Amount of random zones for random blacked out rooms. |
| BlackoutOnRoundStart | bool | false | Enables the blackout on round start functionality. |
| BlackoutZonesOnRoundStart | list | Entrance, LightContainment, HeavyContainment, Surface | Zones affected by round start blackout. |
| BlackoutOnRoundStartDuration | int | 15 | Duration of the blackout at round start. |

# Config Example
```yml
facility_blackout:
  is_enabled: true
  debug: false
  # Number of total blackouts per round.
  blackout_amount: 5
  # Additional delay between each blackout.
  blackout_delay_between: 150
  # Delay before the first blackout occurs.
  blackout_start_delay: 10
  # Minimum delay until next blackout events.
  blackout_min_delay: 30
  # Maximum delay until next blackout events.
  blackout_max_delay: 1200
  # Duration of each blackout event.
  blackout_time: 60
  # Cassie announcement during blackout.
  blackout_cassie: 'LIGHT SYSTEM MALFUNCTION . REPAIRING SYSTEMS'
  # If true, a random zone will be picked from the list. If false, all zones from this list will blackout.
  blackout_random_zones: true
  # Does not work in this release.
  blackout_random_zones_amount: 1
  # Zones affected by blackout.
  blackout_zones:
  - Entrance
  - LightContainment
  - HeavyContainment
  - Surface
  blackout_rooms: false
  blackout_rooms_amount: 3
  blackout_rooms_random_zones: true
  blackout_rooms_random_zones_amount: 2
```
# How does it work now?
- At the start of a round, the plugin first waits. The amount of time you have to wait depends on the "blackout_start_delay" value.
- The plugin initiates a blackout event where lights are turned off.
- It selects a random delay between blackout events within the specified range (MinDelay and MaxDelay).
- It randomly selects a zone affected by the blackout from the configured list of blackout zones.
- The plugin calculates the duration for the Cassie announcement during blackout.
- If it's not the first blackout event, it waits for the delay between blackout events before proceeding (blackout_delay_between).
- It then triggers the Cassie announcement and turns off all lights in the selected zone for the specified blackout time.
- This process continues until the desired number of blackout events is reached.

# Upcoming Features
- Random interval for the blackout duration.
- Random interval for the seconds between each blackout event.
- Enable/Disable Cassie Broadcast for the blackout event.
- Make it possible that more than 1 random zone can be chosen by the plugin (example: Heavy and Light gets chosen randomly).
- Maybe add some red light before the blackout occurs? idk.
