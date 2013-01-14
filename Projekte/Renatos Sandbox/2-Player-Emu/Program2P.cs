using System;
using Emulator;
using InputEmulation;
using MF.Engineering.MF8910.GestureDetector.DataSources;
using MF.Engineering.MF8910.GestureDetector.Events;
using MF.Engineering.MF8910.GestureDetector.Tools;

namespace _2_Player_Emu
{
    class Program2P
    {
        private static Device _d;
        private static Person _p1;
        private static Person _p2;
        private static SteeringGestureChecker sgcP1;
        private static ThrustGestureChecker tgcP1;
        private static FootGestureChecker fgcP1;
        private static SteeringGestureChecker sgcP2;
        private static ThrustGestureChecker tgcP2;
        private static FootGestureChecker fgcP2;
        private static bool leftdown;
        private static bool rightdown;
        private static bool downdown;
        private static bool updown;
        private static bool altdown;
        private static bool enterdown;
        private static bool threedown;
        private static bool spacedown;
        private static bool qdown;
        private static bool edown;
        private static bool wdown;
        private static bool sdown;
        private static bool ddown;
        private static bool xdown;
        private static bool adown;

        static void Main(string[] args)
        {
            _d = new Device();
            _d.NewPerson += NewPlayer;
            _d.PersonLost += Dispose;
            _d.Start();
            while (true)
            {
                System.Threading.Thread.Sleep(5);
            }
        }

        private static void NewPlayer(object sender, NewPersonEventArgs e)
        {
            if (_p1 == null)
            {
                _p1 = e.Person;
                Console.WriteLine("Player1 detected");
                sgcP1 = new SteeringGestureChecker(e.Person);
                tgcP1 = new ThrustGestureChecker(e.Person);
                fgcP1 = new FootGestureChecker(e.Person);
                sgcP1.Successful += P1Steered;
                tgcP1.Successful += P1Thrusted;
                fgcP1.Successful += P1PowerUp;
            }
            else if (_p2 == null)
            {
                _p2 = e.Person;
                Console.WriteLine("Player2 detected");
                sgcP2 = new SteeringGestureChecker(e.Person);
                tgcP2 = new ThrustGestureChecker(e.Person);
                fgcP2 = new FootGestureChecker(e.Person);
                sgcP2.Successful += P2Steered;
                tgcP2.Successful += P2Thrusted;
                fgcP2.Successful += P2PowerUp;
            }
            else
            {
                Console.WriteLine("Sorry, game is full");
            }
        }
#region P1
        private static void P1PowerUp(object sender, GestureEventArgs e)
        {
            Direction direction = ((FootEventArgs)e).Foot;
            if (direction == Direction.Left)
            {
                Console.Write("P1 Powerup ");
                if (!enterdown)
                {
                    DirectInput.KeyDown(DirectInput.VK_ENTER);
                    enterdown = true;
                }
            }
            else if (direction == Direction.Right)
            {
                Console.Write("P1 talking ");
                if (!threedown)
                {
                    DirectInput.KeyDown(DirectInput.VK_3);
                    threedown = true;
                }
            }
            else
            {
                if (enterdown)
                {
                    DirectInput.KeyUp(DirectInput.VK_1);
                    enterdown = false;
                }
                if (threedown)
                {
                    DirectInput.KeyUp(DirectInput.VK_3);
                    threedown = false;
                }
            }
        }

        private static void P1Thrusted(object sender, GestureEventArgs e)
        {
            double dist = ((ThrustGestureEventArgs)e).DistanceToShoulder;
            //Console.Write(dist + " ");
            if (dist > 0.50)
            {
                Console.Write("P1 Forward ");
                if (downdown)
                {
                    DirectInput.KeyUp(DirectInput.VK_DOWN);
                    downdown = false;
                }
                if (!updown)
                {
                    DirectInput.KeyDown(DirectInput.VK_UP);
                    updown = true;
                }
            }
            else if (dist > 0.35)
            {
                Console.Write("P1 Neutral ");
                if (downdown)
                {
                    DirectInput.KeyUp(DirectInput.VK_DOWN);
                    downdown = false;
                }
                if (updown)
                {
                    DirectInput.KeyUp(DirectInput.VK_UP);
                    updown = false;
                }
            }
            else
            {
                Console.Write("P1 Reverse ");
                if (updown)
                {
                    DirectInput.KeyUp(DirectInput.VK_UP);
                    updown = false;
                }
                if (!downdown)
                {
                    DirectInput.KeyDown(DirectInput.VK_DOWN);
                    downdown = true;
                }
            }
        }

