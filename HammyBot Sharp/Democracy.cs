// HammyBot Sharp - HammyBot Sharp
//     Copyright (C) 2021 Thomas Duckworth <tduck973564@gmail.com>
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <https://www.gnu.org/licenses/>.

using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HammyBot_Sharp
{/*
    public class Values
    {
        public int MaxAdmins;
        public int MaxMods;
        public int MinMembers;
        public ulong AdminRole;
        public ulong ModRole;
        public ulong ElectoralCommissionRole;
    }

    public class Democracy : Values
    {
        public List<Party> Parties = new List<Party>();
        public Election? CurrentElection;

        public void AddParty(string name, ulong leader)
        {
            Parties.Add(new Party
            {
                Name = name,
                Leader = leader,
                MaxAdmins = MaxAdmins,
                MaxMods = MaxMods,
            });
        }

        public void RemoveParty(string name)
        {
            Parties.Remove(Parties.First((x) => x.Name == name));
        }

        public void StartElection()
        {
            CurrentElection = new Election();
            CurrentElection.RunningParties.AddRange(Parties.FindAll((x) => x.Members.Count > MinMembers));
        }

        public Party EndElection()
        {
            var elected = CurrentElection.Elect();
            CurrentElection = null;

            return elected;
        }
    }

    abstract public class Electable
    {
        // TODO: Implement the electable properties and methods to be overriden
        public class Party : Values
        {
            public string Name;

            public List<ulong> Members = new List<ulong>();
            public ulong Leader;
            public List<ulong> Administrators = new List<ulong>();
            public List<ulong> Moderators = new List<ulong>();

            public List<Vote<ulong>> Votes = new List<Vote<ulong>>();
            public List<ulong> Voted = new List<ulong>();

            public ulong MessageId;

            public void AddVote(ulong user, ulong member)
            {
                if (!Voted.Contains(user))
                {
                    Voted.Add(user);
                    var party = Votes.First((i) => i.VotedFor == member);
                    party.Votes += 1;
                    Votes[Votes.IndexOf(party)] = party;
                }
            }

            public void RemoveVote(ulong user, ulong member)
            {
                if (Voted.Contains(user))
                {
                    Voted.Remove(user);
                    var party = Votes.First((i) => i.VotedFor == member);
                    party.Votes -= 1;
                    Votes[Votes.IndexOf(party)] = party;
                }
            }

            public void Elect()
            {
                Administrators.Clear();
                Moderators.Clear();

                foreach (Vote<ulong> vote in Votes.OrderByDescending(i => i.Votes).ToList().GetRange(0, MaxAdmins))
                {
                    Administrators.Add(vote.VotedFor);
                }

                foreach (Vote<ulong> vote in Votes.OrderByDescending(i => i.Votes).ToList().GetRange(MaxAdmins, MaxMods))
                {
                    Moderators.Add(vote.VotedFor);
                }
            }
        }

        public class Election
        {
            public List<Party> RunningParties = new List<Party>();
            public List<Vote<Party>> Votes = new List<Vote<Party>>();
            public List<ulong> Voted = new List<ulong>();

            public ulong MessageId;

            public Party Elect()
            {
                return Votes.OrderByDescending(i => i.Votes).ToList()[0].VotedFor;
            }

            public void AddVote(ulong user, string partyName)
            {
                if (!Voted.Contains(user))
                {
                    Voted.Add(user);
                    var party = Votes.First((i) => i.VotedFor.Name == partyName);
                    party.Votes += 1;
                    Votes[Votes.IndexOf(party)] = party;
                }
            }

            public void RemoveVote(ulong user, string partyName)
            {
                if (Voted.Contains(user))
                {
                    Voted.Remove(user);
                    var party = Votes.First((i) => i.VotedFor.Name == partyName);
                    party.Votes -= 1;
                    Votes[Votes.IndexOf(party)] = party;
                }
            }
        }
    }

    public class Vote<T>
    {
        public T VotedFor;
        public int Votes;

        public Vote(T votedFor, int votes)
        {
            VotedFor = votedFor;
            Votes = votes;
        }
    }*/
}
