using System;
using System.Collections.Generic;
using System.Threading;

namespace Snake3._0
{
    class Program
    {
        //game
        static int width = 35;
        static int height = 20;

        //snake information
        static int headX = 1;
        static int headY = 1;
        static List<int> tailX = new List<int>();
        static List<int> tailY = new List<int>();
        static int moveX = 0;
        static int moveY = 0;
        static int points = 0;
        static int speed = 100;
        static List<int> highscore = new List<int>();

        //äpple
        static Random rnd = new Random();
        static int appleX = rnd.Next(1, width - 1);
        static int appleY = rnd.Next(1, height - 1);

        static void Main(string[] args)
        {
            ChangePositionColor(0, 0, ConsoleColor.Yellow);
            Console.WriteLine("Välkommen till snake! Ät äpplen för att få så högt score som möjligt");
            Thread.Sleep(2000);
            do
            {
                PlayGame(); //starta spel

                //efter död
                highscore.Add(points);
                Console.Clear();
                ChangePositionColor(0, 0, ConsoleColor.Red);
                Console.WriteLine("Bra jobbat!");
                Console.WriteLine("Poäng: " + points);
                Console.WriteLine("\nVill du spela igen? Skriv 'Ja' för att köra igen");
                Reset(); //resetta statistik
            } while (Console.ReadLine().ToLower() == "ja");

            Console.Clear();
            Console.WriteLine("Bra jobbat, såhär gick dina rundor:");
            for(int i = 0; i < highscore.Count; i++)
            {
                Console.WriteLine($"Runda {i+1}: {highscore[i]}");
            }
            Console.WriteLine("\nTryck på valfri knapp för att avsluta...");
            Console.ReadKey();
        }

        static void PlayGame()
        {
            while (!Die())
            {
                Console.Clear();
                DrawSnake();
                DrawMap();
                ChangePositionColor(0, height + 1, ConsoleColor.White);
                Console.WriteLine("Poäng: " + points);
                Console.WriteLine("Hastighet: " + speed + " (Lägre är snabbare)");
                Thread.Sleep(speed);
            }
        }

        static void DrawSnake()
        {
            Movement(); //hanterar moveX och moveY
            Tail(); //svansen

            headX += moveX; //ändring X led
            headY += moveY; //ändring Y led

            //måla upp ormen
            ChangePositionColor(headX, headY, ConsoleColor.Green);
            Console.SetCursorPosition(headX, headY);
            Console.Write("*");
        }

        static void Tail()
        {

            if (Collision(appleX, appleY)) //krock med äpple
            {
                //uppdatera koordinater (byt plats på äpple)
                appleX = rnd.Next(1, width - 1);
                appleY = rnd.Next(1, height - 1);

                if(tailX.Count == 0 && tailY.Count == 0) //ingen svanns än -> bygg på huvudet
                {
                    tailX.Add(headX);
                    tailY.Add(headY);
                } else //bygg på svans
                {
                    tailX.Add(tailX[tailX.Count-1] - moveX);
                    tailY.Add(tailY[tailY.Count -1]- moveY);
                }
                points++;
                speed --; //öka hastighet
            }

            //håller gamla X och Y värden
            int[] oldTailX = new int[tailX.Count];
            int[] oldTailY = new int[tailY.Count];

            //uppdatera tail koordinater 
            for (int i = 0; i < tailX.Count; i++) 
            {
                oldTailX[i] = tailX[i];
                oldTailY[i] = tailY[i];

                if (i == 0)
                {
                    //följ efter huvudet
                    tailX[i] = headX;
                    tailY[i] = headY;
                } else
                {
                    //gör så svansen hoppar fram ett steg
                    tailX[i] = oldTailX[i - 1];
                    tailY[i] = oldTailY[i - 1];
                }

                //rita svansen
                ChangePositionColor(tailX[i], tailY[i], ConsoleColor.Green);
                Console.Write("*");
            }
        }

        static void DrawMap()
        {
            //bygg väggar
            for(int i = 0; i < width; i++) //toppen
            {
                ChangePositionColor(i, 0, ConsoleColor.White);
                Console.Write("-");
            }

            for(int i = 0; i < width; i++) //botten
            {
                ChangePositionColor(i, height, ConsoleColor.White);
                Console.Write("-");
            }

            for (int i = 0; i < height; i++) //vänster
            {
                ChangePositionColor(0, i+1, ConsoleColor.White);
                Console.Write("|");
            }

            for (int i = 0; i < height; i++) //höger
            {
                ChangePositionColor(width, i+1, ConsoleColor.White);
                Console.Write("|");
            }

            //placera ut äpple
            ChangePositionColor(appleX, appleY, ConsoleColor.Red);
            Console.Write("O");
        }

        static void Movement()
        {
            if (Console.KeyAvailable) //om knapp är nedtryckt
            {
                ConsoleKeyInfo key = Console.ReadKey(); //läs av input
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow: //upp
                        moveX = 0;
                        moveY = -1;
                        break;
                    case ConsoleKey.DownArrow: //ner
                        moveX = 0;
                        moveY = 1;
                        break;
                    case ConsoleKey.RightArrow: //höger
                        moveX = 1;
                        moveY = 0;
                        break;
                    case ConsoleKey.LeftArrow: //vänster
                        moveX = -1;
                        moveY = 0;
                        break;
                }
            }
        }

        //jämför koordinater med snake huvud
        static bool Collision(int x, int y)
        {
            if (headX == x && headY == y)
            {
                return true; //krock
            }
            else
            {
                return false; //inte krock
            }
        }

        //kollar efter någon krock
        static bool Die()
        {
            //Kollision med vägg
            for(int i = 0; i < width; i++)
            {
                //krock med topp vägg
                if(Collision(i, 0))
                {
                    return true;
                }
                
                //krock med nedre vägg
                if(Collision(i, height))
                {
                    return true;
                }
            }

            for(int i = 0; i < height; i++)
            {
                //krock med höger vägg
                if(Collision(0, i))
                {
                    return true;
                }

                //krock med vänster vägg
                if(Collision(width, i))
                {
                    return true;
                }
            }

            //krock med svans
            for(int i = 0; i < tailX.Count; i++)
            {
                if(Collision(tailX[i], tailY[i]))
                {
                    return true;
                }
            }

            //ingen krock
            return false;
        }

        //resettar all statistik
        static void Reset()
        {
            //snake information
            headX = 1;
            headY = 1;
            tailX = new List<int>();
            tailY = new List<int>();
            moveX = 0;
            moveY = 0;
            points = 0;
            speed = 100;

            //äpple
            Random rnd = new Random();
            appleX = rnd.Next(1, width - 1);
            appleY = rnd.Next(1, height - 1);
        }

        //ändra färg och position 
        static void ChangePositionColor(int x, int y, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.SetCursorPosition(x, y);
        }
    }
}
