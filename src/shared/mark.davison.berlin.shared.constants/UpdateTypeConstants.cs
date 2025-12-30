using System;
using System.Collections.Generic;

namespace mark.davison.berlin.shared.constants
{

    public static class UpdateTypeConstants
    {
        public static Guid EachChapterId = new Guid("2BDC2B2D-007E-4C41-9960-0881DF9E7C7A");
        public static Guid WhenCompleteId = new Guid("0D2D3429-520D-4F33-9DCA-1AF7C515362D");
        public static Guid MonthlyWithUpdateId = new Guid("C1972479-70C9-42F7-ADA7-840448573995");

        public static IEnumerable<Guid> AllIds => new List<Guid>
        {
            EachChapterId,
            WhenCompleteId,
            MonthlyWithUpdateId
        };
    }

}