        private static void P1Steered(object sender, GestureEventArgs e)
        {
            var direction = ((SteeringGestureEventArgs)e).Direction;
            Console.Write("P1 steering " + direction + " ");
            if (direction < 0)
            {
                if (rightdown)
                {
                    DirectInput.KeyUp(DirectInput.VK_RIGHT);
                    rightdown = false;
                }
                if (direction == -2)
                {
                    if (!spacedown)
                    {
                        DirectInput.KeyDown(DirectInput.VK_SPACE);
                        spacedown = true;
                    }
                }
                else
                {
                    if (!leftdown)
                    {
                        DirectInput.KeyDown(DirectInput.VK_LEFT);
                        leftdown = true;
                    }
                    if (spacedown)
                    {
                        DirectInput.KeyUp(DirectInput.VK_SPACE);
                        spacedown = false;
                    }
                }
            }
            else if (direction > 0)
            {
                if (leftdown)
                {
                    DirectInput.KeyUp(DirectInput.VK_LEFT);
                    leftdown = false;
                }
                if (direction == 2)
                {
                    if (!spacedown)
                    {
                        DirectInput.KeyDown(DirectInput.VK_SPACE);
                        spacedown = true;
                    }
                }
                else
                {
                    if (!rightdown)
                    {
                        DirectInput.KeyDown(DirectInput.VK_RIGHT);
                        rightdown = true;
                    }
                    if (spacedown)
                    {
                        DirectInput.KeyUp(DirectInput.VK_SPACE);
                        spacedown = false;
                    }
                }
                //Keyboard.SendKeyAsInput(Keys.Right);
            }
            else
            {
                if (leftdown)
                {
                    DirectInput.KeyUp(DirectInput.VK_LEFT);
                    leftdown = false;
                }
                if (rightdown)
                {
                    DirectInput.KeyUp(DirectInput.VK_RIGHT);
                    rightdown = false;
                }
                if (spacedown)
                {
                    DirectInput.KeyUp(DirectInput.VK_SPACE);
                    spacedown = false;
                }
            }
        }
#endregion
#region P2
        private static void P2PowerUp(object sender, GestureEventArgs e)
        {
            Direction direction = ((FootEventArgs)e).Foot;
            if (direction == Direction.Left)
            {
                Console.Write("P2 Powerup ");
                if (!qdown)
                {
                    DirectInput.KeyDown(DirectInput.VK_Q);
                    qdown = true;
                }
            }
            else if (direction == Direction.Right)
            {
                Console.Write("P2 talking ");
                if (!edown)
                {
                    DirectInput.KeyDown(DirectInput.VK_E);
                    edown = true;
                }
            }
            else
            {
                if (qdown)
                {
                    DirectInput.KeyUp(DirectInput.VK_Q);
                    qdown = false;
                }
                if (edown)
                {
                    DirectInput.KeyUp(DirectInput.VK_E);
                    edown = false;
                }
            }
        }

        private static void P2Thrusted(object sender, GestureEventArgs e)
        {
            double dist = ((ThrustGestureEventArgs)e).DistanceToShoulder;
            //Console.Write(dist + " ");
            if (dist > 0.50)
            {
                Console.Write("P2 Forward ");
                if (sdown)
                {
                    DirectInput.KeyUp(DirectInput.VK_S);
                    sdown = false;
                }
                if (!wdown)
                {
                    DirectInput.KeyDown(DirectInput.VK_W);
                    wdown = true;
                }
            }
            else if (dist > 0.35)
            {
                Console.Write("P2 Neutral ");
                if (sdown)
                {
                    DirectInput.KeyUp(DirectInput.VK_S);
                    sdown = false;
                }
                if (wdown)
                {
                    DirectInput.KeyUp(DirectInput.VK_W);
                    wdown = false;
                }
            }
            else
            {
                Console.Write("P2 Reverse ");
                if (wdown)
                {
                    DirectInput.KeyUp(DirectInput.VK_W);
                    wdown = false;
                }
                if (!sdown)
                {
                    DirectInput.KeyDown(DirectInput.VK_S);
                    sdown = true;
                }
            }
        }

        private static void P2Steered(object sender, GestureEventArgs e)
        {
            var direction = ((SteeringGestureEventArgs)e).Direction;
            Console.WriteLine("P2 steering " + direction + " ");
            if (direction < 0)
            {
                if (ddown)
                {
                    DirectInput.KeyUp(DirectInput.VK_D);
                    ddown = false;
                }
                if (direction == -2)
                {
                    if (!xdown)
                    {
                        DirectInput.KeyDown(DirectInput.VK_X);
                        xdown = true;
                    }
                }
                else
                {
                    if (!adown)
                    {
                        DirectInput.KeyDown(DirectInput.VK_A);
                        adown = true;
                    }
                    if (xdown)
                    {
                        DirectInput.KeyUp(DirectInput.VK_X);
                        xdown = false;
                    }
                }
            }
            else if (direction > 0)
            {
                if (adown)
                {
                    DirectInput.KeyUp(DirectInput.VK_A);
                    adown = false;
                }
                if (direction == 2)
                {
                    if (!xdown)
                    {
                        DirectInput.KeyDown(DirectInput.VK_X);
                        xdown = true;
                    }
                }
                else
                {
                    if (!ddown)
                    {
                        DirectInput.KeyDown(DirectInput.VK_D);
                        ddown = true;
                    }
                    if (xdown)
                    {
                        DirectInput.KeyUp(DirectInput.VK_X);
                        xdown = false;
                    }
                }
                //Keyboard.SendKeyAsInput(Keys.Right);
            }
            else
            {
                if (adown)
                {
                    DirectInput.KeyUp(DirectInput.VK_A);
                    adown = false;
                }
                if (ddown)
                {
                    DirectInput.KeyUp(DirectInput.VK_D);
                    ddown = false;
                }
                if (xdown)
                {
                    DirectInput.KeyUp(DirectInput.VK_X);
                    xdown = false;
                }
            }
        }
#endregion
        private static void Dispose(object sender, PersonDisposedEventArgs e)
        {
            if (e.Person == _p1)
            {
                Console.WriteLine("P1 disapeared");
                sgcP1.Successful -= P1Steered;
                fgcP1.Successful -= P1PowerUp;
                tgcP1.Successful -= P1Thrusted;
                _p1 = null;
            }
            if (e.Person == _p2)
            {
                Console.WriteLine("P2 disapeared");
                sgcP2.Successful -= P2Steered;
                fgcP2.Successful -= P2PowerUp;
                tgcP2.Successful -= P2Thrusted;
                _p2 = null;
            }
            Console.WriteLine("User disapeared");
        }
    }
}
