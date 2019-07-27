using ASU2019_NetworkedGameWorkshop.controller;
using ASU2019_NetworkedGameWorkshop.model.character;
using System;
using System.Collections.Generic;

namespace ASU2019_NetworkedGameWorkshop.model.spell.types
{
    public class Target
    {

        public Character caster { get; set; }
        public List<Character> recievers { get; set; }
        public bool MultiTargeted { get; private set; }
        public int numberOfTargets { get; private set; }
        public bool Ally { get; private set; }
        public PriorityType PriorityType { get; private set; }

        public Target(bool Ally, PriorityType priorityType)
        {
            recievers = new List<Character>();
            this.Ally = Ally;
            MultiTargeted = false;
            this.numberOfTargets = 1;
            PriorityType = priorityType;
        }
        public Target(bool Ally, bool aOE, int numberOfTargets, PriorityType priorityType)
        {
            recievers = new List<Character>();
            this.Ally = Ally;
            MultiTargeted = aOE;
            this.numberOfTargets = numberOfTargets;
            PriorityType = priorityType;
        }
        public List<Character> getTargets()
        {
            int range = caster.CharacterType.statsCopy()[StatusType.Range];
            List<Character> enemyList = (caster.team == Character.Teams.Red) ? caster.getGameManager().TeamBlue : caster.getGameManager().TeamRed;
            enemyList = charactersInRange(enemyList, caster);
            List<Character> teamMateList = (caster.team == Character.Teams.Red) ? caster.getGameManager().TeamRed : caster.getGameManager().TeamBlue;
            teamMateList = charactersInRange(teamMateList, caster);
            List<Character> desiredTeam;
            if (Ally)
            {
                desiredTeam = teamMateList;
            }
            else
            {
                desiredTeam = enemyList;
            }
            for(int i = 0; i < numberOfTargets; i++)
            {
                Character newTarget;
                switch (PriorityType)
                {
                    case PriorityType.Self:
                        recievers.Add(caster);
                        return recievers;
                    case PriorityType.Current:
                        newTarget = PathFinding.findClosestEnemy(caster.CurrentTile, desiredTeam, caster.getGrid(), range, caster.getGameManager());
                        recievers.Add(newTarget);
                        desiredTeam.Remove(newTarget);
                        break;
                    case PriorityType.Random:
                        Random random = new Random();
                        int index = random.Next(desiredTeam.Count);
                        recievers.Add(desiredTeam[index]);
                        break;
                    case PriorityType.LowHealth:
                        Character lowChar = desiredTeam[0];
                        foreach (Character character in desiredTeam)
                        {
                            if (character.Stats[StatusType.HealthPoints] < lowChar.Stats[StatusType.HealthPoints])
                            {
                                lowChar = character;
                            }
                        }
                        recievers.Add(lowChar);
                        desiredTeam.Remove(lowChar);
                        break;
                }
            }
            return recievers;
        }
        


        public List<Character> charactersInRange(List<Character> team, Character caster)
        {
            List<Character> removedList = new List<Character>();
            int range = caster.CharacterType.statsCopy()[StatusType.Range];
            foreach (Character character in team)
            {
                if (PathFinding.getDistance(caster.CurrentTile, character.CurrentTile) > range)
                {
                    removedList.Add(character);
                }
            }
            foreach (Character character in removedList)
            {
                if (PathFinding.getDistance(caster.CurrentTile, character.CurrentTile) <= range)
                {
                    team.Remove(character);
                }
            }
            return team;
        }

    }
    
}
