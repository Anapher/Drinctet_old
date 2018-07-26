using HashidsNet;

namespace Drinctet.Harvester
{
    public enum SourceIds
    {
        CwsConversationStartersHarvester = 1,
        CswDeepTalkQuestionsHarvester,
        CswFunnyQuestionsHarvester,
        CswNeverHaveIEverHarvester,
        CswPersonalGirlQuestionsHarvester,
        CswPersonalGuyQuestionsHarvester,
        CswTruthOrDareDareHarvester,
        CswTruthOrDareTruthHarvester,
        CswWyrHarvester,
        PicoloHarvester,
        BevilHarvester
    }

    public class CardIdProvider
    {
        public static Hashids GetHashIds() => new Hashids("DrinctetByVG");
    }
}