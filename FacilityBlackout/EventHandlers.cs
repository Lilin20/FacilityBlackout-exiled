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
        public bool CheckCassieSpeaking()
        {
            return Cassie.IsSpeaking;
        }

        public void OnRoundStarted()
        {
            if (FacilityBlackout.Instance.Config.BlackoutRooms)
            {
                if (FacilityBlackout.Instance.Config.BlackoutRoomsRandomZones)
                {
                    HashSet<ZoneType> chosenZones = new HashSet<ZoneType>();
                    System.Random random = new System.Random();

                    while (chosenZones.Count < FacilityBlackout.Instance.Config.BlackoutRoomsRandomZonesAmount)
                    {
                        ZoneType randomZone = GetRandomZoneExcludingSurface();
                        chosenZones.Add(randomZone);
                    }

                    foreach (ZoneType zone in chosenZones)
                    {
                        int blackoutRoomsAmount = FacilityBlackout.Instance.Config.BlackoutRoomsAmount; // Move inside the loop
                        Log.Debug($"Blackouting rooms in zone: {zone} - Amount: {blackoutRoomsAmount}");
                        BlackoutRoomsInZone(zone, blackoutRoomsAmount);
                    }
                }
                else
                {
                    foreach (ZoneType zone in FacilityBlackout.Instance.Config.BlackoutZones)
                    {
                        int blackoutRoomsAmount = FacilityBlackout.Instance.Config.BlackoutRoomsAmount; // Move inside the loop
                        Log.Debug($"Blackouting rooms in zone: {zone} - Amount: {blackoutRoomsAmount}");
                        BlackoutRoomsInZone(zone, blackoutRoomsAmount);
                    }
                }
            }

            _blackoutCount = 0;
            Timing.CallDelayed(FacilityBlackout.Instance.Config.BlackoutStartDelay, () =>
            {
                Timing.RunCoroutine(BlackoutCoroutine());
            });
        }

        public void OnDecontamination(DecontaminatingEventArgs ev)
        {
            FacilityBlackout.Instance.Config.BlackoutZones.Remove(ZoneType.LightContainment); // Needs to get changed asap. Bad implementation.
        }

        public IEnumerator<float> BlackoutCoroutine()
        {
            while (_blackoutCount <= FacilityBlackout.Instance.Config.BlackoutAmount -1)
            {
                System.Random random = new System.Random();
                int randomAdditionalDelay = random.Next(FacilityBlackout.Instance.Config.BlackoutMinDelay, FacilityBlackout.Instance.Config.BlackoutMaxDelay);
                int zoneIndex = FacilityBlackout.Instance.Config.BlackoutRandomZones ? random.Next(FacilityBlackout.Instance.Config.BlackoutZones.Count) : 0;
                float cassieTime = Cassie.CalculateDuration(FacilityBlackout.Instance.Config.BlackoutCassie, false, 1);

                if (_firstBlackoutFired)
                {
                    yield return Timing.WaitForSeconds(FacilityBlackout.Instance.Config.BlackoutDelayBetween);

                    Log.Debug($"Additional Blackouts: Additional Delay: {randomAdditionalDelay} until Blackout");
                    yield return Timing.WaitForSeconds(randomAdditionalDelay);

                    Cassie.MessageTranslated(FacilityBlackout.Instance.Config.BlackoutCassie, FacilityBlackout.Instance.Config.BlackoutCassieTranslation, false, false, true);
                    yield return Timing.WaitForSeconds(cassieTime);

                    if (FacilityBlackout.Instance.Config.BlackoutRandomZones)
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

                    Cassie.MessageTranslated(FacilityBlackout.Instance.Config.BlackoutCassie, FacilityBlackout.Instance.Config.BlackoutCassieTranslation, false, false, true);
                    yield return Timing.WaitForSeconds(cassieTime);

                    if (FacilityBlackout.Instance.Config.BlackoutRandomZones)
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
            if (FacilityBlackout.Instance.Config.BlackoutRandomZonesAmount > 1)
            {
                Log.Debug("Multiple random blackout zones detected.");

                HashSet<int> chosenZoneIndices = new HashSet<int>();
                System.Random random = new System.Random();

                while (chosenZoneIndices.Count < FacilityBlackout.Instance.Config.BlackoutRandomZonesAmount)
                {
                    int newIndex = random.Next(FacilityBlackout.Instance.Config.BlackoutZones.Count);
                    chosenZoneIndices.Add(newIndex);
                }

                foreach (int zoneIndex in chosenZoneIndices)
                {
                    Log.Debug($"Blackout in zone: {FacilityBlackout.Instance.Config.BlackoutZones[zoneIndex]}");
                    Map.TurnOffAllLights(FacilityBlackout.Instance.Config.BlackoutTime, FacilityBlackout.Instance.Config.BlackoutZones[zoneIndex]);
                }
            }
            else
            {
                Log.Debug("Only one random blackout zone detected.");
                System.Random random = new System.Random();
                int randomIndex = random.Next(FacilityBlackout.Instance.Config.BlackoutZones.Count);
                Log.Debug($"Blackout is in zone: {FacilityBlackout.Instance.Config.BlackoutZones[randomIndex]}");
                Map.TurnOffAllLights(FacilityBlackout.Instance.Config.BlackoutTime, FacilityBlackout.Instance.Config.BlackoutZones[randomIndex]);
            }
        }

        private void TurnOffAllZones()
        {
            foreach (ZoneType zoneType in FacilityBlackout.Instance.Config.BlackoutZones)
            {
                Log.Debug($"Blackout Zone: {zoneType}");
                Map.TurnOffAllLights(FacilityBlackout.Instance.Config.BlackoutTime, zoneType);
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
            foreach (Room room in RoomManager.GetRandomRoomsInZone(zone, amount))
            {
                room.Blackout(-1, DoorLockType.None);
            }
        }
    }
}
