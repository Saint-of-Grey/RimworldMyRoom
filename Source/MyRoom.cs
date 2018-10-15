//#define DEBUG

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;


namespace MyRoom
{
    public static class Helpers
    {
        public static float GetBeautifulValue(this Thing x)
        {
            return x?.GetInnerIfMinified().GetStatValue(StatDefOf.Beauty) ?? 0f;
        }
    }

    public class ThinkNode_InstallFurniture : ThinkNode_JobGiver
    {
        private static short _tick = 0;

        protected override Job TryGiveJob(Pawn pawn)
        {
            _tick += 1;
            _tick %= 3567;
            //semi-rare tick
            if (_tick % 13 != 0
                || !pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation)
                || !pawn.RaceProps.ToolUser)
                return null;


            var myBed = pawn.Map.listerBuildings.allBuildingsColonist
                .OfType<Building_Bed>()
                .Where(x => x.AssignedPawns.Contains(pawn)).ToList();
            //allRooms = pawn.Map.regionGrid.allRooms;

            var myRoom = myBed?.Select(x => x.GetRoom()).ToList();

            if (!myRoom.Any()) return null;

            var minifiedThings = pawn.Map.listerThings.ThingsOfDef(ThingDefOf.MinifiedThing);

            var thingsIWant = minifiedThings.Where(x =>
                    WantThat(pawn, x, myBed)
                    && pawn.CanReserve(x)
                    && InstallBlueprintUtility.ExistingBlueprintFor(x) == null)
                .OrderByDescending(x => x.GetBeautifulValue());
            //order thing installed in my room!
            foreach (var wanted in thingsIWant)
            foreach (var room in myRoom)
            {
                int scoreStageIndex =
                    RoomStatDefOf.Impressiveness.GetScoreStageIndex(room.GetStat(RoomStatDefOf.Impressiveness));
                if (scoreStageIndex>maxNice(pawn)) 
                {
#if DEBUG
                    Log.Message("Too Nice score : "+RoomStatDefOf.Impressiveness.scoreStages[scoreStageIndex].label);
#endif
                    continue;
                }

                var rot = Rot4.Random;
                foreach (var vec3 in room.Cells.InRandomOrder())
                {
                    if (!GenConstruct.CanPlaceBlueprintAt(wanted.GetInnerIfMinified().def, vec3, rot, room.Map)
                        .Accepted)
                    {
#if DEBUG
                        Log.Message("Not Place-able");
#endif
                        continue;
                    }

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

                    if (bp == null)
                    {
#if DEBUG
                        Log.Message("Couldn't place blueprint, oops");
#endif
                        continue;
                    }
                    var job = InstallJob(pawn, bp);

                    if (job != null)
                    {
                        return job;
                    }
#if DEBUG
                    Log.Message("No job for bp");
#endif
                }
            }


            return null;
        }

        private static bool WantThat(Pawn pawn, Thing x, List<Building_Bed> myBed)
        {
            return IsPretty(x) || IsBetterBed(pawn, x, myBed);
        }

        private static bool IsBetterBed(Pawn pawn, Thing thing, List<Building_Bed> myBed)
        {
            return false;
        }

        private int maxNice(Pawn pawn)
        {
            if (pawn.story.traits.HasTrait(TraitDefOf.Ascetic))
            {
                return -1;
            }else if (IsGreedy(pawn))
            {
                return short.MaxValue;
            }

            return 5;//todo move to setting
        }

        private static bool IsGreedy(Pawn pawn)
        {
            return pawn.story.traits.HasTrait(TraitDefOf.Greedy);
        }


        private static bool IsPretty(Thing x)
        {
            return x.GetInnerIfMinified().GetStatValue(StatDefOf.Beauty) > 10f;
        }

        private static Job InstallJob(Pawn pawn, Blueprint_Install install)
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