using Babel.System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class GazingLog
{
    public RegionOfInterest roi;
    public int timecode;
    public GazingAction flag;

    public GazingLog()
    {
    }

    public GazingLog(RegionOfInterest roi, int timecode, GazingAction flag)
    {
        this.roi = roi;
        this.timecode = timecode;
        this.flag = flag;
    }


    public override string ToString()
    {
        return timecode.ToString("D4")+
            " frames " + roi.roi_name +
            " " + flag+".";
    }

}
public enum GazingAction
{
    ENTERED,
    LEFT
};
