using RaceElement.Data.Common;
using RaceElement.Data.Games;
using RaceElement.Util.SystemExtensions;
using System.Diagnostics;
using static RaceElement.HUD.Common.Overlays.Pitwall.DSX.Resources;

namespace RaceElement.HUD.Common.Overlays.Pitwall.DSX;

internal static class TriggerHaptics
{
    public static DsxPacket HandleBraking(DsxConfiguration config)
    {
        DsxPacket p = new();
        int controllerIndex = 0;

        // TODO: add either an option to threshold it on brake input or based on some curve?
        if (SimDataProvider.LocalCar.Inputs.Brake > config.BrakeSlip.BrakeThreshold / 100f)
        {
            float[] slipRatios = SimDataProvider.LocalCar.Tyres.SlipRatio;

            if (slipRatios.Length == 4)
            {
                float slipRatioFront = Math.Max(slipRatios[0], slipRatios[1]);
                float slipRatioRear = Math.Max(slipRatios[2], slipRatios[3]);

                // TODO: add option for front and rear ratio threshold.
                if (slipRatioFront > config.BrakeSlip.FrontSlipThreshold || slipRatioRear > config.BrakeSlip.RearSlipThreshold)
                {
                    float frontslipCoefecient = slipRatioFront * 4f;
                    frontslipCoefecient.ClipMax(10);

                    float rearSlipCoefecient = slipRatioFront * 2f;
                    rearSlipCoefecient.ClipMax(7.5f);


                    float magicValue = frontslipCoefecient + rearSlipCoefecient;
                    float percentage = magicValue * 1.0f / 17.5f;
                    if (percentage >= 0.05f)
                        p.AddAdaptiveTriggerToPacket(controllerIndex, Trigger.Left, TriggerMode.FEEDBACK, [1, (int)(7 * percentage)]);

                    int freq = config.BrakeSlip.MaxFrequency - (int)(config.BrakeSlip.MaxFrequency * percentage);
                    freq.ClipMin(2);
                    p.AddAdaptiveTriggerToPacket(controllerIndex, Trigger.Left, TriggerMode.VIBRATION, [0, 8, freq]);
                }
            }
        }

        if (p.Instructions == null) p.AddAdaptiveTriggerToPacket(0, Trigger.Left, TriggerMode.Normal, []);

        return p;
    }

    public static DsxPacket HandleAcceleration(DsxConfiguration config)
    {
        DsxPacket p = new();
        int controllerIndex = 0;

        if (SimDataProvider.LocalCar.Inputs.Throttle > config.ThrottleSlip.ThrottleThreshold / 100f)
        {
            float[] slipRatios = SimDataProvider.LocalCar.Tyres.SlipRatio;
            if (slipRatios.Length == 4)
            {
                float slipRatioFront = Math.Max(slipRatios[0], slipRatios[1]);
                float slipRatioRear = Math.Max(slipRatios[2], slipRatios[3]);

                if (slipRatioFront > config.ThrottleSlip.FrontSlipThreshold || slipRatioRear > config.ThrottleSlip.RearSlipThreshold)
                {
                    float frontslipCoefecient = slipRatioFront * 4;
                    frontslipCoefecient.ClipMax(5);
                    float rearSlipCoefecient = slipRatioFront * 6f;
                    rearSlipCoefecient.ClipMax(7.5f);

                    float magicValue = frontslipCoefecient + rearSlipCoefecient;
                    float percentage = magicValue * 1.0f / 12.5f;

                    if (percentage >= 0.05f)
                        p.AddAdaptiveTriggerToPacket(controllerIndex, Trigger.Right, TriggerMode.FEEDBACK, [1, (int)(6 * percentage)]);

                    int freq = config.ThrottleSlip.MaxFrequency - (int)(config.ThrottleSlip.MaxFrequency * percentage);
                    freq.ClipMin(2);
                    p.AddAdaptiveTriggerToPacket(controllerIndex, Trigger.Right, TriggerMode.VIBRATION, [0, 8, freq]);
                }
            }
        }

        if (p.Instructions == null) p.AddAdaptiveTriggerToPacket(0, Trigger.Right, TriggerMode.Normal, []);

        return p;
    }
}
