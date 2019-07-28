using ASU2019_NetworkedGameWorkshop.controller;
using ASU2019_NetworkedGameWorkshop.model.character;
using System;
using System.Collections.Generic;

namespace ASU2019_NetworkedGameWorkshop.model.spell.types
{
    public class Target
    {
        private readonly Random random;

        public Character Caster { get; set; }
        public List<Character> Recievers { get; set; }
        public bool MultiTargeted { get; private set; }
        public int NumberOfTargets { get; private set; }
        public bool TargetAlly { get; private set; }
        public CastTarget CastTarget { get; private set; }

        public Target(bool TargetAlly, CastTarget castTarget) : this(TargetAlly, false, 1, castTarget)
        { }

        public Target(bool TargetAlly, bool aOE, int numberOfTargets, CastTarget castTarget)
        {
            Recievers = new List<Character>();
            this.TargetAlly = TargetAlly;
            MultiTargeted = aOE;
            NumberOfTargets = numberOfTargets;
            CastTarget = castTarget;
            random = new Random();
        }
        public List<Character> getTargets()
        {
            List<Character> desiredTeam = TargetAlly ? charactersInRange((Caster.team == Character.Teams.Red) ? Caster.gameManager.TeamRed : Caster.gameManager.TeamBlue, Caster)
                : charactersInRange((Caster.team == Character.Teams.Red) ? Caster.gameManager.TeamBlue : Caster.gameManager.TeamRed, Caster);

            switch (CastTarget)
            {
                case CastTarget.Self:
                    Recievers.Add(Caster);
                    break;
                case CastTarget.CurrentTarget:
                    for (int i = 0; i < NumberOfTargets; i++)
                    {
                        Character newTarget = PathFinding.findClosestEnemy(Caster.CurrentTile,
                                                                           desiredTeam,
                                                                           Caster.grid,
                                                                           Caster.CharacterType[StatusType.Range],
                                                                           Caster.gameManager);
                        Recievers.Add(newTarget);
                        desiredTeam.Remove(newTarget);
                    }
                    break;
                case CastTarget.Random:
                    for (int i = 0; i < NumberOfTargets; i++)
                    {
                        Recievers.Add(desiredTeam[random.Next(desiredTeam.Count)]);
                    }
                    break;
                case CastTarget.LowHealth:
                    for (int j = 0; j < NumberOfTargets; j++)
                    {
                        if (desiredTeam.Count > 0)
                        {
                            Character lowChar = desiredTeam[0];
                            for (int i = 0; i < desiredTeam.Count; i++)
                            {
                                Character character = desiredTeam[i];
                                if (character.Stats[StatusType.HealthPoints] < lowChar.Stats[StatusType.HealthPoints])
                                {
                                    lowChar = character;
                                }
                            }
                            Recievers.Add(lowChar);
                            desiredTeam.Remove(lowChar);
                        }
                    }
                    break;
            }
            return Recievers;
        }

        public List<Character> charactersInRange(List<Character> team, Character caster)
        {
            int range = caster.CharacterType[StatusType.Range];
            List<Character> inRange = new List<Character>();
            foreach (Character character in team)
            {
                if (PathFinding.getDistance(caster.CurrentTile, character.CurrentTile) <= range)
                {
                    inRange.Add(character);
                }
            }
            return inRange;
        }

    }

}
