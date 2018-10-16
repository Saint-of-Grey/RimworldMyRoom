using MyRoom.Common;
using Verse;
using Verse.AI;

namespace MyRoom
{
    public class JobGiver_StealFurniture : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            var mentalState = pawn.MentalState as MentalState_StealToRoom;
            if (mentalState?.target == null) return null;

            var myBed = pawn.MyBeds();
            var myRoom = RoomUtilities.MyRoom(myBed);
            foreach (var room in myRoom)
                if (mentalState.target.PlaceThing(pawn, room.CellsNotNextToDoorCardinal(),
                    mentalState.target.def.rotatable ? Rot4.Random : Rot4.North,
                    room, out var furnitureJob1))
                    return furnitureJob1;


            return null;
        }
    }
}