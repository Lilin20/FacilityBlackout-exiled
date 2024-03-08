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
            while (_blackoutCount <= FacilityBlackout.Instance.Config.BlackoutAmount)
            {
                //yield return Timing.WaitForSeconds(EarlyGameTweaks.Instance.Config.BlackoutStartDelay);

                Random random = new Random();

                int randomAdditionalDelay = random.Next(FacilityBlackout.Instance.Config.BlackoutMinDelay, FacilityBlackout.Instance.Config.BlackoutMaxDelay + 1);
                int randomZoneIndex = random.Next(FacilityBlackout.Instance.Config.BlackoutZones.Count);

                float cassieTime = Cassie.CalculateDuration(FacilityBlackout.Instance.Config.BlackoutCassie, false, 1);

                if (_firstBlackoutFired)
                {
                    Timing.WaitForSeconds(FacilityBlackout.Instance.Config.BlackoutDelayBetween);
                    Log.Info($"Additional Delay: {randomAdditionalDelay}, Zone: {FacilityBlackout.Instance.Config.BlackoutZones[randomZoneIndex]}");

                    yield return Timing.WaitForSeconds(randomAdditionalDelay);
                    Cassie.Message(FacilityBlackout.Instance.Config.BlackoutCassie, false, false, true);

                    yield return Timing.WaitForSeconds(cassieTime);
                    // Check for lightcontainment decontamination timer
                    Map.TurnOffAllLights(FacilityBlackout.Instance.Config.BlackoutTime, FacilityBlackout.Instance.Config.BlackoutZones[randomZoneIndex]);
                    _blackoutCount++;
                }
                Log.Info($"Additional Delay: {randomAdditionalDelay}, Zone: {FacilityBlackout.Instance.Config.BlackoutZones[randomZoneIndex]}");

                yield return Timing.WaitForSeconds(randomAdditionalDelay);
                Cassie.Message(FacilityBlackout.Instance.Config.BlackoutCassie, false, false, true);

                yield return Timing.WaitForSeconds(cassieTime);
                // Check for lightcontainment decontamination timer
                Map.TurnOffAllLights(FacilityBlackout.Instance.Config.BlackoutTime, FacilityBlackout.Instance.Config.BlackoutZones[randomZoneIndex]);
                _blackoutCount++;
            }
        }
    }
}
