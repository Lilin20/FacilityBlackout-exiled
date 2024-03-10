using Exiled.API.Features;
using Exiled.API.Enums;
using System.Collections.Generic;
using PlayerRoles;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Map;
using System.Globalization;
using MEC;
using System;


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
            _blackoutCount = 0;
            Timing.CallDelayed(FacilityBlackout.Instance.Config.BlackoutStartDelay, () =>
            {
                Timing.RunCoroutine(BlackoutCoroutine());
            });
        }

        public void OnDecontamination(DecontaminatingEventArgs ev)
        {
            FacilityBlackout.Instance.Config.BlackoutZones.Remove(ZoneType.LightContainment);
        }

        public IEnumerator<float> BlackoutCoroutine()
        {
            while (_blackoutCount <= FacilityBlackout.Instance.Config.BlackoutAmount - 1)
            {
                //yield return Timing.WaitForSeconds(EarlyGameTweaks.Instance.Config.BlackoutStartDelay);

                Random random = new Random();

                int randomAdditionalDelay = random.Next(FacilityBlackout.Instance.Config.BlackoutMinDelay, FacilityBlackout.Instance.Config.BlackoutMaxDelay + 1);
                int ZoneIndex = 0;

                if (FacilityBlackout.Instance.Config.BlackoutRandomZones)
                {
                    ZoneIndex = random.Next(FacilityBlackout.Instance.Config.BlackoutZones.Count);
                }

                float cassieTime = Cassie.CalculateDuration(FacilityBlackout.Instance.Config.BlackoutCassie, false, 1);

                if (_firstBlackoutFired)
                {
                    yield return Timing.WaitForSeconds(FacilityBlackout.Instance.Config.BlackoutDelayBetween);
                    Log.Debug($"Additional Delay: {randomAdditionalDelay} until Blackout");

                    yield return Timing.WaitForSeconds(randomAdditionalDelay);
                    Cassie.Message(FacilityBlackout.Instance.Config.BlackoutCassie, false, false, true);

                    yield return Timing.WaitForSeconds(cassieTime);

                    if (FacilityBlackout.Instance.Config.BlackoutRandomZones)
                    {
                        Map.TurnOffAllLights(FacilityBlackout.Instance.Config.BlackoutTime, FacilityBlackout.Instance.Config.BlackoutZones[ZoneIndex]);
                    }
                    else
                    {
                        foreach (ZoneType zoneType in FacilityBlackout.Instance.Config.BlackoutZones)
                        {
                            Log.Debug($"Blackout Zone: {zoneType}");
                            Map.TurnOffAllLights(FacilityBlackout.Instance.Config.BlackoutTime, zoneType);
                        }
                    }
                    _blackoutCount++;
                }
                Log.Debug($"Additional Delay: {randomAdditionalDelay} until Blackout");

                yield return Timing.WaitForSeconds(randomAdditionalDelay);
                Cassie.Message(FacilityBlackout.Instance.Config.BlackoutCassie, false, false, true);

                yield return Timing.WaitForSeconds(cassieTime);

                if (FacilityBlackout.Instance.Config.BlackoutRandomZones)
                {
                    Map.TurnOffAllLights(FacilityBlackout.Instance.Config.BlackoutTime, FacilityBlackout.Instance.Config.BlackoutZones[ZoneIndex]);
                }
                else
                {
                    foreach (ZoneType zoneType in FacilityBlackout.Instance.Config.BlackoutZones)
                    {
                        Log.Debug($"Blackout Zone: {zoneType}");
                        Map.TurnOffAllLights(FacilityBlackout.Instance.Config.BlackoutTime, zoneType);
                    }
                }
                _firstBlackoutFired = true;
                _blackoutCount++;
            }
        }
    }
}
