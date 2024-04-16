using Exiled.API.Features;
using Exiled.API.Enums;
using System.Collections.Generic;
using PlayerRoles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Map;
using System.Globalization;
using MEC;
using System;
using UnityEngine;
using System.Diagnostics.Eventing.Reader;


namespace FacilityBlackout
{
    public class EventHandlers
    {
        private readonly Config _config;
        private int _blackoutCount = 0;
        private bool _firstBlackoutFired = false;
        private CoroutineHandle _coroutine;

        public bool CheckCassieSpeaking()
        {
            return Cassie.IsSpeaking;
        }

        public DateTime GetTimestamp()
        {
            DateTime currentTime = DateTime.Now;
            return currentTime.ToUniversalTime();
        }

        public void OnRoundRestart()
        {
            Log.Debug("Killing coroutine...");
            Timing.KillCoroutines(_coroutine);
        }

        public void OnRoundStarted()
        {
            Log.Debug("Detected a round start... Trying to execute OnRoundStarted function...");

            if (FacilityBlackout.Singleton.Config.BlackoutOnRoundStart)
            {
                foreach (ZoneType zone in FacilityBlackout.Singleton.Config.BlackoutZonesOnRoundStart)
                {
                    Map.TurnOffAllLights(FacilityBlackout.Singleton.Config.BlackoutOnRoundStartDuration, zone);
                }
            }

            if (!FacilityBlackout.Singleton.Config.BlackoutEnabled)
                return;

            if (FacilityBlackout.Singleton.Config.BlackoutRooms)
            {
                if (FacilityBlackout.Singleton.Config.BlackoutRoomsRandomZones)
                {
                    HashSet<ZoneType> chosenZones = new HashSet<ZoneType>();
                    System.Random random = new System.Random();

                    while (chosenZones.Count < FacilityBlackout.Singleton.Config.BlackoutRoomsRandomZonesAmount) // FacilityBlackout.Instance.Config.BlackoutRoomsRandomZonesAmount
                    {
                        ZoneType randomZone = GetRandomZoneExcludingSurface();
                        chosenZones.Add(randomZone);
                    }

                    foreach (ZoneType zone in chosenZones)
                    {
                        int blackoutRoomsAmount = FacilityBlackout.Singleton.Config.BlackoutRoomsAmount; // Move inside the loop FacilityBlackout.Instance.Config.BlackoutRoomsAmount
                        Log.Debug($"Blackouting rooms in zone: {zone} - Amount: {blackoutRoomsAmount}");
                        BlackoutRoomsInZone(zone, blackoutRoomsAmount);
                    }
                }
                else
                {
                    foreach (ZoneType zone in FacilityBlackout.Singleton.Config.BlackoutZones)
                    {
                        int blackoutRoomsAmount = FacilityBlackout.Singleton.Config.BlackoutRoomsAmount; // Move inside the loop
                        Log.Debug($"Blackouting rooms in zone: {zone} - Amount: {blackoutRoomsAmount}");
                        BlackoutRoomsInZone(zone, blackoutRoomsAmount);
                    }
                }
            }

            _blackoutCount = 0;
            Log.Debug($"Waiting initial delay before blackout coroutine starts... {FacilityBlackout.Singleton.Config.BlackoutStartDelay}");
            Timing.CallDelayed(FacilityBlackout.Singleton.Config.BlackoutStartDelay, () =>
            {
                _coroutine = Timing.RunCoroutine(BlackoutCoroutine());
            });
        }

        public void OnDecontamination(DecontaminatingEventArgs ev)
        {
            if (!FacilityBlackout.Singleton.Config.BlackoutEnabled)
                return;
            FacilityBlackout.Singleton.Config.BlackoutZones.Remove(ZoneType.LightContainment); // Needs to get changed asap. Bad implementation.
        }

