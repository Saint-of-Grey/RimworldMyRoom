using System.Collections.Generic;
using System.Linq;
using MyRoom.Common;
using RimWorld;
using Verse;
using Verse.AI;

namespace MyRoom
{
    public class ThinkNode_InstallFurniture : ThinkNode_FurnitureJob
    {
        public override int Commonality()
        {
            return 13;
        }

        public override Job FurnitureJob(Pawn pawn, List<Building_Bed> myBed,
            List<Room> myRoom)
        {
            var minifiedThings = pawn.Map.listerThings.ThingsOfDef(ThingDefOf.MinifiedThing);

            var thingsIWant = minifiedThings.Where(x =>
                    pawn.WantThat(x, myBed)
                    && pawn.CanReserve(x)
                    && InstallBlueprintUtility.ExistingBlueprintFor(x) == null)
                .OrderByDescending(x => x.GetBeautifulValue());
            //order thing installed in my room!
            foreach (var wanted in thingsIWant)
            foreach (var room in myRoom)
            {
                if (room.IsRoomTooNice(pawn)) continue;

                var rot = Rot4.Random;
                var roomCells = room.CellsNotNextToDoorCardinal();
                if (wanted.PlaceThing(pawn, roomCells, rot, room, out var furnitureJob1)) return furnitureJob1;
            }


            return null;
        }
    }
}