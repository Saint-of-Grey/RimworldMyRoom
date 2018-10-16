//#define DEBUG
using System.Collections.Generic;
using MyRoom.Common;
using RimWorld;
using Verse;
using Verse.AI;

namespace MyRoom
{
    public class ThinkNode_MoveMyFurniture : ThinkNode_FurnitureJob
    {
        public override int Commonality()
        {
            return 4347;
        }

        public override Job FurnitureJob(Pawn pawn, List<Building_Bed> myBed,
            List<Room> myRoom)
        {
            foreach (var room in myRoom)
            {
                var movable = new List<Thing>();
                foreach (var thing in room.ContainedAndAdjacentThings)
                {
                    if (thing.def.Minifiable
                        && pawn.CanReserve(thing)
                        && InstallBlueprintUtility.ExistingBlueprintFor(thing) == null)
                    {
                        movable.Add(thing);
                        //consider moving it
                    }
                }

                var cells = room.CellsNotNextToDoorCardinal();
                foreach (var thing in movable.InRandomOrder())
                {
                    if (thing is Building_Bed) continue;

                    if (thing.PlaceThing(pawn, cells,
                        thing.def.rotatable ? Rot4.Random : Rot4.North,
                        room, out var furnitureJob1)) return furnitureJob1;
                }
            }

            return null;
        }
    }
}