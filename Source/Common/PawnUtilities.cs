using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace MyRoom.Common
{
    public static class PawnUtilities
    {
        public static List<Building_Bed> MyBeds(this Pawn pawn)
        {
            var myBed = pawn.Map.listerBuildings.allBuildingsColonist
                .OfType<Building_Bed>()
                .Where(x => x.AssignedPawns.Contains(pawn)).ToList();
            //allRooms = pawn.Map.regionGrid.allRooms;
            return myBed;
        }

        public static int MinWantedNice(this Pawn pawn)
        {
            if (pawn.story.traits.HasTrait(TraitDefOf.Ascetic))
            {
                return -1;
            }
            else if (IsGreedy(pawn))
            {
                return Int16.MaxValue;
            }

            return 5; //todo move to setting
        }


        public static int MaxWantedNice(this Pawn pawn)
        {
            if (pawn.story.traits.HasTrait(TraitDefOf.Ascetic))
            {
                return -1;
            }

            return Int16.MaxValue;
        }

        private static bool IsGreedy(this Pawn pawn)
        {
            return pawn.story.traits.HasTrait(TraitDefOf.Greedy);
        }

        public static bool WantThat(this Pawn pawn, Thing x, List<Building_Bed> myBed)
        {
            return x.IsPretty() || ThingUtilities.IsBetterBed(x, pawn, myBed);
        }
    }
}