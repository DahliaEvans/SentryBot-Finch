using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinchAPI;

namespace SentryBot_Finch
{
    class Program
    {
        static void Main(string[] args)
        {
            DisplayWelcomeScreen();
            DisplayMenu();
            DisplayClosingScreen();
        }

        static void DisplayWelcomeScreen()
        {
            DisplayHeader("\t\tWelcome to Our Application");

            DisplayContinue();
        }

        static void DisplayClosingScreen()
        {
            DisplayHeader("\t\tThank you for using this application.");
            DisplayContinue();
        }

        static void DisplayContinue()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }

        static void DisplayHeader(string headertext)
        {
            // display header
            Console.WriteLine();
            Console.WriteLine("\t" + headertext);
            Console.WriteLine();
        }
        static void DisplayMenu()
        {
            bool exiting = false;
            Finch sentryBot = new Finch();
            sentryBot.connect();
            double lowerTempThreshold = 0;
            int upperLightThreshold = 0;

            while (!exiting)
            {
                Console.Clear();

                DisplayHeader("Main Menu");
                Console.WriteLine("1) Display Setup");
                Console.WriteLine("2) Activate Sentry Bot");
                Console.WriteLine("E) Exit");
                Console.WriteLine();
                Console.WriteLine("Enter Menu Choice: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        Console.Clear();
                        lowerTempThreshold = DisplayTempSetup(sentryBot);
                        upperLightThreshold = DisplayLightSetup(sentryBot);
                        break;
                    case "2":
                        Console.Clear();
                        DisplayActivateSentryBot(lowerTempThreshold, upperLightThreshold, sentryBot);
                        break;
                    case "e":
                    case "E":
                        Console.Clear();
                        exiting = true;
                        sentryBot.disConnect();
                        break;
                    default:
                        Console.WriteLine();
                        Console.WriteLine("Please enter a valid menu choice.");
                        DisplayContinue();
                        break;
                }
            }
        }

        static int DisplayLightSetup(Finch sentryBot)
        {
            int ambientLight;
            int upperLightThreshold = 0;
            bool validResponse;

            do
            {
                validResponse = true;

                Console.WriteLine();
                Console.Write("Enter desired change in Light Level: ");

                if (int.TryParse(Console.ReadLine(), out int lightDifference))
                {
                    ambientLight = sentryBot.getRightLightSensor();

                    upperLightThreshold = ambientLight + lightDifference;

                    DisplayContinue();
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Please enter a valid light level");
                    validResponse = false;
                }
            } while (!validResponse);

            return upperLightThreshold;
        }

        static void DisplayActivateSentryBot(double lowerTempThreshold, int upperLightThreshold, Finch sentryBot)
        {
            DisplayHeader("Activate Sentry Bot");

            Console.WriteLine(sentryBot.getTemperature());
            Console.WriteLine(lowerTempThreshold);
            do
            {
                NominalIndicator(sentryBot);

            } while (!TemperatureBelowThresholdValue(lowerTempThreshold, sentryBot) && !LightAboveThresholdValue(upperLightThreshold, sentryBot));

            if (TemperatureBelowThresholdValue(lowerTempThreshold, sentryBot))
            {
                Console.WriteLine();
                Console.WriteLine("The temperature has gone below the threshold value.");
                sentryBot.noteOn(500);
                sentryBot.setLED(255, 0, 0);
                DisplayContinue();
                sentryBot.noteOff();
                sentryBot.setLED(0, 0, 0);
            }
            else if (LightAboveThresholdValue(upperLightThreshold, sentryBot))
            {
                Console.WriteLine();
                Console.WriteLine("The light has exceded the threshold value.");
                sentryBot.noteOn(400);
                sentryBot.setLED(0, 0, 255);
                DisplayContinue();
                sentryBot.noteOff();
                sentryBot.setLED(0, 0, 0);
            }
        }
        
        static double DisplayTempSetup(Finch sentryBot)
        {
            double lowerTempThreshold = 0;
            double ambientTemp;
            bool validResponse;

            DisplayHeader("Sentry Bot Setup");

            do
            {
                validResponse = true;

                Console.Write("Enter desired change in Temperature: ");

                if (double.TryParse(Console.ReadLine(), out double tempDifference))
                {
                    ambientTemp = sentryBot.getTemperature();

                    lowerTempThreshold = ambientTemp - tempDifference;

                    DisplayContinue();
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Please enter a valid temperature");
                    validResponse = false;
                }
            } while (!validResponse);

            return lowerTempThreshold;
        }

        static bool TemperatureBelowThresholdValue(double lowerTempThreshold, Finch sentryBot)
        {
            if (sentryBot.getTemperature() <= lowerTempThreshold)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static bool LightAboveThresholdValue(int upperLightThreshold, Finch sentryBot)
        {
            if (sentryBot.getRightLightSensor() >= upperLightThreshold)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static void NominalIndicator(Finch sentryBot)
        {
            sentryBot.setLED(0, 255, 0);
            sentryBot.wait(500);
            sentryBot.setLED(0, 0, 0);
            sentryBot.wait(100);
        }
    }
}
