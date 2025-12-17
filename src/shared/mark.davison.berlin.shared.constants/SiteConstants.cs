using System;

namespace mark.davison.berlin.shared.constants
{

    public static class SiteConstants
    {
        public static Guid ArchiveOfOurOwn_Id = new Guid("8C5DE266-C026-4550-A24B-F79CE91A529A");
        public static string ArchiveOfOurOwn_ShortName = "AO3";
        public static string ArchiveOfOurOwn_LongName = "Archive Of Our Own";
        public static string ArchiveOfOurOwn_Address = "https://archiveofourown.org";

        public static Guid FakeArchiveOfOurOwn_Id = new Guid("9219661D-657D-4A34-BAD2-76AF92EC41C9");
        public static string FakeArchiveOfOurOwn_ShortName = "FakeAO3";
        public static string FakeArchiveOfOurOwn_LongName = "Fake Archive Of Our Own";
        public static string FakeArchiveOfOurOwn_Address = "https://localhost:50002";
    }

}