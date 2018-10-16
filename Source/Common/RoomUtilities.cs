using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace MyRoom.Common
{
    public static class RoomUtilities
    {
        public static bool IsRoomTooNice(this Room room, Pawn pawn)
        {
            RoomStatDef roomStatDef = RoomStatDefOf.Impressiveness;
            return IsRoomTooNiceByStat(room, pawn, roomStatDef);
        }

        public static bool IsRoomTooNiceByStat(this Room room, Pawn pawn, RoomStatDef roomStatDef)
        {
            int scoreStageIndex = roomStatDef.GetScoreStageIndex(room.GetStat(roomStatDef));
            return scoreStageIndex > pawn.MinWantedNice();
        }


        public static List<IntVec3> CellsNotNextToDoorCardinal(this Room room)
        {
            var doors = new List<Building_Door>();

            foreach (var thing in room.ContainedAndAdjacentThings)
            {
                if (thing is Building_Door door)
                {
                    doors.Add(door);
                }
            }

            var cells = room.Cells.ToList();
            foreach (var buildingDoor in doors)
            {
                foreach (var cell in GenAdj.CellsAdjacentCardinal(buildingDoor))
                {
                    cells.Remove(cell);
                }
            }
            
            return cells;
        }
    }
}