using RollingOutTools.CmdLine;
using RollingOutTools.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicornOutsourceApp
{

    public class MainCmd : CommandLineBase
    {
        string[] StatsArray = { "Strength", "Agility", "Speed", "Intelligance", "Charm" };
        Settings SettingsProp;
        Random random = new Random();

        public MainCmd(ICmdSwitcher cmdSwitcher, CmdLineExtension cmdLineExtension = null) : base(cmdSwitcher, cmdLineExtension)
        {
            SettingsProp = StorageHardDrive.Get<Settings>("all_settings").Result ?? new Settings();
        }

        [CmdInfo]
        public void Settings()
        {
            CmdLineExtension.Inst.WriteLine("Current settings:");
            CmdLineExtension.Inst.WriteLine(SettingsProp,null, true);
            SettingsProp = ReadResource<Settings>("all_settings");
            StorageHardDrive.Set("all_settings", SettingsProp);
        }

        [CmdInfo]
        public void Go()
        {
            int iterationsCount = ReadResource<int>(
                "iterations_count",
                new ReadResourseOptions { Hint = "Введите число турниров." }
                );
            int unicornsInMatch = ReadResource<int>(
               "unicorns_in_match",
               new ReadResourseOptions { Hint = "Введите число единорогов в турнире." }
               );

            //Заполнение списка единорогов
            CmdLineExtension.Inst.WriteLine("Пример");
            CmdLineExtension.Inst.WriteLine(new Unicorn(), null, true);
            var unicornArray = ReadResource<List<Unicorn>>("unicorns");
            if (unicornArray.Count != unicornsInMatch)
            {
                throw new Exception($"Количество единорогов должно быть {unicornsInMatch}.");
            }


            for (int k = 0; k < unicornArray.Count; k++)
            {
                unicornArray[k].TournamentWins = 0;
            }

            StringBuilder stringBuilder = new StringBuilder();            
            for (int iterationNum = 0; iterationNum < iterationsCount; iterationNum++)
            {
                var matchesVariants = GetRoundStats(SettingsProp.MatchesInTournament);
                for (int k = 0; k < unicornArray.Count; k++)
                {
                    unicornArray[k].SumBuf_Tournament = 0;
                }

                string tournamentSumStr = "Tournament sum:;";
                for (int i = 0; i < SettingsProp.MatchesInTournament; i++)
                {
                    for (int k = 0; k < unicornArray.Count; k++)
                    {
                        unicornArray[k].SumBuf_Match = 0;
                    }

                    string matchSumStr = "Match sum:;";
                    var getMethod_Primary = typeof(Unicorn).GetProperty(matchesVariants[i].Primary).GetMethod;
                    var getMethod_Secondary = typeof(Unicorn).GetProperty(matchesVariants[i].Secondary).GetMethod;

                    for (int roundNum = 0; roundNum < SettingsProp.RoundsInMatch; roundNum++)
                    {
                        string roundSumStr = $"Round{roundNum} sum:;";
                        for (int k = 0; k < unicornArray.Count; k++)
                        {
                            int primaryValue = ((int)getMethod_Primary.Invoke(unicornArray[k], new object[0]));
                            int secondaryValue = ((int)getMethod_Secondary.Invoke(unicornArray[k], new object[0]));
                            double primaryValueRand =random.Next(1, primaryValue) * SettingsProp.PrimaryModifier;
                            double secondaryValueRand = random.Next(1, secondaryValue) * SettingsProp.SecondaryModifier;
                            double roundSum = primaryValueRand + secondaryValueRand;
                            roundSum *= 100;

                            //Round sum modifiers
                            int addedSum=AttachModifiers(unicornArray[k]);
                            //Round sum modifiers
                            roundSum += addedSum;
                            unicornArray[k].SumBuf_Round = roundSum;
                            roundSumStr += $"=\"{roundSum-addedSum} + {addedSum} = {roundSum}\";";

                            unicornArray[k].SumBuf_Match += roundSum;
                            unicornArray[k].SumBuf_Tournament += roundSum;
                        }
                        roundSumStr += matchesVariants[i].Primary + ";" + matchesVariants[i].Secondary + ";";
                        if (SettingsProp.LogRounds && SettingsProp.LogMatches)
                        {
                            stringBuilder.Append(roundSumStr);
                            stringBuilder.AppendLine();
                        }
                    }
                    for (int k = 0; k < unicornArray.Count; k++)
                    {
                        matchSumStr += unicornArray[k].SumBuf_Match + ";";
                    }

                    if (SettingsProp.LogMatches)
                    {
                        stringBuilder.Append(matchSumStr);
                        stringBuilder.AppendLine();
                    }


                }

                for (int k = 0; k < unicornArray.Count; k++)
                {
                    tournamentSumStr += unicornArray[k].SumBuf_Tournament + ";";
                }

                stringBuilder.Append(tournamentSumStr);
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();

                double maxSum = 0;
                int maxIndex = 0;
                for (int k = 0; k < unicornArray.Count; k++)
                {
                    if (unicornArray[k].SumBuf_Tournament > maxSum)
                    {
                        maxSum = unicornArray[k].SumBuf_Tournament;
                        maxIndex = k;
                    }
                }
                unicornArray[maxIndex].TournamentWins++;
            }

            string percentCountStr = "Winrate:;";            
            for (int k = 0; k < unicornArray.Count; k++)
            {
                double winrate = ((double)(unicornArray[k].TournamentWins * 100) / (double)iterationsCount);
                percentCountStr += winrate.ToString("00.00") +"%;";
            }
            stringBuilder.Insert(0, percentCountStr + "\n");

            string winsCountStr = "Wins:;";
            for (int k = 0; k < unicornArray.Count; k++)
            { 
                winsCountStr += unicornArray[k].TournamentWins + ";";
            }
            stringBuilder.Insert(0,winsCountStr+"\n");

            string title = "Name of test;";
            for (int k = 0; k < unicornArray.Count; k++)
            {
                title += $"Unicorn #{k};";
            }
            title += "Primary;Secondary;\n";
            stringBuilder.Insert(0, title);

            CmdLineExtension.Inst.WriteLine((title + "\n" + winsCountStr + "\n" + percentCountStr).Replace(";","\t"));
            File.WriteAllText(
                "stats.csv",
                stringBuilder.ToString()
                );

            if (iterationsCount < 100000)
            {
                Process.Start("stats.csv");
            }

           
        }

        /// <summary>
        /// </summary>
        /// <param name="unicorn"></param>
        /// <returns>Additional sum</returns>
        public int AttachModifiers(Unicorn unicorn)
        {
            double chance = unicorn.PartsOfBody.Uncommon * SettingsProp.CritModifier;
            bool uncommonRoll = chance > random.Next(100);
            chance = unicorn.PartsOfBody.Rare * SettingsProp.CritModifier;
            bool rareRoll = chance > random.Next(100);
            chance = unicorn.PartsOfBody.Epic * SettingsProp.CritModifier;
            bool epicRoll = chance > random.Next(100);
            chance = unicorn.PartsOfBody.Legendary * SettingsProp.CritModifier;
            bool legendaryRoll = chance > random.Next(100);

            int addedSum = 0;
            if (uncommonRoll)
            {
                addedSum += SettingsProp.UncommonScore;
            }
            if (rareRoll)
            {
                addedSum += SettingsProp.RareScore;
            }
            if (epicRoll)
            {
                addedSum += SettingsProp.EpicScore;
            }
            if (legendaryRoll)
            {
                addedSum += SettingsProp.LegendaryScore;
            }

            return addedSum;
        }

        public List<RoundStats> GetRoundStats(int roundesCount)
        {
            var roundes = new List<RoundStats>();
            int i = 0;            
            while( i < roundesCount)
            {
                int stat1Num = random.Next(StatsArray.Length);
                int stat2Num = -1;
                do
                {
                    stat2Num = random.Next(StatsArray.Length);
                }
                while (stat1Num == stat2Num);

                var tupleOfRound = new RoundStats(StatsArray[stat1Num], StatsArray[stat2Num]);
                if (roundes.Contains(tupleOfRound))
                    continue;
                roundes.Add(tupleOfRound);
                i++;
            }

            return roundes;
        }
    }
}