        public IEnumerator<float> BlackoutCoroutine()
        {
            Log.Debug($"Waiting for initial delay ended. Running coroutine...");
            while (_blackoutCount <= FacilityBlackout.Singleton.Config.BlackoutAmount -1)
            {
                System.Random random = new System.Random();
                int randomAdditionalDelay = random.Next(FacilityBlackout.Singleton.Config.BlackoutMinDelay, FacilityBlackout.Singleton.Config.BlackoutMaxDelay);
                int zoneIndex = FacilityBlackout.Singleton.Config.BlackoutRandomZones ? random.Next(FacilityBlackout.Singleton.Config.BlackoutZones.Count) : 0;
                float cassieTime = Cassie.CalculateDuration(FacilityBlackout.Singleton.Config.BlackoutCassie, false, 1);

                if (_firstBlackoutFired)
                {
                    Log.Debug("Delaying additional blackout using the BlackoutDelayBetween config element.");
                    yield return Timing.WaitForSeconds(FacilityBlackout.Singleton.Config.BlackoutDelayBetween);
                    Log.Debug($"Waited for {FacilityBlackout.Singleton.Config.BlackoutDelayBetween} seconds... Proceeding with next random blackout delay...");

                    Log.Debug($"Additional Blackouts: Additional Delay: {randomAdditionalDelay} until Blackout");
                    yield return Timing.WaitForSeconds(randomAdditionalDelay);

                    Log.Debug($"Starting Blackout...");
                    Cassie.MessageTranslated(FacilityBlackout.Singleton.Config.BlackoutCassie, FacilityBlackout.Singleton.Config.BlackoutCassieTranslation, false, false, true);
                    yield return Timing.WaitForSeconds(cassieTime);

                    if (FacilityBlackout.Singleton.Config.BlackoutRandomZones)
                    {
                        TurnOffRandomBlackoutZones();
                    }
                    else
                    {
                        TurnOffAllZones();
                    }
                }
                else
                {
                    Log.Debug($"First Blackout: Additional Delay: {randomAdditionalDelay} until Blackout");
                    yield return Timing.WaitForSeconds(randomAdditionalDelay);

                    Cassie.MessageTranslated(FacilityBlackout.Singleton.Config.BlackoutCassie, FacilityBlackout.Singleton.Config.BlackoutCassieTranslation, false, false, true);
                    yield return Timing.WaitForSeconds(cassieTime);

                    Log.Debug($"Starting Blackout...");
                    if (FacilityBlackout.Singleton.Config.BlackoutRandomZones)
                    {
                        TurnOffRandomBlackoutZones();
                    }
                    else
                    {
                        TurnOffAllZones();
                    }
                    _firstBlackoutFired = true;
                }

                _blackoutCount++;
            }
        }

        private void TurnOffRandomBlackoutZones()
        {
            Log.Debug("Trying to execute TurnOffRandomBlackoutZones...");
            if (!FacilityBlackout.Singleton.Config.BlackoutEnabled)
                return;
            if (FacilityBlackout.Singleton.Config.BlackoutRandomZonesAmount > 1)
            {
                Log.Debug("Multiple random blackout zones detected.");

                HashSet<int> chosenZoneIndices = new HashSet<int>();
                System.Random random = new System.Random();

                while (chosenZoneIndices.Count < FacilityBlackout.Singleton.Config.BlackoutRandomZonesAmount)
                {
                    int newIndex = random.Next(FacilityBlackout.Singleton.Config.BlackoutZones.Count);
                    chosenZoneIndices.Add(newIndex);
                }

                foreach (int zoneIndex in chosenZoneIndices)
                {
                    Log.Debug($"Blackout in zone: {FacilityBlackout.Singleton.Config.BlackoutZones[zoneIndex]}");
                    Map.TurnOffAllLights(FacilityBlackout.Singleton.Config.BlackoutTime, FacilityBlackout.Singleton.Config.BlackoutZones[zoneIndex]);
                }
            }
            else
            {
                Log.Debug("Only one random blackout zone detected.");
                System.Random random = new System.Random();
                int randomIndex = random.Next(FacilityBlackout.Singleton.Config.BlackoutZones.Count);
                Log.Debug($"Blackout is in zone: {FacilityBlackout.Singleton.Config.BlackoutZones[randomIndex]}");
                Map.TurnOffAllLights(FacilityBlackout.Singleton.Config.BlackoutTime, FacilityBlackout.Singleton.Config.BlackoutZones[randomIndex]);
            }
        }

        private void TurnOffAllZones()
        {
            Log.Debug("Trying to execute function TurnOffAllZones...");
            if (!FacilityBlackout.Singleton.Config.BlackoutEnabled)
                return;
            foreach (ZoneType zoneType in FacilityBlackout.Singleton.Config.BlackoutZones)
            {
                Log.Debug($"Blackout Zone: {zoneType}");
                Map.TurnOffAllLights(FacilityBlackout.Singleton.Config.BlackoutTime, zoneType);
            }
        }

        private ZoneType GetRandomZoneExcludingSurface()
        {
            ZoneType randomZone;
            do
            {
                randomZone = (ZoneType)UnityEngine.Random.Range(1, Enum.GetValues(typeof(ZoneType)).Length); // Exclude SurfaceZone
            } while (randomZone == ZoneType.Surface);

            return randomZone;
        }

        private void BlackoutRoomsInZone(ZoneType zone, int amount)
        {
            Log.Debug("Trying to execute function BlackoutRoomsInZone...");
            foreach (Room room in RoomManager.GetRandomRoomsInZone(zone, amount))
            {
                room.Blackout(-1, DoorLockType.None);
            }
        }
    }
}
