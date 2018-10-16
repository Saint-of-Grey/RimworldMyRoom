using RimWorld;
using Verse;
using Verse.AI;

namespace MyRoom.Common
{
    public static class BlueprintUtilities
    {
        public static Blueprint_Install BlueprintInstall(this Thing wanted, Pawn pawn, IntVec3 vec3, Room room,
            Rot4 rot)
        {
            Blueprint_Install bp;
            if (wanted is MinifiedThing minifiedThing)
            {
                bp = GenConstruct.PlaceBlueprintForInstall(minifiedThing, vec3, room.Map, rot,
                    pawn.Faction);
            }
            else
            {
                bp = GenConstruct.PlaceBlueprintForReinstall((Building) wanted, vec3, room.Map, rot,
                    pawn.Faction);
            }

            return bp;
        }

        public static Job InstallJob(this Blueprint_Install install, Pawn pawn)
        {
            var miniToInstallOrBuildingToReinstall = install.MiniToInstallOrBuildingToReinstall;
            if (miniToInstallOrBuildingToReinstall.IsForbidden(pawn))
            {
                return null;
            }

            if (!pawn.CanReach(miniToInstallOrBuildingToReinstall, PathEndMode.ClosestTouch, pawn.NormalMaxDanger()))
            {
                return null;
            }

            if (!pawn.CanReserve(miniToInstallOrBuildingToReinstall, 1, -1, null, false))
            {
                return null;
            }

            return new Job(JobDefOf.HaulToContainer)
            {
                targetA = miniToInstallOrBuildingToReinstall,
                targetB = install,
                count = 1,
                haulMode = HaulMode.ToContainer
            };
        }
    }
}