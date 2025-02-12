using System;

namespace AppoMobi.Xam
{
    //***********************************************************
    public class FontPreset
    //***********************************************************
    {
	    void Create()
        {
            Id = Guid.NewGuid().ToString();
            TypeName = GetType().Name;
        }

        public FontPreset()
        {
            Create();
        }

        public FontPreset(string filename, double adjustSize, double adjustYPos)
        {
            Create();
            Filename = filename;
            AdjustSize = adjustSize;
            AdjustYPos = adjustYPos;
        }

        public FontPreset(string filename, string filenameBold, double adjustSize, double adjustYPos, double reduceSizeForTabs = 0.0)
        {
            Create();
            Filename = filename;
            FilenameBold = filenameBold;

            Name = Filename;
            NameBold = FilenameBold;

            AdjustSize = adjustSize;
            AdjustYPos = adjustYPos;
            ReduceSizeForTabs = reduceSizeForTabs;
        }
        //-----------------------------------------------------------------
        public FontPreset(string name, string nameBold, string filename,  string filenameBold, double adjustSize, double adjustYPos, double reduceSizeForTabs = 0.0)
        //-----------------------------------------------------------------
        {
            Create();
            Name = name;
            NameBold = nameBold;
            Filename = filename;
            FilenameBold = filenameBold;
            AdjustSize = adjustSize;
            AdjustYPos = adjustYPos;
            ReduceSizeForTabs = reduceSizeForTabs;
        }

        public string TypeName { get; protected set; }
        public string Alias { get; protected set; }
        public string Name { get; set; }
        public string NameBold { get; set; }
        public string Filename { get; set; }
        public string FilenameBold { get; set; }
        public string FilenameItalic { get; set; }
        public double ReduceSizeForTabs { get; set; }
        public double AdjustSize { get; set; }
        public double AdjustYPos { get; set; }
        public string Id { get; set; }

        //new
        public float LetterSpacing = 0.00f;
        public float LineSpacing = 1.00f;
    }
}
