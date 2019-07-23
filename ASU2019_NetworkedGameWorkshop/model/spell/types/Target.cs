﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASU2019_NetworkedGameWorkshop.controller;
using ASU2019_NetworkedGameWorkshop.model.character;

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
            PriorityType = priorityType;
        }
        public Target(bool Ally, bool aOE,int numberOfTargets, PriorityType priorityType)
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
            if (!Ally)

            {
                if (MultiTargeted == false)
                {
                    if (PriorityType == PriorityType.Current)
                    {
                        recievers.Add(caster.currentTarget);
                        return recievers;
                    }
                    if (PriorityType == PriorityType.Random)
                    {
                        Random random = new Random();
                        List<Character> enemyList = (caster.team == Character.Teams.Red) ? caster.getGameManager().TeamBlue : caster.getGameManager().TeamRed;
                        List<Character> removedList = new List<Character>();
                        foreach (Character character in enemyList)
                        {
                            if(PathFinding.getDistance(caster.CurrentTile, character.CurrentTile) > range)
                            {
                                removedList.Add(character);
                            }
                        }
                        foreach (Character character in removedList)
                        {
                            if (PathFinding.getDistance(caster.CurrentTile, character.CurrentTile) <= range)
                            {
                                enemyList.Remove(character);
                            }
                        }
                        int index = random.Next(enemyList.Count);
                        recievers.Add(enemyList[index]);
                    }
                }
                if(MultiTargeted == true)
                {
                    if(PriorityType == PriorityType.Current)
                    {
                        recievers.Add(caster.currentTarget);
                        Character newTarget;
                        for(int i = 0; i < numberOfTargets; i++)
                        {
                            (_, newTarget) = PathFinding.findClosestEnemy(caster.currentTarget.CurrentTile, caster.team, caster.getGrid(), range,caster.getGameManager());
                            recievers.Add(newTarget);
                        }
                    }
                }
            }
            else
            {
                if (MultiTargeted == false)
                {
                    if (PriorityType == PriorityType.Random)
                    {
                        Random random = new Random();
                        List<Character> teamMateList = (caster.team == Character.Teams.Red) ? caster.getGameManager().TeamRed : caster.getGameManager().TeamBlue;
                        List<Character> removedList = new List<Character>();
                        foreach (Character character in teamMateList)
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
                                teamMateList.Remove(character);
                            }
                        }
                        int index = random.Next(teamMateList.Count);
                        recievers.Add(teamMateList[index]);
                    }
                }
                if (MultiTargeted == true)
                {
                    if (PriorityType == PriorityType.Current)
                    {
                        recievers.Add(caster.currentTarget);
                        Character newTarget;
                        for (int i = 0; i < numberOfTargets; i++)
                        {
                            (_, newTarget) = PathFinding.findClosestEnemy(caster.currentTarget.CurrentTile, caster.currentTarget.team, caster.getGrid(), range,caster.getGameManager());
                            recievers.Add(newTarget);
                        }
                    }
                }
            }
            return recievers;
            
        }
        
    }
}