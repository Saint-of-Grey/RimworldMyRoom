using RimWorld;
using Verse;
using Verse.AI;

namespace MyRoom
{
    public class MentalStateWorker_StealToRoom : MentalStateWorker
    {
        // Token: 0x06003C32 RID: 15410 RVA: 0x001C591C File Offset: 0x001C3D1C
        public override bool StateCanOccur(Pawn pawn)
        {
            if (!base.StateCanOccur(pawn))
            {
                return false;
            }
            Building_Bed ownedBed = pawn.ownership.OwnedBed;
            return ownedBed != null && ownedBed.GetRoom(RegionType.Set_Passable) != null && !ownedBed.GetRoom(RegionType.Set_Passable).PsychologicallyOutdoors;
        }
    }
}