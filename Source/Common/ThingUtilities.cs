//#define DEBUG
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace MyRoom.Common
{
    public static class ThingUtilities
    {
        public static bool IsPretty(this Thing x)
        {
            return x.GetInnerIfMinified().GetStatValue(StatDefOf.Beauty) > 10f;
        }

        public static float GetBeautifulValue(this Thing x)
        {
            return x?.GetInnerIfMinified().GetStatValue(StatDefOf.Beauty) ?? 0f;
        }
        
        

        public static bool IsBetterBed(this Thing bed, Pawn pawn, List<Building_Bed> myBed)
        {
            return false;
        }

        public static bool PlaceThing(this Thing wanted, Pawn pawn, IEnumerable<IntVec3> roomCells, Rot4 rot, Room room,
            out Job furnitureJobResult)
        {
            rot = wanted.def.rotatable ? rot: Rot4.North;
            foreach (var vec3 in roomCells.InRandomOrder())
            {
                if (!GenConstruct.CanPlaceBlueprintAt(wanted.GetInnerIfMinified().def, vec3, rot, room.Map)
                    .Accepted)
                {
#if DEBUG
                        Log.Message("Not Place-able");
#endif
                    continue;
                }

                var bp = wanted.BlueprintInstall(pawn, vec3, room, rot);

                if (bp == null)
                {
#if DEBUG
                        Log.Message("Couldn't place blueprint, oops");
#endif
                    continue;
                }

                var job = bp.InstallJob(pawn);  

                if (job != null)
                {
                    {
                        furnitureJobResult = job;
                        return true;
                    }
                }
#if DEBUG
                    Log.Message("No job for bp");
#endif
            }

            furnitureJobResult = null;
            return false;
        }
    }
}