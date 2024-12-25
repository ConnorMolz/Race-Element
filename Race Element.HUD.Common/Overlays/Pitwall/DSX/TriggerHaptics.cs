using RaceElement.Data.Common;
using RaceElement.Data.Games;
using RaceElement.Util.SystemExtensions;
using static RaceElement.HUD.Common.Overlays.Pitwall.DSX.DsxResources;

namespace RaceElement.HUD.Common.Overlays.Pitwall.DSX;

internal static class TriggerHaptics
{
    public static Packet HandleBraking(DsxConfiguration config)
    {
        Packet p = new();
        List<Instruction> instructions = [];
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
                    frontslipCoefecient.ClipMax(20);

                    float rearSlipCoefecient = slipRatioFront * 2f;
                    rearSlipCoefecient.ClipMax(15);

                    float magicValue = frontslipCoefecient + rearSlipCoefecient;

                    instructions.Add(new Instruction()
                    {
                        type = InstructionType.TriggerUpdate,
                        /// Start: 0-9 Strength:0-8 Frequency:0-255
                        //parameters = new object[] { controllerIndex, Trigger.Left, TriggerMode.AutomaticGun, 0, 6, 45 } // vibrate is not enough
                        parameters = [controllerIndex, Trigger.Left, TriggerMode.CustomTriggerValue, CustomTriggerValueMode.VibrateResistanceB, config.BrakeSlip.Frequency /*85*/, magicValue, 0, 0, 0, 0, 0]
                    });
                }
            }
        }

        if (instructions.Count == 0)
        {
            instructions.Add(new Instruction()
            {
                type = InstructionType.TriggerUpdate,
                parameters = [controllerIndex, Trigger.Left, TriggerMode.Normal]
            });
        }


        if (instructions.Count == 0) return null;
        p.instructions = instructions.ToArray();
        return p;
    }

    public static Packet HandleAcceleration(DsxConfiguration config)
    {
        Packet p = new();
        List<Instruction> instructions = [];
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
                    frontslipCoefecient.ClipMax(20);

                    float rearSlipCoefecient = slipRatioFront * 6f;
                    rearSlipCoefecient.ClipMax(30);

                    float magicValue = frontslipCoefecient + rearSlipCoefecient;


                    instructions.Add(new Instruction()
                    {
                        type = InstructionType.TriggerUpdate,
                        parameters = [controllerIndex, Trigger.Right, TriggerMode.CustomTriggerValue, CustomTriggerValueMode.VibrateResistanceB, config.ThrottleSlip.Frequency/*130*/, magicValue, 0, 0, 0, 0, 0]
                        /// Start: 0-9 Strength:0-8 Frequency:0-255
                        //parameters = new object[] { controllerIndex, Trigger.Right, TriggerMode.AutomaticGun, 0, 6, 65 }
                    });


                    //instructions.Add(new Instruction()
                    //{
                    //    type = InstructionType.TriggerUpdate,
                    //    parameters = [controllerIndex, Trigger.Right, TriggerMode.AutomaticGun, 0, magicValue, 45]
                    //});
                }
            }
        }

        if (instructions.Count == 0)
        {
            instructions.Add(new Instruction()
            {
                type = InstructionType.TriggerUpdate,
                parameters = [controllerIndex, Trigger.Right, TriggerMode.Normal]
            });
        }


        if (instructions.Count == 0) return null;
        p.instructions = instructions.ToArray();
        return p;
    }
}
