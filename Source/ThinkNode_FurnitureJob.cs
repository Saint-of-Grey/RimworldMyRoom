using System.Collections.Generic;
using System.Linq;
using MyRoom.Common;
using RimWorld;
using Verse;
using Verse.AI;

namespace MyRoom
{
    public abstract class ThinkNode_FurnitureJob : ThinkNode_JobGiver
    {
        private static short _tick = 0;

        protected override Job TryGiveJob(Pawn pawn)
        {
            _tick += 1;
            _tick %= 29387;
            //semi-rare tick
            if (_tick % Commonality() != 0
                || !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation)
                || !pawn.RaceProps.ToolUser
                || pawn.IsPrisoner)
                return null;


            var myBed = pawn.MyBeds();

            var myRoom = RoomUtilities.MyRoom(myBed);

            if (myRoom == null || !myRoom.Any()) return null;

            return FurnitureJob(pawn, myBed, myRoom);
        }

        public abstract int Commonality();

        public abstract Job FurnitureJob(Pawn pawn, List<Building_Bed> myBed,
            List<Room> myRoom);
    }
}