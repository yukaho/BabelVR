using Babel.System.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RegionOfInterestAnimationData
{

    //keyframe time code 
    [JsonPropertyAttribute]
    public int time_code;

    //Position
    [JsonPropertyAttribute]
    public SimpleVector3 position;


}

