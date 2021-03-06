﻿using Discord;
using Discord.Commands;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

{
    //TODO: classes for each person (doubtful), nickname changes for fun, utility (deleting)
    class MyBot
    {
        DiscordClient discord;
        CommandService commands;
        //this should initialize the meme database and RNG
        List<string> memes = File.ReadAllLines("C:\\Users\\Paddy\\Documents\\Projects\\DiscordBot\\Memes.txt").ToList();
        List<string> animemes = File.ReadAllLines("C:\\Users\\Paddy\\Documents\\Projects\\DiscordBot\\Animes.txt").ToList();
        Random rng;
        int haikuCount;

        public MyBot()
        {
            rng = new Random();
            haikuCount = 0;

            discord = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });

            //the bot either accepts mentions or exclamation marks as a prefix to it's commands
            discord.UsingCommands(x =>
            {
                x.PrefixChar = '!';
                x.AllowMentionPrefix = true;
            });

            commands = discord.GetService<CommandService>();
            
            //yeah yeah I know that this could all be slimmed down with classes but whatever
            //either way these are the descriptions of everyone with images
            commands.CreateCommand("Christian")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("Dweeb who unironically likes visual novels");
                });

            commands.CreateCommand("Jacob")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("The only person who's watched more one piece than Kevin Gallagher");
                    await e.Channel.SendMessage("http://i.imgur.com/C2HNQ2s.png");
                });

            commands.CreateCommand("Jeffrey")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("How did we let this guy be admin?");
                    await e.Channel.SendMessage("https://cdn.discordapp.com/attachments/158721080813420544/256994569122480128/20150825_131902.jpg");
                });

            commands.CreateCommand("Paddy")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("Did you mean to say \"!meg \"?");
                    await e.Channel.SendMessage("http://i.imgur.com/9sHddY7.png");
                });

            commands.CreateCommand("Ryan")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("Washed up CS player who has a \"normal\" shoe size");
                    await e.Channel.SendMessage("http://i.imgur.com/Okofs8D.png");
                });

            commands.CreateCommand("Ennis")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("Put your **** away!");
                    await e.Channel.SendMessage("http://i.imgur.com/wfRbBAN.png");
                });

            commands.CreateCommand("Daniel")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("Slashing ***** since 1998");
                });

            commands.CreateCommand("I gotta move here")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("It'll bring down the house");
                    await e.Channel.SendMessage("http://i.imgur.com/p4KhA0y.png");
                });

            commands.CreateCommand("Liam")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("Melee is a children's party game and fails as a competitive one");
                    await e.Channel.SendMessage("http://i.imgur.com/FAa3zon.png");
                });

            //now everything is pretty much done through function calls because that's nicer
            AddMeme();
            AddAnime();
            OutputMeme();
            OutputAnime();
            OutputMemeSize();
            OutputAnimeSize();
            Roulette();
            OutputNewestMeme();
            OutputHaikuCount();
            OutputOGMemeCount();
            //connects bot to all the servers it's in
            discord.ExecuteAndWait(async () =>
            {
                await discord.Connect("MjU2OTU5NDkyNzE2MTAxNjMy.CyzwFQ.qIGLgrLGIAW7YpJL58vrn_qtmx4", TokenType.Bot);
            });
        }

        //takes most recent message before command and stores it as a new anime
        private void AddAnime()
        {
            commands.CreateCommand("add anime")
                .Do(async (e) =>
                {
                    String newAnime;
                    Message[] x;

                    x = await e.Channel.DownloadMessages(2);
                    newAnime = x[1].RawText;
                    if (!newAnime.Contains("csgoani.me"))
                    {
                        await e.Channel.SendMessage("Why would you waste a good meme like that?");
                        await e.Channel.SendMessage("I'm not putting any memes in the bad database");
                    }
                    else if (animemes.Contains(newAnime))
                    {
                        await e.Channel.SendMessage("Anime was a mistake and your reposting only makes it worse");
                    }
                    else
                    {
                        animemes.Add(newAnime);
                        File.WriteAllLines("C:\\Users\\Paddy\\Documents\\Projects\\DiscordBot\\Animes.txt", animemes);
                        await e.Channel.SendMessage("You're mean for doing this to me...");
                        await e.Channel.SendMessage("Anime added (please stop)");
                    }
                });
        }

        //takes most recent message before command and stores it as a meme (it should always be a url but I guess copy pastas can work)
        private void AddMeme()
        {
            commands.CreateCommand("add meme")
                .Do(async (e) =>
                {
                    String newMeme;
                    Message[] x;

                    x = await e.Channel.DownloadMessages(2);
                    newMeme = x[1].RawText;
                    if (newMeme.Contains("csgoani.me"))
                    {
                        await e.Channel.SendMessage("Our memes are too pure for that bad");
                    }
                    else if (memes.Contains(newMeme))
                    {
                        await e.Channel.SendMessage("That's an old meme you dip!");
                    }
                    else if (newMeme.Contains("scontent.xxx.fbcdn"))
                    {
                        await e.Channel.SendMessage("we don't accept links of those type since they expire, please take a screenshot and use that");
                    }
                    else if (haikuCount == 5 && (newMeme.Contains("youtube.com") || newMeme.Contains("youtu.be")))
                    {
                        await e.Channel.SendMessage("Max haikus reached for the day. Come back tomorrow buddy");
                    }
                    else
                    {
                        memes.Add(newMeme);
                        File.WriteAllLines("C:\\Users\\Paddy\\Documents\\Projects\\DiscordBot\\Memes.txt", memes);
                        await e.Channel.SendMessage("Meme added");
                        
                        //checks if it's a youtube video
                        if (newMeme.Contains("youtube.com") || newMeme.Contains("youtu.be"))
                        {
                            haikuCount++;
                            if (haikuCount == 5)
                            {
                                await e.Channel.SendMessage("That's the last haiku for today");
                            }
                            else if (haikuCount > 1)
                            {
                                await e.Channel.SendMessage("There have been " + haikuCount + " youtube haikus submitted today (5 allowed per day)");
                            }
                            else
                            {
                                await e.Channel.SendMessage("There has been " + haikuCount + " youtube haiku submitted today (5 allowed per day)");
                            }
                        }
                    }
                });
        }

        //outputs bad
        private void OutputAnime()
        {
            commands.CreateCommand("anime")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage(animemes[rng.Next(animemes.Count)]);
                });
        }

        //outputs a random meme
        private void OutputMeme()
        {
            commands.CreateCommand("meme")
                .Do(async (e) =>
                {
		    await e.Channel.SendMessage("woohoo, this is my purpose!!");
                    await e.Channel.SendMessage(memes[rng.Next(memes.Count)]);
                });
        }

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        //outputs the current size of the meme database
        private void OutputMemeSize()
        {
            commands.CreateCommand("how big")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("There are currently " + memes.Count.ToString() + " dank spicy memes in the meme database");
                });
        }

        //gets the amount of youtube haikus in the meme database
        //implimented in OutputOGMemeCount()
        private int GetHaikuCount()
        {
            int counter = 0;

            for (int i = 0; i < memes.Count; i++)
            {
                if (memes[i].Contains("youtube.com") || memes[i].Contains("youtu.be"))
                {
                    counter++;
                }
            }
            return counter;
        }

        //outputs the amount of OG memes in the meme database
        private void OutputOGMemeCount()
        {
            commands.CreateCommand("how meme")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("there are currently " + (memes.Count - GetHaikuCount()).ToString() + " OG funny guy memes in the meme database");
                    await e.Channel.SendMessage("I'm proud of all of you");
                });
        }

        //outputs the amount of youtube haikus in the meme database
        private void OutputHaikuCount()
        {
            commands.CreateCommand("how many haikus")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("there are currently " + GetHaikuCount().ToString() + " youtube haikus in the meme database");
                    await e.Channel.SendMessage("Remember, \"5 a day and I love being gay\" - Paddy");
                });
        }

        //outputs the amount of cancer we've accumulated
        private void OutputAnimeSize()
        {
            commands.CreateCommand("how weeb")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("I currently have " + animemes.Count.ToString() + " bad (csgoanimes) in my body");
                });
        }

        //plays a game of roulette
        private void Roulette()
        {
            commands.CreateCommand("roulette")
                .Do(async (e) =>
                {
                    string name;
                    Message[] newMessage;

                    newMessage = await e.Channel.DownloadMessages(1);
                    name = newMessage[0].User.Name;
                    await e.Channel.SendMessage(name + " is  playing roulette");

                    if (rng.Next(6) == 0)
                    {
                        await e.Channel.SendMessage("after pulling the trigger " + name + " died instantly as the bullet was in the chamber");
                        await e.Channel.SendMessage("I wish he took me with him");
                    }
                    else
                    {
                        await e.Channel.SendMessage("sucks for you " + name + " looks like you didn't die");
                        await e.Channel.SendMessage("better luck next time pal");
                    }
                });
        }

        //outputs the newest meme
        private void OutputNewestMeme()
        {
            commands.CreateCommand("newest meme")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage(memes[memes.Count - 1]);
                    await e.Channel.SendMessage("this is the freshest meme we have");
                });
        }
    }
}
