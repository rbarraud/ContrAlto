﻿using Contralto.Display;
using System;


namespace Contralto.CPU
{
    public partial class AltoCPU
    {
        /// <summary>
        /// DisplayWordTask provides functionality for the DWT task
        /// </summary>
        private sealed class DisplayWordTask : Task
        {
            public DisplayWordTask(AltoCPU cpu) : base(cpu)
            {
                _taskType = TaskType.DisplayWord;
                _wakeup = false;

                _displayController = _cpu._system.DisplayController;
            }

            protected override bool ExecuteInstruction(MicroInstruction instruction)
            {
                // We remove our wakeup only if there isn't a wakeup being generated for us by the
                // display controller.
                _wakeup = (!_displayController.FIFOFULL &&
                            !_displayController.DHTBLOCK &&
                            !_displayController.DWTBLOCK);                

                return base.ExecuteInstruction(instruction);
            }

            protected override void ExecuteSpecialFunction2(MicroInstruction instruction)
            {
                DisplayWordF2 dw2 = (DisplayWordF2)instruction.F2;
                switch (dw2)
                {
                    case DisplayWordF2.LoadDDR:
                        _displayController.LoadDDR(_busData);
                        break;

                    default:
                        throw new InvalidOperationException(String.Format("Unhandled display word F2 {0}.", dw2));                        
                }
            }

            protected override void ExecuteBlock()
            {
                _displayController.DWTBLOCK = true;

                //
                // Wake up DHT if it has not blocked itself.
                //
                if (!_displayController.DHTBLOCK)
                {
                    _cpu.WakeupTask(TaskType.DisplayHorizontal);
                }
            }

            private DisplayController _displayController;
        }
    }
}