using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleButton2
{
    class Program
    {
        static Stopwatch _sw = new Stopwatch();
        static GpioController _controller;

        static int _redPin = 27;
        static int _greenPin = 22;

        static int _pulses;

        static bool _buttonPressed;
        static List<long> _elaspedTimes = new List<long>();

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            _controller = new GpioController(PinNumberingScheme.Logical);

            var buttonPin = 17;

            _controller.OpenPin(_redPin, PinMode.Output);
            _controller.OpenPin(_greenPin, PinMode.Output);

            PinChangeEventHandler buttonPushedDown = (object sender, PinValueChangedEventArgs args) => { ButtonPushedDown(args); };
            PinChangeEventHandler buttonLetGo = (object sender, PinValueChangedEventArgs args) => { ButtonLetGo(args); };

            var startTime = DateTime.Now;
            _controller.OpenPin(buttonPin, PinMode.InputPullUp);
            _controller.RegisterCallbackForPinValueChangedEvent(buttonPin, PinEventTypes.Falling, buttonPushedDown);
            _controller.RegisterCallbackForPinValueChangedEvent(buttonPin, PinEventTypes.Rising, buttonLetGo);

            
            while (true)
            {
                Task.Delay(new TimeSpan(0, 0, 0, 0, 500)).Wait();

                // Possible double tap
                if (!_buttonPressed && _elaspedTimes.Count > 1)
                {
                    if (_elaspedTimes.All(x => x <= 1000))
                    {
                        // Alternate Lights
                        Console.WriteLine("Double tap");
                        _controller.Write(_redPin, PinValue.Low);
                        _controller.Write(_greenPin, PinValue.High);
                        Thread.Sleep(200);
                        _controller.Write(_redPin, PinValue.High);
                        _controller.Write(_greenPin, PinValue.Low);
                        Thread.Sleep(200);
                        _controller.Write(_redPin, PinValue.Low);
                        _controller.Write(_greenPin, PinValue.High);
                        Thread.Sleep(200);
                        _controller.Write(_redPin, PinValue.High);
                        _controller.Write(_greenPin, PinValue.Low);
                        Thread.Sleep(200);

                        ResetVariables();
                    }
                }
                else if (!_buttonPressed && _elaspedTimes.Count == 1)
                {
                    if (_elaspedTimes[0] > 1500)
                    {
                        _controller.Write(_redPin, PinValue.High);
                        _controller.Write(_greenPin, PinValue.Low);
                        Console.WriteLine("Red LED On");
                    }
                    else
                    {
                        _controller.Write(_redPin, PinValue.Low);
                        _controller.Write(_greenPin, PinValue.High);
                        Console.WriteLine("Green LED On");
                    }

                    ResetVariables();
                }
                else
                {
                    Console.WriteLine("Lights Off");
                    _controller.Write(_redPin, PinValue.Low);
                    _controller.Write(_greenPin, PinValue.Low);
                }
                
            }
            //Task.Delay(new TimeSpan(0, 0, 0, 0, sampleMilliseconds)).Wait(); //wait
            //int count = 0;
            //try
            //{
            //    while (true)
            //    {
            //        Console.WriteLine($"Button Value at {count} - " + controller.Read(buttonPin));
            //        count++;
            //        if (controller.Read(buttonPin) == false)
            //        {
            //            controller.Write(redPin, PinValue.High);
            //            Console.WriteLine("Red LED On");
            //        }
            //        else
            //        {
            //            controller.Write(redPin, PinValue.Low);
            //            Console.WriteLine("Red LED Off");
            //        }

            //        Thread.Sleep(2000);
            //        Console.WriteLine("Sleeping");
            //    }
            //}
            //finally
            //{
            //    controller.ClosePin(redPin);
            //    Console.WriteLine("Closing Pin");
            //    Console.WriteLine("Program finsihed");
            //}
            
        }

        private static void ResetVariables()
        {
            _elaspedTimes.Clear();
            _buttonPressed = false;
            _pulses = 0;
        }

        private static void ButtonLetGo(PinValueChangedEventArgs args)
        {
            _sw.Stop();
            _buttonPressed = false;
            Console.WriteLine($"Button was pressed for {_sw.ElapsedMilliseconds} milliseconds");

            if(_sw.ElapsedMilliseconds < 1000)
            {
                _pulses++;
            }

            _elaspedTimes.Add(_sw.ElapsedMilliseconds);

            //if(_sw.ElapsedMilliseconds > 2000)
            //{
            //    _controller.Write(_redPin, PinValue.High);
            //    _controller.Write(_greenPin, PinValue.Low);
            //    Console.WriteLine("Red LED On");
            //}
            //else if (_sw.ElapsedMilliseconds < 2000)
            //{
            //    _controller.Write(_redPin, PinValue.Low);
            //    _controller.Write(_greenPin, PinValue.High);
            //    Console.WriteLine("Green LED On");
            //}
            //else
            //{
            //    _controller.Write(_redPin, PinValue.Low);
            //    _controller.Write(_greenPin, PinValue.Low);
            //    Console.WriteLine("Both LEDs Off");
            //}

                        
            _sw.Reset();
        }

        private static void ButtonPushedDown(PinValueChangedEventArgs args)
        {
            _sw.Start();
            _buttonPressed = true;
        }


    }
}
