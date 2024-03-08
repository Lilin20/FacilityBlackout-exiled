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
| BlackoutZones | list | Entrance, LightContainment, HeavyContainment, Surface | Zones affected by blackout. |

# Config Example
```yml
  is_enabled: true
  debug: false
  blackout_amount: 3
  blackout_delay_between: 120
  blackout_start_delay: 60
  blackout_min_delay: 120
  blackout_max_delay: 300
  blackout_time: 20
  blackout_cassie: 'LIGHT SYSTEM MALFUNCTION . REPAIRING SYSTEMS'
  blackout_zones:
  - Entrance
  - LightContainment
  - HeavyContainment
  - Surface
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
- Enable the use of static zones.
- Make it possible that more than 1 random zone can be chosen by the plugin (example: Heavy and Light gets chosen randomly).
- Maybe add some red light before the blackout occurs? idk.
